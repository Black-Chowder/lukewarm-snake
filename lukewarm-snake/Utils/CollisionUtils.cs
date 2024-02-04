using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BlackMagic
{
    public class CollisionUtils
    {
        public static bool pointRectCollision(float pointX, float pointY, float x, float y, float w, float h)
        {
            if (x < pointX && pointX < x + w && y < pointY && pointY < y + h)
            {
                return true;
            }

            return false;
        }

        public static bool rectCollision(float x1, float y1, float w1, float h1, float x2, float y2, float w2, float h2)
        {
            return (
                y1 + h1 > y2 &&
                y1 < y2 + h2 &&
                x1 < x2 + w2 &&
                x1 + w1 > x2
                );
        }

        //Gotten from https://stackoverflow.com/questions/4243042/c-sharp-point-in-polygon#comment68419447_7123291
        /// <summary>
        /// Determines if the given point is inside the polygon
        /// </summary>
        /// <param name="polygon">the vertices of polygon</param>
        /// <param name="testPoint">the given point</param>
        /// <returns>true if the point is inside the polygon; otherwise, false</returns>
        public static bool IsPointInPolygon4(Vector2[] polygon, Vector2 testPoint)
        {
            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y || polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y)
                {
                    if (polygon[i].X + (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < testPoint.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }
    }
}
