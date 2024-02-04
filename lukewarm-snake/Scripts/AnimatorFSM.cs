using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectAstrid
{
    //Animator Finite State Machine.  Used for transitioning between animations
    public class AnimatorFSM
    {
        public const byte priority = TAnimator.priority - 1;

        public class Node
        {
            public delegate void WhilePlayingFunc();
            public WhilePlayingFunc whilePlayingFunc { get; private set; } //Function that plays while this node is active

            public string Name { get; private set; }
            private List<Edge> edges;
            public int SetToIndex { get; private set; }

            public Node(string animationName, int setToIndex = 0, WhilePlayingFunc whilePlayingFunc = null)
            {
                Name = animationName;
                SetToIndex = setToIndex;
                edges = new List<Edge>();
                this.whilePlayingFunc = whilePlayingFunc ?? (() => { });
            }

            //Connects a node to another node when provided transitionFunc returns true
            public void ConnectTo(Node node, Edge.TransitionFunc transitionFunc) => edges.Add(new Edge(this, node, transitionFunc));
            public void AddEdge(Edge edge) => edges.Add(edge);

            //Returns node of next animation to play
            public Node CheckTransitions()
            {
                for (int i = 0; i < edges.Count; i++)
                {
                    Edge edge = edges[i];
                    bool check = edge.Check();
                    if (check)
                    {
                        Node newAnimation = edge.Head;
                        return newAnimation;
                    }
                }

                //If no edge checks passes, return self
                return this;
            }
        }

        public class Edge
        {
            public delegate bool TransitionFunc();
            private TransitionFunc transitionFunc;
            public Node Tail { get; private set; }
            public Node Head { get; private set; }

            //edge goes from tail to head
            public Edge(Node tail, Node head, TransitionFunc transitionFunc)
            {
                Tail = tail;
                Head = head;
                this.transitionFunc = transitionFunc;
            }

            public bool Check() => transitionFunc();
        }


        private Animator animator;
        private Node curNode;

        public AnimatorFSM(Node graphStart, Animator animator)
        {
            curNode = graphStart;
            this.animator = animator;
        }

        public void Update()
        {
            curNode.whilePlayingFunc();
            Node nextNode = curNode.CheckTransitions();
            if (curNode == nextNode) return;

            curNode = nextNode;
            animator.SetAnimation(curNode.Name, curNode.SetToIndex);
        }
    }
}
