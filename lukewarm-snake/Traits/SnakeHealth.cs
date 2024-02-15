﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace lukewarm_snake
{
    public class SnakeHealth : TUpdates
    {
        public Entity parent;

        public const float HitboxBuffer = 10;
        public const float HitboxRadius = TailRenderer.BodyRadius - HitboxBuffer;

        public int Priority => Trait.defaultPriority;

        public SnakeHealth(Entity parent)
        {
            this.parent = parent;
        }

        public void Update()
        {
            TailRenderer tailRenderer = parent.GetTrait<TailRenderer>();
            LinkedList<Vector2> anchors = parent.GetTrait<TailHandler>().Anchors;
            List<Entity> obstacleEntities = parent.batch.GetEntityBucket<Obstacle>();

            bool wantToBreak = false;
            int anchorIndex = 0;
            for (var curAnchor = anchors.First; curAnchor != null; curAnchor = curAnchor.Next, anchorIndex++)
            {
                //Calculate particular anchor hitbox radius
                int tailEndProgressIndex = anchors.Count - anchorIndex;
                float anchorHitboxRadius = TailRenderer.BodyRadius;
                if (tailEndProgressIndex <= TailRenderer.ShrinkBeginIndex)
                    anchorHitboxRadius = MathHelper.Lerp(TailRenderer.TailEndRadius, TailRenderer.BodyRadius, (float)tailEndProgressIndex / (float)TailRenderer.ShrinkBeginIndex);
                anchorHitboxRadius -= HitboxBuffer;

                //Determine anchor collision with obstacles
                for (int i = 0; i < obstacleEntities.Count; i++)
                {
                    Obstacle obstacle = (Obstacle)obstacleEntities[i];

                    if (CollisionUtils.IsCirclesColliding(curAnchor.Value, anchorHitboxRadius, obstacle.Pos, ObstacleRenderer.ObstacleRadius))
                    {
                        parent.GetTrait<TailHandler>().MaxAnchors = (int)MathF.Max(1, anchorIndex);
                        obstacle.exists = false;
                        wantToBreak = true;
                        break;
                    }
                }

                if (wantToBreak) break;
            }
        }
    }
}
