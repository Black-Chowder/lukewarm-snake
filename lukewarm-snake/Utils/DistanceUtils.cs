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

        public static float GetEnclosingRadius(Rectangle rectangle)
        {
            // Calculate the center of the rectangle
            Vector2 center = new Vector2(rectangle.X + rectangle.Width / 2f, rectangle.Y + rectangle.Height / 2f);

            // Calculate distances from the center to each corner
            float[] distances = new float[4];
            distances[0] = Vector2.Distance(center, new Vector2(rectangle.Left, rectangle.Top));
            distances[1] = Vector2.Distance(center, new Vector2(rectangle.Right, rectangle.Top));
            distances[2] = Vector2.Distance(center, new Vector2(rectangle.Left, rectangle.Bottom));
            distances[3] = Vector2.Distance(center, new Vector2(rectangle.Right, rectangle.Bottom));

            // Find the maximum distance
            float maxDistance = distances[0];
            for (int i = 1; i < distances.Length; i++)
            {
                if (distances[i] > maxDistance)
                    maxDistance = distances[i];
            }

            // Return the maximum distance as the radius
            return maxDistance;
        }

        public static float[] GetEncompassingAngles(Rectangle rectangle, Vector2 point)
        {
            // Calculate the center of the rectangle
            Vector2 center = new Vector2(rectangle.X + rectangle.Width / 2f, rectangle.Y + rectangle.Height / 2f);

            // Calculate the vector from the center to the point
            Vector2 toPoint = point - center;

            // Calculate the vectors from the center to each corner of the rectangle
            Vector2[] cornerVectors = new Vector2[4];
            cornerVectors[0] = new Vector2(rectangle.Left, rectangle.Top) - center;
            cornerVectors[1] = new Vector2(rectangle.Right, rectangle.Top) - center;
            cornerVectors[2] = new Vector2(rectangle.Left, rectangle.Bottom) - center;
            cornerVectors[3] = new Vector2(rectangle.Right, rectangle.Bottom) - center;

            // Calculate the angles between the vector to the point and each corner vector
            float[] angles = new float[4];
            for (int i = 0; i < cornerVectors.Length; i++)
            {
                angles[i] = MathHelper.ToDegrees((float)Math.Atan2(cornerVectors[i].Y, cornerVectors[i].X) -
                                                 (float)Math.Atan2(toPoint.Y, toPoint.X));
                if (angles[i] < 0)
                    angles[i] += 360;
            }

            // Find the two smallest angles
            Array.Sort(angles);
            float[] encompassingAngles = { angles[0], angles[1] };

            return encompassingAngles;
        }
    }
}
