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
    public class TailHandler : TUpdates, TDraws
    {
        private readonly Entity parent;
        public Entity Parent { get => parent; }

        //Tail Length Variables
        public const float DefaultAnchorDist = 50f;
        public readonly float AnchorDist; //Distance traveled before placing an anchor
        public int MaxAnchors = 5; //Number of anchors in tail

        //Public travel data
        public float FormingAnchorProgress { get; private set; } //Value from 0->1 to when next anchor will be placed
        public Vector2 TravelDiff { get => prevPos - parent.Pos; }
        public float TravelDiffDist { get => TravelDiff.Length(); }

        //List of all anchors currently placed
        public readonly LinkedList<Vector2> Anchors = new();

        //Anchor placement private variables
        private Vector2 prevPos;
        private float formingAnchorDist = 0; //Distance traveled since last anchor placement


        //Draw variables
        private Texture2D bodyTexture;
        private float bodyRadius = 25;

        //TUpdates priority setting
        public int Priority { get => Trait.defaultPriority; }

        public TailHandler(Entity parent, float anchorDist = DefaultAnchorDist)
        {
            this.parent = parent;
            prevPos = parent.Pos;
            Anchors.AddFirst(prevPos);

            AnchorDist = anchorDist;

            bodyTexture ??= DrawUtils.createCircleTexture(Globals.spriteBatch.GraphicsDevice, (int)(bodyRadius * 2f));
        }

        public void Update()
        {
            prevPos = parent.Pos;

            //TODO: Have this not hard coded to follow mouse position
            MouseState mouse = Mouse.GetState();
            parent.Pos = mouse.Position.ToVector2();

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
        }

        public void Draw()
        {
            //Draw body segments
            for (LinkedListNode<Vector2> cur = Anchors.First, next = cur.Next; next != null; cur = cur.Next, next = cur.Next)
            {
                Globals.spriteBatch.Draw(bodyTexture,
                    Vector2.Lerp(next.Value, cur.Value, FormingAnchorProgress) - Vector2.One * bodyRadius,
                    Color.White);
            }

            //Draw head
            Globals.spriteBatch.Draw(bodyTexture,
                parent.Pos - Vector2.One * bodyRadius,
                Color.White);

            //Draw anchors
            for (var i = Anchors.First; i != null; i = i.Next)
            {
                Globals.spriteBatch.Draw(DrawUtils.createTexture(Globals.spriteBatch.GraphicsDevice),
                    new Rectangle((int)i.Value.X, (int)i.Value.Y, 10, 10),
                    Color.Red);
            }
        }
    }
}
