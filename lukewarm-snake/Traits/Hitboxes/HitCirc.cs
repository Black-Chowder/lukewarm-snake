using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackMagic
{
    public class HitCirc : Hitbox
    {
        private Entity parent;

        public Vector2 Origin { get; private set; }
        public float Radius { get; private set; }

        public HitCirc(Entity parent, Vector2 origin, float radius)
        {
            this.parent = parent;
            this.Origin = origin;
            this.Radius = radius;
        }

        public float GetDist(Vector2 point)
        {
            float cx = Origin.X + parent.X;
            float cy = Origin.Y + parent.Y;
            float cr = Radius / 2;

            return DistanceUtils.getDistance(point.X, point.Y, cx, cy) - cr;
        }
    }
}
