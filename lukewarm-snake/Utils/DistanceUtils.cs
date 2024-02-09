using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace BlackMagic
{
    public class DistanceUtils
    {
        public static float getDistance(float x1, float y1, float x2, float y2) =>
            MathF.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

        public static float getDistance(Vector2 point1, Vector2 point2) =>
            getDistance(point1.X, point1.Y, point2.X, point2.Y);

        public static float getSqrDist(float x1, float y1, float x2, float y2) =>
            (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);

        public static float getSqrDist(Vector2 point1, Vector2 point2) =>
            getSqrDist(point1.X, point1.Y, point2.X, point2.Y);


        //Credit: http://csharphelper.com/blog/2016/09/find-the-shortest-distance-between-a-point-and-a-line-segment-in-c/
        public static double FindDistanceToSegment(Vector2 pt, Vector2 p1, Vector2 p2, out Vector2 closest)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            float t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) /
                (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closest = new Vector2(p1.X, p1.Y);
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                closest = new Vector2(p2.X, p2.Y);
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closest = new Vector2(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static float Lerp(float firstFloat, float secondFloat, float by) =>
            firstFloat * (1 - by) + secondFloat * by;

        public static Vector2 Lerp(Vector2 firstVector, Vector2 secondVector, float by)
        {
            float retX = Lerp(firstVector.X, secondVector.X, by);
            float retY = Lerp(firstVector.Y, secondVector.Y, by);
            return new(retX, retY);
        }


        // Calculates the shortest difference between two given angles.
        public static float DeltaAngle(float current, float target)
        {
            float delta = Repeat((target - current), MathF.PI * 2f);
            if (delta > MathF.PI)
                delta -= MathF.PI * 2f;
            return delta;
        }

        // Loops the value t, so that it is never larger than length and never smaller than 0.
        public static float Repeat(float t, float length)
        {
            return (float)Math.Clamp(t - MathF.Floor(t / length) * length, 0.0f, length);
        }

        public static bool IsAngleBetween(float angleToCheck, float startAngle, float endAngle)
        {
            // Convert angles to the range from 0 to 2π radians
            angleToCheck = (angleToCheck % (2f * MathF.PI) + 2f * MathF.PI) % (2f * MathF.PI);
            startAngle = (startAngle % (2f * MathF.PI) + 2f * MathF.PI) % (2f * MathF.PI);
            endAngle = (endAngle % (2f * MathF.PI) + 2f * MathF.PI) % (2f * MathF.PI);

            if (startAngle < endAngle)
            {
                // Case 1: startAngle is less than endAngle (no wraparound)
                return angleToCheck >= startAngle && angleToCheck <= endAngle;
            }
            else if (startAngle > endAngle)
            {
                // Case 2: startAngle is greater than endAngle (wraparound)
                return angleToCheck >= startAngle || angleToCheck <= endAngle;
            }
            else
            {
                // Case 3: startAngle and endAngle are equal (range is 2*PI radians)
                return true;
            }
        }

        public static void GetLineEquation(float x1, float y1, float x2, float y2, out float slope, out float yIntercept)
        {
            if (x1 == x2)
            {
                throw new ArgumentException("Cannot calculate line equation for vertical line (x1 == x2).");
            }

            slope = (y2 - y1) / (x2 - x1);
            yIntercept = y1 - slope * x1;
        }

        public static void GetLineEquation(Vector2 p1, Vector2 p2, out float slope, out float yIntercept) =>
            GetLineEquation(p1.X, p1.Y, p2.X, p2.Y, out slope, out yIntercept);


        public static Rectangle GetBounds(List<Vector2> points)
        {
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (Vector2 point in points)
            {
                if (point.X < minX)
                {
                    minX = point.X;
                }
                if (point.X > maxX)
                {
                    maxX = point.X;
                }
                if (point.Y < minY)
                {
                    minY = point.Y;
                }
                if (point.Y > maxY)
                {
                    maxY = point.Y;
                }
            }

            return new Rectangle((int)minX, (int)minY, (int)(maxX - minX), (int)(maxY - minY));
        }

        public static float GetCircleRadiusForRectangle(Rectangle rectangle)
        {
            float widthHalf = rectangle.Width / 2f;
            float heightHalf = rectangle.Height / 2f;
            return (float)Math.Sqrt(widthHalf * widthHalf + heightHalf * heightHalf);
        }
    }
}
