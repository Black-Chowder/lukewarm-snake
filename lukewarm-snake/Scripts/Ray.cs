using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BlackMagic
{
    public class Ray
    {
        public Vector2 pos;
        public float angle;

        private Vector2? tip;
        private Entity closestEntity = null;

        public const int defaultMaxSteps = 100;
        
        //Constructors
        public Ray(float x, float y, float angle = 0f)
        {
            pos = new Vector2(x, y);
            this.angle = angle;
        }

        public Ray(Vector2 pos, float angle = 0f)
        {
            this.pos = pos;
            this.angle = angle;
        }

        public Ray(Vector3 data)
        {
            pos = new Vector2(data.X, data.Y);
            angle = data.Z;
        }

        //Ray casting for just a list of rigidbodies
        public Vector2? cast(List<Rigidbody> rigidbodies, float maxDist = float.PositiveInfinity, int maxSteps = defaultMaxSteps)
        {
            Vector2 rayTip = new Vector2(pos.X, pos.Y);
            float rayDist = 0f;

            for (int i = 0; i < maxSteps; i++)
            {
                float shortestDist = float.PositiveInfinity;
                for (int j = 0; j < rigidbodies.Count; j++)
                {
                    if (!rigidbodies[j].isActive) continue;

                    float distance = rigidbodies[j].GetDist(rayTip);
                    if (distance < shortestDist)
                        shortestDist = distance;
                }

                rayDist += shortestDist;
                if (rayDist >= maxDist)
                    return null;

                if (shortestDist < 0.1f)
                {
                    tip = new Vector2(rayTip.X, rayTip.Y);
                    return tip;
                }

                Vector2 extension = new Vector2((float)(shortestDist * Math.Cos(angle)), (float)(shortestDist * Math.Sin(angle)));
                rayTip += extension;
            }
            tip = null;
            return null;
        }

        //Casts ray to be colliding with all entities in given list
        public Vector2? cast(Entity self, float maxDist = float.PositiveInfinity, int maxSteps = defaultMaxSteps)
        {
            Vector2 rayTip = new Vector2(pos.X, pos.Y);
            float rayDist = 0f;

            //Get Self Rigidbody
            Rigidbody selfRB = self.GetTrait<Rigidbody>();

            for (int i = 0; i < maxSteps; i++)
            {
                //Step 1: get shortest distance
                float shortestDist = float.PositiveInfinity;
                foreach (Entity entity in self.batch.entities)
                {
                    //Skips entity if any of the following criteria is met:
                    if (!entity.HasTrait<Rigidbody>() //Doesn't have rigidbody trait
                        || entity == self //Is itself
                        || selfRB.ignoreTypes.Contains(entity.GetType().BaseType) //Self is supposed to ignore entity base type
                        || selfRB.ignoreTypes.Contains(entity.GetType())) //Self is supposed to ignore entity type
                            continue;

                    //Gets entitie's rigidbody trait
                    Rigidbody rigidbody = entity.GetTrait<Rigidbody>();

                    if (!rigidbody.isActive) continue;

                    float distance = rigidbody.GetDist(rayTip);
                    if (distance < shortestDist)
                    {
                        shortestDist = distance;
                        closestEntity = entity;
                    }
                }

                //Check that ray distance is below max dist allowed to travel
                //Note: This may be a bad way of doing things because although it's faster, because of the inaccuracy of
                //      floating point decimals, the more itterations, the less accurate.
                rayDist += shortestDist;
                if (rayDist >= maxDist)
                    return null;
                
                //Step 2: Check if touching object (or really really close)
                if (shortestDist < .01)//ARBITRAIRILY SMALL NUMBER (in pixel units)
                {
                    tip = new Vector2(rayTip.X, rayTip.Y);
                    return tip;
                }

                //Step 3: Extend Point
                Vector2 extension = new Vector2((float)(shortestDist*Math.Cos(angle)), (float)(shortestDist*Math.Sin(angle)));
                rayTip += extension;
                
                //Repeat
            }
            tip = null;
            return null;
        }

        //Gets entity that the ray collides with (can be null, if not colliding with anything)
        public Entity getEntity()
        {
            return closestEntity;
        }

        public void drawRay(SpriteBatch spriteBatch)
        {
            Vector2 line = new Vector2((float)(10000*Math.Cos(angle)), (float)(10000*Math.Sin(angle)));
            DrawUtils.DrawLine(spriteBatch, pos, new Vector2(pos.X + line.X, pos.Y + line.Y), Color.White);
            if (tip != null)
            {
                DrawUtils.DrawLine(spriteBatch, pos, tip.Value, Color.White);
            }
        }

        public void drawPoint(SpriteBatch spriteBatch, Texture2D circle)
        {
            spriteBatch.Draw(circle, new Rectangle((int)(pos.X - 1), (int)(pos.Y - 1), 2, 2), Color.White);
        }

        public List<Vector3> debugDrawRay(List<Entity> entities, Entity self)
        {
            List<Vector3> toReturn = new List<Vector3>();
            Color debugColor = new Color(255, 255, 255, 100);
            Vector2 rayTip = new Vector2(pos.X, pos.Y);
            for (int i = 0; i < 100; i++)
            {
                //Step 1:
                float shortestDist = float.PositiveInfinity;
                foreach (Entity entity in entities)
                {
                    if (!entity.HasTrait<Rigidbody>() || entity == self) continue;
                    Rigidbody rigidbody = entity.GetTrait<Rigidbody>();
                    float dist = rigidbody.GetDist(rayTip);
                    
                    if (dist < shortestDist)
                    {
                        shortestDist = dist;
                    }

                }
                if (shortestDist < 1000)
                {
                    toReturn.Add(new Vector3(rayTip.X, rayTip.Y, shortestDist));
                }

                //Step 2:
                if (shortestDist < .0001)
                {
                    break;
                }

                //Step 3:
                Vector2 extension = new Vector2((float)(shortestDist * Math.Cos(angle)), (float)(shortestDist * Math.Sin(angle)));
                rayTip.X += extension.X;
                rayTip.Y += extension.Y;
            }

            return toReturn;
        }
    }
}
