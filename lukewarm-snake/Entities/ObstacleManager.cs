using BlackMagic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BlackMagic.Globals;

namespace lukewarm_snake
{
    public class ObstacleManager : Entity
    {
        //Obstacle storage variables
        public Obstacle[] Obstacles { get; private set; }
        public const int MaxObstacles = 25;
        private int curObstacleIndex = 0;

        //Obstacle spawning variables
        public float SpawnObstacleRate = 400f;
        public float SpawnObstacleAccumulator = 0f;

        public float ObstacleSpeed = 1f;

        public static float ObstacleSpawnDist { get; private set; } = 0f;

        //Food spawning variables
        Food food;
        public const int FoodSpawnPadding = 50;

        public ObstacleManager() : base(Vector2.Zero)
        {
            //Initialize obstacle batch
            Obstacles = new Obstacle[MaxObstacles];
            for (int i = 0; i < Obstacles.Length; i++)
                Obstacles[i] = new Obstacle();

            if (ObstacleSpawnDist == 0f)
                ObstacleSpawnDist = DistanceUtils.GetCircleRadiusForRectangle(new Rectangle(0, 0, Globals.Camera.Width, Globals.Camera.Height));
        }

        public override void Update()
        {
            UpdateObstacles();
            SpawnManager();
        }

        public override void FixedUpdate()
        {
            for (int i = 0; i < Obstacles.Length; i++)
                if (Obstacles[i].IsActive) Obstacles[i].FixedUpdate();
        }

        public override void Draw()
        {
            for (int i = 0; i < Obstacles.Length; i++)
                if (Obstacles[i].IsActive) 
                    Obstacles[i].Draw();
        }

        public override void DrawRippleInfluence()
        {
            for (int i = 0; i < Obstacles.Length; i++)
                if (Obstacles[i].IsActive) Obstacles[i].DrawRippleInfluence();
        }


        private void SpawnManager()
        {
            //Spawn ostacles
            SpawnObstacleAccumulator += MathF.Max(Globals.MinTimeMod, Globals.TimeMod);
            for (; SpawnObstacleAccumulator > SpawnObstacleRate; SpawnObstacleAccumulator -= SpawnObstacleRate)
            {
                //Calculate spawn position
                float spawnRadius = DistanceUtils.GetCircleRadiusForRectangle(new Rectangle(0, 0, Globals.Camera.Width, Globals.Camera.Height));
                float spawnAngle = (float)(rnd.NextDouble()) * MathF.Tau;
                Vector2 spawnPos = new Vector2(MathF.Cos(spawnAngle), MathF.Sin(spawnAngle)) * spawnRadius;

                //Calculate heading
                float angleToCenter = (-spawnPos).Atan2();
                float upperAngleBound = (angleToCenter + MathF.PI / 4f);
                float lowerAngleBound = (angleToCenter - MathF.PI / 4f);
                float headingAngle = MathHelper.Lerp(upperAngleBound, lowerAngleBound, (float)rnd.NextDouble());
                Vector2 heading = headingAngle.ToVector2();

                //Initialize new obstacle
                Obstacle curObstacle = Obstacles[curObstacleIndex];
                curObstacle.Init(spawnPos + new Vector2(Globals.Camera.Width, Globals.Camera.Height) / 2f, heading);

                //Update curObstacleIndex
                curObstacleIndex++;
                if (curObstacleIndex >= Obstacles.Length)
                    curObstacleIndex = 0;
            }

            //Spawn food
            if (food is null || food.exists == false)
            {
                food = new Food(GenerateRandomPosInRect(new Rectangle(
                    FoodSpawnPadding, FoodSpawnPadding, Globals.Camera.Width - FoodSpawnPadding * 2, Globals.Camera.Height - FoodSpawnPadding * 2
                )));
                batch.Add(food);
            }
        }

        private Vector2 GenerateRandomPosInRect(Rectangle rectangle)
        {
            int x = rnd.Next(rectangle.Left, rectangle.Right);
            int y = rnd.Next(rectangle.Top, rectangle.Bottom);
            return new Vector2(x, y);
        }

        private void UpdateObstacles()
        {
            for (int i = 0; i < Obstacles.Length; i++)
            {
                if (!Obstacles[i].IsActive)
                    continue;

                Obstacles[i].Update();
            }
        }
    }
}
