using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackMagic
{
    public class HitPoly : Hitbox
    {
        private Entity parent;

        public Vector2[] Points { get; private set; }
        
        public HitPoly(Entity parent, params Vector2[] points)
        {
            this.parent = parent;
            this.Points = points;
        }

        public HitPoly(Entity parent, List<Vector2> points)
        {
            this.Points = points.ToArray();
        }

        public float GetDist(Vector2 point)
        {
            float min = float.PositiveInfinity;
            for (int i = 0; i < Points.Length; i++)
            {
                //Store the two points on the hitbox that make the edge
                Vector2 point1 = new Vector2(Points[i].X + parent.X, Points[i].Y + parent.Y);

                Vector2 point2;
                if (i == Points.Length - 1) point2 = new Vector2(Points[0].X + parent.X, Points[0].Y + parent.Y);
                else point2 = new Vector2(Points[i + 1].X + parent.X, Points[i + 1].Y + parent.Y);

                //Calculate distance to line
                float dist = (float)DistanceUtils.FindDistanceToSegment(point, point1, point2, out _);

                //Update min value
                if (dist < min) min = dist;
            }
            return min;
        }
    }
}
