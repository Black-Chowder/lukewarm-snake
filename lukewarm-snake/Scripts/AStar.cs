using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Priority_Queue;
using BlackMagic;

namespace AStar2D
{
    public class AStar
    {
        //Class for A*'s nodes to represent grid being traversed
        private class Node
        {
            public int x;
            public int y;

            //A* variables
            public int GCost; //Distance from start node
            public int HCost; //(herustic) distance from end node
            public int FCost //GCost + FCost
            {
                get { return GCost + HCost; }
            }
            public Node prev;
            public bool walkable = true;

            //Temporary Barrier Variables
            public bool tempBarrier
            {
                get
                {
                    return tempBarriers.Count > 0 ? true : false;
                }
            }
            public List<Rectangle> tempBarriers;


            public Node(int x, int y)
            {
                this.x = x;
                this.y = y;
                tempBarriers = new List<Rectangle>();
            }
        }

        //Position (top left) of node grid
        public readonly float x;
        public readonly float y;

        //Actual dimensions of grid according to space it takes up (not number of nodes)
        public readonly float width;
        public readonly float height;

        //Gets the dimensions of the node grid
        public int gridWidth { get { return nodes.GetLength(0); } }
        public int gridHeight { get { return nodes.GetLength(1); } }

        //Space between each node
        public const float defaultSpacing = 32f;
        public readonly float spacing = defaultSpacing;

        public List<Rectangle> tempBarriers;

        private Node[,] nodes;

        //Constructor (duh)
        public AStar(float x, float y, float width, float height, float spacing = defaultSpacing)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.spacing = spacing;

            tempBarriers = new List<Rectangle>();

            //Create grid of nodes
            createGrid();
        }

        //Creates a grid of nodes
        private void createGrid()
        {
            nodes = new Node[(int)MathF.Floor(width / spacing), (int)MathF.Floor(height / spacing)];

            for (int x = 0; x < nodes.GetLength(0); x++)
            {
                for (int y = 0; y < nodes.GetLength(1); y++)
                {
                    nodes[x, y] = new Node(x, y);
                }
            }
        }

        //Pretty self-explanatory, no?
        public void AddBarrier<T>(T shape, Func<T, Vector2, bool> pointInShape)
        {
            for (int x = 0; x < nodes.GetLength(0); x++)
            {
                for (int y = 0; y < nodes.GetLength(1); y++)
                {
                    if (pointInShape(shape, new Vector2(x * spacing + this.x, y * spacing + this.y)))
                    {
                        nodes[x, y].walkable = false;
                    }
                }
            }
        }
        public void AddBarriers<T>(T[] shapes, Func<T, Vector2, bool> pointInShape)
        {
            foreach (T shape in shapes)
            {
                AddBarrier(shape, pointInShape);
            }
        }
        public void AddBarrier(Rectangle barrier)
        {
            float startX = (barrier.X - this.x) / spacing;
            float startY = (barrier.Y - this.y) / spacing;

            for (int x = startX <= 0 ? 0 : (int)(startX + 1); x <= (int)((barrier.X + barrier.Width - this.x) / spacing) && x < nodes.GetLength(0); x++)
            {
                for (int y = startY <= 0 ? 0 : (int)(startY + 1); y <= (int)((barrier.Y + barrier.Height - this.y) / spacing) && y < nodes.GetLength(1); y++)
                {
                    nodes[x, y].walkable = false;
                }
            }
        }
        public void AddBarriers(List<Rectangle> barriers)
        {
            AddBarriers(barriers.ToArray());
        }
        public void AddBarriers(Rectangle[] barriers)
        {
            //Loop through grid and find those intersecting
            foreach (Rectangle barrier in barriers)
            {
                AddBarrier(barrier);
            }
        }

        public void ClearBarriers(Rectangle[] barriers)
        {
            for (int x = 0; x < nodes.GetLength(0); x++)
            {
                for (int y = 0; y < nodes.GetLength(1); y++)
                {
                    nodes[x, y].walkable = true;
                }
            }
        }

        //Temporary Barrier Commands
        public void AddTempBarrier(Rectangle barrier)
        {
            tempBarriers.Add(barrier);
            float startX = (barrier.X - this.x) / spacing;
            float startY = (barrier.Y - this.y) / spacing;

            for (int x = startX <= 0 ? 0 : (int)(startX + 1); x <= (int)((barrier.X + barrier.Width - this.x) / spacing) && x < nodes.GetLength(0); x++)
            {
                for (int y = startY <= 0 ? 0 : (int)(startY + 1); y <= (int)((barrier.Y + barrier.Height - this.y) / spacing) && y < nodes.GetLength(1); y++)
                {
                    nodes[x, y].tempBarriers.Add(barrier);
                }
            }
        }

        public bool RemoveTempBarrier(Rectangle barrier)
        {
            //Quick return false if temp barrier doesn't exist in list
            if (!tempBarriers.Remove(barrier)) return false;

            float startX = (barrier.X - this.x) / spacing;
            float startY = (barrier.Y - this.y) / spacing;

            for (int x = startX <= 0 ? 0 : (int)(startX + 1); x <= (int)((barrier.X + barrier.Width - this.x) / spacing) && x < nodes.GetLength(0); x++)
            {
                for (int y = startY <= 0 ? 0 : (int)(startY + 1); y <= (int)((barrier.Y + barrier.Height - this.y) / spacing) && y < nodes.GetLength(1); y++)
                {
                    nodes[x, y].tempBarriers.Remove(barrier);
                }
            }

            return true;
        }

