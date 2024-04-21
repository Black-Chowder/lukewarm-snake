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
        public bool IsSpawningObstacles { get; set; } = true;
        public float SpawnObstacleRate = 400f;
        public float SpawnObstacleAccumulator = 0f;

        public float ObstacleSpeed = 1f;

        public static float ObstacleSpawnDist { get; private set; } = 0f;

        //Obstacle drawing variables
        private RenderTarget2D obstacleRt;
        private RenderTarget2D borderRt;
        private static Effect borderEffect;

        public ObstacleManager() : base(Vector2.Zero)
        {
            //Initialize obstacle batch
            Obstacles = new Obstacle[MaxObstacles];
            for (int i = 0; i < Obstacles.Length; i++)
                Obstacles[i] = new Obstacle();

            if (ObstacleSpawnDist == 0f)
                ObstacleSpawnDist = DistanceUtils.GetCircleRadiusForRectangle(new Rectangle(0, 0, Globals.Camera.Width, Globals.Camera.Height));

            //Set up border effect stuff
            borderEffect ??= content.Load<Effect>(@"Effects/Border");
            borderRt = new RenderTarget2D(spriteBatch.GraphicsDevice, MainEntityBatch.rt.Width, MainEntityBatch.rt.Height);
            obstacleRt = new RenderTarget2D(spriteBatch.GraphicsDevice, MainEntityBatch.rt.Width, MainEntityBatch.rt.Height);
        }

        public override void Update()
        {
            UpdateObstacles();
            PrerenderObstacles();
            SpawnManager();
        }

        public override void FixedUpdate()
        {
            for (int i = 0; i < Obstacles.Length; i++)
                if (Obstacles[i].IsActive) Obstacles[i].FixedUpdate();
        }

        public override void Draw() =>
            spriteBatch.Draw(borderRt, Vector2.Zero, Color.White);

        public override void DrawRippleInfluence()
        {
            for (int i = 0; i < Obstacles.Length; i++)
                if (Obstacles[i].IsActive) Obstacles[i].DrawRippleInfluence();
        }


        private void SpawnManager()
        {
            if (!IsSpawningObstacles)
                return;

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

        private void PrerenderObstacles()
        {
            //Draw all sprites to obstacle render target
            spriteBatch.GraphicsDevice.SetRenderTarget(obstacleRt);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            for (int i = 0; i < Obstacles.Length; i++)
            {
                if (!Obstacles[i].IsActive)
                    continue;

                Obstacles[i].GetTrait<ObstacleRenderer>().DrawObstacle();
            }

            spriteBatch.End();

            //Draw all sprites with border effect applied
            borderEffect.Parameters["OutlineColor"].SetValue(new Vector4(0, 0, 0, 1));
            borderEffect.Parameters["texelSize"].SetValue(new Vector2(1f / (obstacleRt.Width - 1f), 1f / (obstacleRt.Height - 1f)));

            spriteBatch.GraphicsDevice.SetRenderTarget(borderRt);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: borderEffect);
            spriteBatch.Draw(obstacleRt, Vector2.Zero, Color.White);
            spriteBatch.End();
        }
    }
}
