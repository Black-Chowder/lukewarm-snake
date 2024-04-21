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
        private float targetAngle = 0f;
        public float Angle { get; private set; }

        //Timing variables
        private float timer = 0f;
        private bool isMoving = true;

        private const float newMoveTimer = 300f,//Frequency at which food changes directions
            newMoveTimerVariation = 50f;

        private const float waitTimer = 200f,
            waitTimerVariation = 200f;


        int TUpdates.Priority => Trait.defaultPriority;

        public FoodMovement(Entity parent)
        {
            this.parent = parent;

            NewHeading();
            Angle = targetAngle;
        }

        public void Update()
        {
            if (!(parent as Food).IsActive)
                return;

            //Move parent
            heading *= 0.99f;
            parent.DeltaPos = heading * MathF.Max(MinTimeMod, TimeMod);

            Angle = MathHelper.Lerp(Angle, targetAngle, 0.1f);

            //Update parent heading
            timer -= MathF.Max(MinTimeMod, TimeMod);
            if (timer <= 0f)
            {
                if (!isMoving)
                {
                    NewHeading();
                    timer = newMoveTimer + newMoveTimerVariation * ((float)rnd.NextDouble() - 0.5f) * 2f;
                }
                else
                {
                    heading = Vector2.Zero;
                    timer = waitTimer + waitTimerVariation * ((float)rnd.NextDouble() - 0.5f) * 2f;
                }

                isMoving = !isMoving;
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

            targetAngle = headingAngle;
        }
    }
}
