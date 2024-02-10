using BlackMagic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lukewarm_snake
{
    public class SpawnManager : Entity
    {
        //Obstacle spawning params
        public float SpawnObstacleRate = 100f;
        public float SpawnObstacleAccumulator = 0f;

        public float ObstacleSpeed = 1f;

        public static float ObstacleSpawnDist { get; private set; } = 0f;

        Food food = null;
        public const int FoodSpawnPadding = 50;

        public SpawnManager() : base(Vector2.Zero)
        {
            if (ObstacleSpawnDist == 0f) 
                ObstacleSpawnDist = DistanceUtils.GetCircleRadiusForRectangle(new Rectangle(0, 0, Globals.Camera.Width, Globals.Camera.Height));
        }

        public override void Update()
        {
            //Spawn obstacles
            SpawnObstacleAccumulator += MathF.Max(ObstacleMovement.MinTimeMod, Globals.TimeMod);
            while (SpawnObstacleAccumulator > SpawnObstacleRate)
            {
                SpawnObstacleAccumulator -= SpawnObstacleRate;

                //Calculate spawn position
                float spawnAngle = (float)(Globals.rnd.NextDouble()) * MathF.Tau;
                Vector2 spawnPos = new Vector2(Globals.Camera.Width, Globals.Camera.Height) / 2f + spawnAngle.ToVector2() * ObstacleSpawnDist;

                //Calculate heading
                float headingAngle = MathF.Atan2(Globals.Camera.Height / 2f - spawnPos.Y, Globals.Camera.Width / 2f - spawnPos.X);
                headingAngle += ((float)Globals.rnd.NextDouble() - 0.5f) * MathF.PI;
                Vector2 heading = headingAngle.ToVector2();

                Obstacle obstacle = new(spawnPos, heading);
                batch.Add(obstacle);
            }

            //Spawn food
            if (food is null || food.exists == false)
            {
                food = new Food(generateRandomPosInRect(new Rectangle(
                    FoodSpawnPadding, FoodSpawnPadding, Globals.Camera.Width - FoodSpawnPadding * 2, Globals.Camera.Height - FoodSpawnPadding * 2
                )));
                batch.Add(food);
            }
        }

        private Vector2 generateRandomPosInRect(Rectangle rectangle)
        {
            int x = Globals.rnd.Next(rectangle.Left, rectangle.Right);
            int y = Globals.rnd.Next(rectangle.Top, rectangle.Bottom);
            return new Vector2(x, y);
        }
    }
}