        public void ClearTempBarriers()
        {
            tempBarriers.Clear();
            for (int x = 0; x < nodes.GetLength(0); x++)
            {
                for (int y = 0; y < nodes.GetLength(1); y++)
                {
                    nodes[x, y].tempBarriers.Clear();
                }
            }
        }

        public Rectangle[] GetTempBarriers()
        {
            return tempBarriers.ToArray();
        }

        //Finds the path from a start position and an end position
        public Vector2[] FindPath(Vector2 start, Vector2 end) { return FindPath(start.X, start.Y, end.X, end.Y); }
        public Vector2[] FindPath(float startX, float startY, float endX, float endY)
        {
            //Handle if start or end are out of bounds
            if (startX - x < 0 || (startX - x) / spacing >= nodes.GetLength(0) || startY - y < 0 || (startY - y) / spacing >= nodes.GetLength(1))
            {
                return null;
            }

            //Get start and end nodes
            Node start = nodes[(int)MathF.Round((startX - x) / spacing), (int)MathF.Round((startY - y) / spacing)];
            Node end = nodes[(int)MathF.Round((endX - x) / spacing), (int)MathF.Round((endY - y) / spacing)];

            SimplePriorityQueue<Node, int> open = new SimplePriorityQueue<Node, int>(); //Set of nodes to be evaluated
            HashSet<Node> closed = new HashSet<Node>(); //Set of nodes already evaluated

            open.Enqueue(start, 0);
            while (open.Count > 0)
            {
                Node target = open.Dequeue();
                closed.Add(target);

                if (target == end) //<= if true, then path has been found
                {
                    List<Node> path = retracePath(start, end);

                    //Convert from list of nodes to array of vectors
                    Vector2[] toReturn = new Vector2[path.Count];
                    for (int i = 0; i < path.Count; i++)
                    {
                        toReturn[i] = new Vector2(path[i].x * spacing + x, path[i].y * spacing + y);
                    }
                    return toReturn;
                }

                //foreach neighbor of current node
                foreach (Node neighbor in getNeighbors(target))
                {
                    //If neighbor is not traversable or neighbor is in CLOSED, skip to the next neighbor
                    if (!neighbor.walkable || closed.Contains(neighbor) || neighbor.tempBarrier) continue;

                    int newMovementCostToNeighbor = target.GCost + getDist(target, neighbor);
                    if (newMovementCostToNeighbor < neighbor.GCost || !open.Contains(neighbor))
                    {
                        neighbor.GCost = newMovementCostToNeighbor;
                        neighbor.HCost = getDist(neighbor, end);
                        neighbor.prev = target;

                        open.EnqueueWithoutDuplicates(neighbor, neighbor.FCost);
                    }
                }
            }

            //Theoretically, no path is available if it gets here, so return null
            return null;
        }

        //Is used to get the path once astar has reached the end node
        private List<Node> retracePath(Node start, Node end)
        {
            List<Node> path = new List<Node>();
            Node target = end;
            while (target != start)
            {
                path.Add(target);
                target = target.prev;
            }
            path.Add(start);

            return path;
        }

        //Gets the neighboring nodes of teh given node
        private List<Node> getNeighbors(Node node)
        {
            List<Node> neighbors = new List<Node>(8);

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue; //is self, then skip

                    int checkX = node.x + x;
                    int checkY = node.y + y;

                    //Add neighbor
                    if (checkX >= 0 && checkX < nodes.GetLength(0) && checkY >= 0 && checkY < nodes.GetLength(1))
                        neighbors.Add(nodes[checkX, checkY]);
                }
            }

            return neighbors;
        }

        //Gets the distance from one node to another (not true distance, but approximation)
        private int getDist(Node nodeA, Node nodeB)
        {
            int distX = (int)MathF.Abs(nodeA.x - nodeB.x);
            int distY = (int)MathF.Abs(nodeA.y - nodeB.y);

            if (distX > distY)
                return 14 * distY + 10 * (distX - distY);
            return 14 * distX + 10 * (distY - distX);
        }

        public void Draw(SpriteBatch spriteBatch, float offsetX = 0, float offsetY = 0, float scale = 1)
        {
            for (int x = 0; x < nodes.GetLength(0); x++)
            {
                for (int y = 0; y < nodes.GetLength(1); y++)
                {
                    spriteBatch.Draw(DrawUtils.createTexture(spriteBatch.GraphicsDevice),
                        new Vector2((x * spacing + this.x - offsetX) * scale, (y * spacing + this.y - offsetY) * scale),
                        new Rectangle(0, 0, 1, 1),
                        nodes[x, y].walkable ? (nodes[x, y].tempBarrier ? Color.Orange : Color.Blue) : Color.Red,
                        0,
                        new Vector2(0, 0),
                        3,
                        SpriteEffects.None,
                        .99f);
                }
            }
        }
    }
}
