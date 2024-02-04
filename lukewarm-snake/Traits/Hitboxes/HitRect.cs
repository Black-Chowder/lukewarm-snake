using Microsoft.Xna.Framework;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace BlackMagic
{
    public class HitRect : Hitbox
    {
        private Entity parent;

        private Rectangle hitbox;
        public Rectangle Rect { get => hitbox; }
        public Rectangle AbsoluteRect { get => new Rectangle((int)AbsoluteX, (int)AbsoluteY, Width, Height); }

        public int X { get => hitbox.X; }
        public float AbsoluteX { get => hitbox.X + parent.X; }
        public int Y { get => hitbox.Y; }
        public float AbsoluteY { get => hitbox.Y + parent.Y; }

        public int Width { get => hitbox.Width; }
        public int Height { get => hitbox.Height; }

        //Collision data tracker
        public Entity Left { get; set; }
        public Entity Right { get; set; }
        public Entity Top { get; set; }
        public Entity Bottom { get; set; }

        //Constructors
        public HitRect(Entity parent)
        {
            this.parent = parent;
            this.hitbox = new Rectangle(0, 0, (int)parent.Width, (int)parent.Height);
        }

        public HitRect(Entity parent, Rectangle rect)
        {
            this.parent = parent;
            this.hitbox = rect;
        }


        public float GetDist(Vector2 point)
        {
            // Ripped from  https://www.youtube.com/watch?v=Cp5WWtMoeKg&t=45s
            // signed -> if point is inside the rect, then dist is negative
            float px = point.X;
            float py = point.Y;
            //These are myself I assume
            float rx = hitbox.X + parent.X;
            float ry = hitbox.Y + parent.Y;
            float rw = hitbox.Width;
            float rh = hitbox.Height;

            float ox = rx + rw / 2;
            float oy = ry + rh / 2;
            float offsetX = Math.Abs(px - ox) - rw / 2;
            float offsetY = Math.Abs(py - oy) - rh / 2;

            float unsignedDist = DistanceUtils.getDistance(0, 0, Math.Max(offsetX, 0), Math.Max(offsetY, 0));
            float distInsideBox = Math.Max(Math.Min(offsetX, 0), Math.Min(offsetY, 0));
            return unsignedDist + distInsideBox;
        }

        public void resetCollisionData()
        {
            Left = null;
            Right = null;
            Top = null;
            Bottom = null;
        }
    }
}
