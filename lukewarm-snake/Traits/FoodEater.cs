using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using static BlackMagic.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace lukewarm_snake
{
    public class FoodEater : TUpdates
    {
        private Entity parent;

        //Eat food variables
        public int FoodValue { get; set; } = 3;
        public int FoodEaten { get; set; } = 0;

        //Sfx variables
        SoundEffect eatSfx;
        SoundEffectInstance eatSfxInstance;
        readonly bool sfxOn;

        public int Priority => Trait.defaultPriority;

        public FoodEater(Entity parent, bool sfxOn = true)
        {
            this.parent = parent;
            this.sfxOn = sfxOn;

            eatSfx = content.Load<SoundEffect>(@"SFX/Short Rising Flutter");//content.Load<SoundEffect>(@"SFX/PLOP Mouth Drip Dry");
            eatSfxInstance = eatSfx.CreateInstance();
        }

        public void Update()
        {
            //Eat food
            FoodManager foodManager = parent.batch.GetEntityBucket<FoodManager>()?.First() as FoodManager;

            if (foodManager is null)
                return;

            Food food = foodManager.Food;
            if (!food.IsActive)
                return;

            if (CollisionUtils.IsCirclesColliding(food.Pos, FoodRenderer.FoodRadius, parent.Pos, SnakeRenderer.BodyRadius))
            {
                food.IsActive = false;
                Debug.WriteLine($"Score: {parent.GetTrait<TailHandler>().Anchors.Count}");
                parent.GetTrait<TailHandler>().MaxAnchors += FoodValue;
                parent.GetTrait<SnakeRenderer>().EatenFood();
                FoodEaten++;

                if (sfxOn) eatSfxInstance.Play();
            }
        }
    }
}
