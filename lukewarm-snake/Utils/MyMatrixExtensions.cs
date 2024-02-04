using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using System.Reflection.Metadata.Ecma335;

namespace BlackMagic
{
    public static class MyMatrixExtensions
    {
        //Source: https://stackoverflow.com/questions/21986909/convert-multidimensional-array-to-jagged-array-in-c-sharp
        internal static T[][] ToJaggedArray<T>(this T[,] twoDimensionalArray)
        {
            int rowsFirstIndex = twoDimensionalArray.GetLowerBound(0);
            int rowsLastIndex = twoDimensionalArray.GetUpperBound(0);
            int numberOfRows = rowsLastIndex + 1;

            int columnsFirstIndex = twoDimensionalArray.GetLowerBound(1);
            int columnsLastIndex = twoDimensionalArray.GetUpperBound(1);
            int numberOfColumns = columnsLastIndex + 1;

            T[][] jaggedArray = new T[numberOfRows][];
            for (int i = rowsFirstIndex; i <= rowsLastIndex; i++)
            {
                jaggedArray[i] = new T[numberOfColumns];

                for (int j = columnsFirstIndex; j <= columnsLastIndex; j++)
                {
                    jaggedArray[i][j] = twoDimensionalArray[i, j];
                }
            }
            return jaggedArray;
        }

        //Transpose a matrix
        internal static T[,] Transpose<T>(this T[,] matrix)
        {
            T[,] newMatrix = new T[matrix.GetLength(1), matrix.GetLength(0)];

            for (int x = 0; x < matrix.GetLength(0); x++)
            {
                for (int y = 0; y < matrix.GetLength(1); y++)
                {
                    newMatrix[y, x] = matrix[x, y];
                }
            }

            return newMatrix;
        }

        //Print generic type matrix
        internal static void Print<T>(this T[,] matrix)
        {
            if (matrix.GetLength(0) == 0 || matrix.GetLength(1) == 0) throw new ArgumentOutOfRangeException(nameof(matrix), " was empty");
            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                for (int col = 0; col < matrix.GetLength(1); col++)
                {
                    Debug.Write(matrix[row, col]);
                }
                Debug.WriteLine("");
            }
        }

        //Returns-by-value the given matrix
        internal static T[,] Clone<T>(this T[,] matrix)
        {
            T[,] newMatrix = new T[matrix.GetLength(0), matrix.GetLength(1)];

            for (int x = 0; x < matrix.GetLength(0); x++)
            {
                for (int y = 0; y < matrix.GetLength(1); y++)
                {
                    newMatrix[x, y] = matrix[x, y];
                }
            }

            return newMatrix;
        }

