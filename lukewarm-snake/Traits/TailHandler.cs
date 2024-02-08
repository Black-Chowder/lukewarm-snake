using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace lukewarm_snake
{
    public class TailHandler : TUpdates
    {
        private readonly Entity parent;
        public Entity Parent { get => parent; }

        //Tail Length Variables
        public const float DefaultAnchorDist = 25f;
        public readonly float AnchorDist; //Distance traveled before placing an anchor
        public int MaxAnchors = 10; //Number of anchors in tail

        //Public travel data
        public float FormingAnchorProgress { get; private set; } //Value from 0->1 to when next anchor will be placed
        public Vector2 TravelDiff { get => prevPos - parent.Pos; }
        public float TravelDiffDist { get => TravelDiff.Length(); }

        //List of all anchors currently placed
        public readonly LinkedList<Vector2> Anchors = new();

        //Anchor placement private variables
        private Vector2 prevPos;
        private float formingAnchorDist = 0; //Distance traveled since last anchor placement

        //TUpdates priority setting
        public int Priority { get => Trait.defaultPriority; }

        public TailHandler(Entity parent, float anchorDist = DefaultAnchorDist)
        {
            this.parent = parent;
            prevPos = parent.Pos;
            Anchors.AddFirst(prevPos);

            AnchorDist = anchorDist;
        }

        public void Update()
        {
            //Skip anchor processing if player didn't move
            if (parent.Pos == prevPos)
                return;

            //Place new anchors
            formingAnchorDist += TravelDiffDist;
            while (formingAnchorDist > AnchorDist)
            {
                formingAnchorDist -= AnchorDist;

                //Calculate next anchor position
                Vector2 nextAnchor = parent.Pos - Anchors.First();
                nextAnchor.Normalize();
                nextAnchor *= AnchorDist;
                nextAnchor += Anchors.First();

                Anchors.AddFirst(nextAnchor);
            }
            FormingAnchorProgress = formingAnchorDist / AnchorDist;

            //Remove excess anchors
            while (Anchors.Count > MaxAnchors)
                Anchors.RemoveLast();

            prevPos = parent.Pos;
        }

        public const int AnchorSize = 10;
        private const int halfAnchorSize = AnchorSize / 2;
        public void DrawAnchors()
        {
            for (var i = Anchors.First; i != null; i = i.Next)
            {
                Globals.spriteBatch.Draw(DrawUtils.createTexture(Globals.spriteBatch.GraphicsDevice),
                    new Rectangle((int)i.Value.X - halfAnchorSize, (int)i.Value.Y - halfAnchorSize, AnchorSize, AnchorSize),
                    Color.Red);
            }
        }
    }
}
