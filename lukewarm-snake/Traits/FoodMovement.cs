using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using static BlackMagic.Globals;
using Microsoft.Xna.Framework;

namespace lukewarm_snake
{
    public class FoodMovement : TUpdates
    {
        private Entity parent;

        //Speed variables
        private const float speed = 2f,
            speedVariance = 1f;
        private Vector2 heading;

        //Timing variables
        private float timer = 0f;
        private const float newMoveTimer = 200f,//Frequency at which food changes directions
            newMoveTimerVariation = 100f;

        int TUpdates.Priority => Trait.defaultPriority;

        public FoodMovement(Entity parent)
        {
            this.parent = parent;

            NewHeading();
        }

        public void Update()
        {
            //Move parent
            heading *= 0.99f;
            parent.DeltaPos = heading * MathF.Max(MinTimeMod, TimeMod);

            //Update parent heading
            timer -= MathF.Max(MinTimeMod, TimeMod);
            if (timer <= 0f)
            {
                NewHeading();
                timer = newMoveTimer + newMoveTimerVariation * (float)rnd.NextDouble() - 0.5f * 2f;
            }
        }


        private void NewHeading()
        {
            //Calculate heading angle
            float angleToCenter = (new Vector2(Globals.Camera.Width, Globals.Camera.Height) / 2f - parent.Pos).Atan2();
            float upperAngleBound = angleToCenter + MathF.PI / 2f;
            float lowerAngleBound = angleToCenter - MathF.PI / 2f;
            float headingAngle = MathHelper.Lerp(upperAngleBound, lowerAngleBound, (float)rnd.NextDouble());

            //Calculate speed
            float newSpeed = speed + speedVariance * (float)rnd.NextDouble() - 0.5f * 2f;

            //Apply to heading
            heading = headingAngle.ToVector2() * newSpeed;
        }
    }
}
