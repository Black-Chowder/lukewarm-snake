using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using static BlackMagic.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace lukewarm_snake
{
    public class SnakeHealth : TUpdates
    {
        public Entity parent;

        public const float HitboxBuffer = 10;
        public const float HitboxRadius = SnakeRenderer.BodyRadius - HitboxBuffer;

        public bool IsAlive { get; private set; } = true;
        public delegate void DeathDelegate();
        public event DeathDelegate deathEvent;

        public int Priority => Trait.defaultPriority;

        SoundEffect deathSfx;
        SoundEffectInstance deathSfxInstance;

        public SnakeHealth(Entity parent)
        {
            this.parent = parent;

            deathSfx = content.Load<SoundEffect>(@"SFX/GS projectile splash 004");
            deathSfxInstance = deathSfx.CreateInstance();
        }

        public void Update()
        {
            LinkedList<Vector2> anchors = parent.GetTrait<TailHandler>().Anchors;

            if (parent.batch.GetEntityBucket<ObstacleManager>()?.First() is not ObstacleManager obstacleManager)
                return;

            Obstacle[] obstacles = obstacleManager.Obstacles;

            bool wantToBreak = false;
            int anchorIndex = 0;
            for (var curAnchor = anchors.First; curAnchor != null; curAnchor = curAnchor.Next, anchorIndex++)
            {
                //Calculate particular anchor hitbox radius
                int tailEndProgressIndex = anchors.Count - anchorIndex;
                float anchorHitboxRadius = SnakeRenderer.BodyRadius;
                if (tailEndProgressIndex <= SnakeRenderer.ShrinkBeginIndex)
                    anchorHitboxRadius = MathHelper.Lerp(SnakeRenderer.TailEndRadius, SnakeRenderer.BodyRadius, (float)tailEndProgressIndex / (float)SnakeRenderer.ShrinkBeginIndex);
                anchorHitboxRadius -= HitboxBuffer;

                //Determine anchor collision with obstacles
                for (int i = 0; i < obstacles.Length; i++)
                {
                    Obstacle obstacle = obstacles[i];
                    if (!obstacle.IsActive)
                        continue;

                    //If is colliding
                    if (CollisionUtils.IsCirclesColliding(curAnchor.Value, anchorHitboxRadius, obstacle.Pos, ObstacleRenderer.ObstacleRadius))
                    {
                        //Snake is hit!
                        IsAlive = false;
                        deathEvent?.Invoke();

                        //Delete obstacles
                        obstacle.IsActive = false;
                        for (int j = 0; j < obstacles.Length; j++)
                        {
                            obstacles[j].IsActive = false;
                        }
                        obstacleManager.IsSpawningObstacles = false;

                        FoodManager foodManager = MainEntityBatch.GetEntityBucket<FoodManager>()[0] as FoodManager;
                        foodManager.IsSpawningFood = false;
                        foodManager.Food.IsActive = false;

                        //parent.GetTrait<TailHandler>().MaxAnchors = (int)MathF.Max(1, anchorIndex);

                        //Sound effect
                        deathSfxInstance.Play();

                        wantToBreak = true;
                        break;
                    }
                }

                if (wantToBreak) break;
            }
        }
    }
}