        //Converts an array to a multidimensional array
        internal static T[,] ToMatrix<T> (this T[] array, int columns)
        {
            int rows = array.Length / columns;
            T[,] toReturn = new T[rows, columns];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    toReturn[r, c] = array[c + r * columns];
                }
            }
            return toReturn;
        }

        public static T[,] CopySection<T>(this T[,] matrix, Rectangle toCopy)
        {
            T[,] toReturn = new T[toCopy.Width, toCopy.Height];
            for (int r = toCopy.X, or = 0; or < toCopy.Width; r++, or++)
            {
                for (int c = toCopy.Y, oc = 0; oc < toCopy.Height; c++, oc++)
                {
                    toReturn[or, oc] = matrix[r, c];
                }
            }

            return toReturn;
        }

        //Converts an uint array to an int array
        internal static int[] ToSignedArray(this uint[] given)
        {
            int[] toReturn = new int[given.Length];
            for (int i = 0; i < given.Length; i++)
            {
                toReturn[i] = (int)given[i];
            }
            return toReturn;
        }

        //Creates matrices with holes removed
        internal static List<int[,]> RemoveDonuts(this int[,] _matrix)
        {
            int[,] matrix = _matrix.Clone<int>();

            List<int[,]> donutsRemoved = new List<int[,]>();

            //Recursively remove donuts
            matrix.RemoveDonuts(donutsRemoved);

            //Remove islands
            List<int[,]> islandsIsolated = new List<int[,]>();
            foreach (int[,] m in donutsRemoved)
                islandsIsolated.AddRange(m.IsolateIslands());

            //Clean up returned matrices
            //(set -1 values to 0)
            foreach (int[,] m in islandsIsolated)
                for (int r = 0; r < m.GetLength(0); r++)
                    for (int c = 0; c < m.GetLength(1); c++)
                        m[r, c] = m[r, c] == -1 ? 0 : m[r, c];

            return islandsIsolated;
        }

        private static void RemoveDonuts(this int[,] matrix, List<int[,]> holeless)
        {
            Point? topHole = FindTopHole(ref matrix);
            if (topHole == null)
                holeless.Add(matrix);
            else
            {
                int[][,] results = matrix.SplitHorizontal(topHole.Value.X);
                RemoveDonuts(results[0], holeless);
                RemoveDonuts(results[1], holeless);
            }
        }

        private static Point? FindTopHole(ref int[,] matrix)
        {
            for (int r = 1; r < matrix.GetLength(0) - 1; r++)
            {
                for (int c = 1; c < matrix.GetLength(1) - 1; c++)
                {
                    if (matrix[r, c] != 0)
                        continue;

                    Point? result = FindTopHole(ref matrix, new Point(r, c));
                    if (result != null)
                        return result;
                }
            }
            return null;
        }

        private static Point? FindTopHole(ref int[,] matrix, Point cur)
        {
            Point? topHole = cur;
            matrix[cur.X, cur.Y] = -1;

            //If touching edge, return null
            if (cur.X == matrix.GetLength(0) - 1 || cur.X == 0 || cur.Y == matrix.GetLength(1) - 1 || cur.Y == 0)
                return null;

            for (int dr = -1; dr <= 1; dr++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    //Ignore corners and center
                    if (dr == dc || dr == dc * -1)
                        continue;

                    Point pointToSearch = new Point(cur.X + dr, cur.Y + dc);
                    if (matrix[pointToSearch.X, pointToSearch.Y] == 0)
                    {
                        Point? result = FindTopHole(ref matrix, pointToSearch);

                        if (result == null)
                            topHole = null;

                        if (topHole != null && result.Value.Y > topHole.Value.Y)
                            topHole = result;
                    }
                }
            }

            return topHole;
        }

        private static List<int[,]> IsolateIslands(this int[,] given)
        {
            List<int[,]> toReturn = new List<int[,]>();

            int[,] matrix = given.Clone<int>();

            for (int r = 0; r < matrix.GetLength(0); r++)
            {
                for (int c = 0; c < matrix.GetLength(1); c++)
                {
                    if (matrix[r, c] <= 0)
                        continue;

                    int[,] result = new int[matrix.GetLength(0), matrix.GetLength(1)];
                    IsolateIsland(ref matrix, new Point(r, c), ref result);

                    toReturn.Add(result);
                }
            }
            return toReturn;
        }

        private static void IsolateIsland(ref int[,] matrix, Point cur, ref int[,] result)
        {
            result[cur.X, cur.Y] = matrix[cur.X, cur.Y];
            matrix[cur.X, cur.Y] = 0;

            for (int r = cur.X == 0 ? 0 : -1; r <= 1 && r + cur.X < matrix.GetLength(0); r++)
            {
                for (int c = cur.Y == 0 ? 0 : -1; c <= 1 && c + cur.Y < matrix.GetLength(1); c++)
                {
                    if (MathF.Abs(r) == MathF.Abs(c) || (r == 0 && c == 0))
                        continue;

                    if (matrix[cur.X + r, cur.Y + c] <= 0)
                        continue;

                    IsolateIsland(ref matrix, cur + new Point(r, c), ref result);
                }
            }
        }

        public static T[][,] SplitVertical<T>(this T[,] matrix, int col)
        {
            T[][,] results = new T[2][,];

            T[,] left = new T[matrix.GetLength(0), col];
            T[,] right = new T[matrix.GetLength(0), matrix.GetLength(1) - col];
            results[0] = left;
            results[1] = right;
            
            //Copy data into top
            for (int r = 0; r < matrix.GetLength(0); r++)
                for (int c = 0; c < col; c++)
                    left[r, c] = matrix[r, c];

            //Copy data into bottom
            for (int r = 0; r < matrix.GetLength(0); r++)
                for (int c = 0; c < matrix.GetLength(1) - col; c++)
                    right[r, c] = matrix[r, c + col];

            return results;
        }

        public static T[][,] SplitHorizontal<T>(this T[,] matrix, int row)
        {
            T[][,] results = new T[2][,];

            T[,] top = new T[matrix.GetLength(0), matrix.GetLength(1)];
            T[,] bottom = new T[matrix.GetLength(0), matrix.GetLength(1)];
            results[0] = top;
            results[1] = bottom;

            //Copy data into top
            for (int r = 0; r < row; r++)
                for (int c = 0; c < matrix.GetLength(1); c++)
                    top[r, c] = matrix[r, c];

            //Copy data into bottom
            for (int r = row; r < matrix.GetLength(0); r++)
                for (int c = 0; c < matrix.GetLength(1); c++)
                    bottom[r, c] = matrix[r, c];

            return results;
        }

         

        //Creates polygons generated from a given 2D matrix (polygon represented with List<Vector2>)
        internal static List<List<Vector2>> ToPoly(this int[,] _matrix)
        {
            //TODO: Set polygon position to proper position

            List<List<Vector2>> toReturn = new List<List<Vector2>>();

            //Remove holes
            List<int[,]> preppedMatrices = _matrix.Transpose().RemoveDonuts();

            //for each matrix, convert to polygon
            foreach (int[,] matrix in preppedMatrices)
            {
                List<Edge> edgePool = new List<Edge>();

                for (int r = 0; r < matrix.GetLength(0); r++)
                {
                    for (int c = 0; c < matrix.GetLength(1); c++)
                    {
                        if (matrix[r, c] == 0) continue;

                        //Check Left (0)
                        if ((r != 0 && matrix[r - 1, c] == 0) || r == 0)
                            edgeCheck(edgePool, r, c, Edge.LEFT);

                        //Check Right (1)
                        if ((r != matrix.GetLength(0) - 1 && matrix[r + 1, c] == 0) || r == matrix.GetLength(0) - 1)
                            edgeCheck(edgePool, r, c, Edge.RIGHT);

                        //Check Top (2)
                        if ((c != 0 && matrix[r, c - 1] == 0) || c == 0)
                            edgeCheck(edgePool, r, c, Edge.TOP);

                        //Check Bottom (3)
                        if ((c != matrix.GetLength(1) - 1 && matrix[r, c + 1] == 0) || c == matrix.GetLength(1) - 1)
                            edgeCheck(edgePool, r, c, Edge.BOTTOM);

                    }
                }

                //Get points in where they connect
                List<Edge> sortedEdges = SortEdges(edgePool);

                //Turn edges into points
                List<Vector2> points = getPoints(sortedEdges);

                toReturn.Add(points);
            }

            return toReturn;
        }

        //Checks if edge should be created or if there already exists one to extend
        private static List<Edge> edgeCheck(List<Edge> edgePool, int x, int y, int edge)
        {
            //Check if edge already exists in proper direction
            bool foundOne = false;
            for (int i = 0; i < edgePool.Count; i++)
            {
                //Edge must be on same side as looking for
                if (edge != edgePool[i].edge) continue;

                //Must check proper adjacent coordinate
                if (edgePool[i].end.X == x + (edge == Edge.RIGHT ? 1 : 0) &&
                    edgePool[i].end.Y == y + (edge == Edge.BOTTOM ? 1 : 0))
                {
                    //Extend Edge
                    edgePool[i].end.X = x + (edge != Edge.LEFT ? 1 : 0);
                    edgePool[i].end.Y = y + (edge != Edge.TOP ? 1 : 0);
                    foundOne = true;
                    break;
                }
            }

            //Else, create an edge
            if (foundOne) return edgePool;
            edgePool.Add(new Edge(
                new Vector2(x + (edge == Edge.RIGHT ? 1 : 0), y + (edge == Edge.BOTTOM ? 1 : 0)),
                new Vector2(x + (edge != Edge.LEFT ? 1 : 0), y + (edge != Edge.TOP ? 1 : 0)),
                edge));

            return edgePool;
        }

        //Sorts edges so that the ends of edges line up with their next edge's start
        private static List<Edge> SortEdges(List<Edge> edgePool)
        {
            List<Edge> sorted = new List<Edge>();
            sorted.Add(edgePool[0]);
            return SortEdges(edgePool, sorted);
        }
        private static List<Edge> SortEdges(List<Edge> edgePool, List<Edge> sorted)
        {
            //Store previous edge (edge at top of sorted list)
            Edge previousEdge = sorted[sorted.Count - 1];

            //Returns completed list once edges have completed a loop
            if (sorted[0].start == sorted[sorted.Count - 1].end)
            {
                //Check if there are still edges to be sorted
                if (edgePool.Count != 0)
                {
                    //Edge newEdge = new Edge(sorted[sorted.Count - 1].end, new Vector2(0, 0));
                    //Find closest edge
                    Edge closestEdge = edgePool[0];
                    Edge lastEdge = sorted[sorted.Count - 1];
                    for (int i = 0; i < edgePool.Count; i++)
                    {
                        if (DistanceUtils.getDistance(closestEdge.start, lastEdge.end) > DistanceUtils.getDistance(edgePool[i].start, lastEdge.end) ||
                            DistanceUtils.getDistance(closestEdge.start, lastEdge.end) > DistanceUtils.getDistance(edgePool[i].end, lastEdge.end) ||
                            DistanceUtils.getDistance(closestEdge.end, lastEdge.end) > DistanceUtils.getDistance(edgePool[i].start, lastEdge.end) ||
                            DistanceUtils.getDistance(closestEdge.end, lastEdge.end) > DistanceUtils.getDistance(edgePool[i].end, lastEdge.end))
                        {
                            closestEdge = edgePool[i];
                        }
                    }

                    //Create new edge connecting sorted to closest edge
                    //Check if new edge needs to be flipped
                    if (DistanceUtils.getDistance(closestEdge.start, lastEdge.end) > DistanceUtils.getDistance(closestEdge.end, lastEdge.end))
                    {
                        closestEdge.Flip();
                    }

                    Edge newEdge = new Edge(lastEdge.end, closestEdge.start);
                    sorted.Add(newEdge);
                    sorted.Add(closestEdge);
                    edgePool.Remove(closestEdge);
                    return SortEdges(edgePool, sorted);
                }
            }


            //Loops through edgePool to find next edge
            for (int i = 0; i < edgePool.Count; i++)
            {
                //Skips edge if already in sorted list
                if (isRedundant(sorted, edgePool[i])) continue;

                //If this edge's start == the previous edge's end, add to list
                if (edgePool[i].start == previousEdge.end)
                {
                    sorted.Add(edgePool[i]);
                    edgePool.RemoveAt(i);
                    return SortEdges(edgePool, sorted);
                }
                //Edge case where edge is flipped
                else if (edgePool[i].end == previousEdge.end)
                {
                    edgePool[i].Flip();
                    sorted.Add(edgePool[i]);
                    edgePool.RemoveAt(i);
                    return SortEdges(edgePool, sorted);
                }
            }

            //Close loop by creating new edge, if edges don't loop
            if (sorted[0].start != sorted[sorted.Count - 1].end)
            {
                Edge lastEdge = sorted[sorted.Count - 1];
                Edge newEdge = new Edge(lastEdge.end, sorted[0].start);
                sorted.Add(newEdge);
            }

            //Return sorted edges
            return sorted;
        }

        //Used to tell if an edge already exists in a list
        private static bool isRedundant(List<Edge> edges, Edge edge)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                if (edges[i] == edge)
                {
                    return true;
                }
            }
            return false;
        }

        //Gets points from a list of sorted edges
        private static List<Vector2> getPoints(List<Edge> edges)
        {
            List<Vector2> points = new List<Vector2>();

            for (int i = 0; i < edges.Count; i++)
            {
                points.Add(edges[i].start);
            }

            return points;
        }

        private class Edge
        {
            public Vector2 start;
            public Vector2 end;
            public int edge; //0 = left, 1 = right, 2 = top, 3 = bottom

            public const int LEFT = 0;
            public const int RIGHT = 1;
            public const int TOP = 2;
            public const int BOTTOM = 3;

            //Class to store edges of polygon
            public Edge(Vector2 start, Vector2 end, int edge = LEFT)
            {
                this.start = start;
                this.end = end;
                this.edge = edge;
            }

            public override string ToString()
            {
                return ("Start = " + start + " | End = " + end + " | Edge = " + edge);
            }

            //Flips start and end points of edge
            public void Flip()
            {
                Vector2 temp = new Vector2(start.X, start.Y);

                start.X = end.X;
                start.Y = end.Y;

                end.X = temp.X;
                end.Y = temp.Y;
            }
        }

        //Converts 2D matrix to list of rectangles
        public static List<Rectangle> ToRectangles(this int[,] m)
        {
            List<Rectangle> results = new List<Rectangle>();
            int[,] matrix = m.Clone<int>();

            for (int r = 0; r < matrix.GetLength(0); r++)
            {
                for (int c = 0; c < matrix.GetLength(1); c++)
                {
                    if (matrix[r, c] == 0)
                        continue;

                    Rectangle rect = getRectAtPoint(matrix, new Point(r, c));
                    removeRect(ref matrix, rect);
                    results.Add(rect);
                }
            }

            return results;
        }

        private static Rectangle getRectAtPoint(int[,] matrix, Point start)
        {
            //Get width
            int width = -1;
            for (int r = start.X; r < matrix.GetLength(0); r++)
            {
                if (matrix[r, start.Y] == 0)
                {
                    width = r - start.X;
                    break;
                }
            }
            if (width < 0)
                width = matrix.GetLength(0) - start.X;

            //Get height
            int height = 0;
            for (int c = start.Y; c < matrix.GetLength(1); c++)
            {
                bool isValidRow = true;
                for (int r = start.X; r < start.X + width; r++)
                {
                    if (matrix[r, c] != 0)
                        continue;
                    isValidRow = false;
                    break;
                }

                if (isValidRow)
                    height++;
                else
                    break;
            }

            return new Rectangle(start.X, start.Y, width, height);
        }

        private static void removeRect(ref int[,] matrix, Rectangle rect)
        {
            for (int r = rect.X; r < rect.Right; r++)
            {
                for (int c = rect.Y; c < rect.Bottom; c++)
                {
                    matrix[r, c] = 0;
                }
            }
        }
    }
}
