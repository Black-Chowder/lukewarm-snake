using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using static BlackMagic.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace lukewarm_snake
{
    public class SnakeController : TUpdates
    {
        private Entity parent;

        SoundEffect moveSound;
        SoundEffectInstance soundInstance;

        public const float DistToMaxVolume = 20f;
        public const float VolumeUpStep = 0.01f;
        public const float VolumeDownStep = 0.01f;
        public const float MaxVolume = 1f;

        public int Priority => Trait.defaultPriority;

        public SnakeController(Entity parent, TailHandler tailHandler)
        {
            this.parent = parent;

            moveSound = content.Load<SoundEffect>(@"SFX/15_Calm sea_close shot_few people_few cars");
            soundInstance = moveSound.CreateInstance();
            soundInstance.Play();
            soundInstance.IsLooped = true;
            soundInstance.Volume = 0f;

            Mouse.SetPosition((int)parent.Pos.X, (int)parent.Pos.Y);
        }

        public void Update()
        {
            if (!parent.GetTrait<SnakeHealth>().IsAlive)
            {
                soundInstance.Volume = MathHelper.Clamp(soundInstance.Volume - VolumeDownStep, 0f, MaxVolume);
                return;
            }

            MouseState mouse = Mouse.GetState();

            //Handle sound
            Vector2 posDiff = parent.Pos - mouse.Position.ToVector2();
            float targetVolume = MathF.Max(soundInstance.Volume, MathF.Min(posDiff.Length(), 1f));

            float newVolume = soundInstance.Volume;
            if (soundInstance.Volume < targetVolume)
                newVolume += VolumeUpStep;
            else
                newVolume -= VolumeDownStep;
            newVolume = MathHelper.Clamp(newVolume, 0f, MaxVolume);
            soundInstance.Volume = newVolume;

            parent.Pos = mouse.Position.ToVector2();
        }
    }
}
