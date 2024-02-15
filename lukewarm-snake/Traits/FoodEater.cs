using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using Microsoft.Xna.Framework;

namespace lukewarm_snake
{
    public class FoodEater : TUpdates
    {
        private Entity parent;

        //Eat food variables
        public int FoodValue { get; set; } = 3;

        public int Priority => Trait.defaultPriority;

        public FoodEater(Entity parent)
        {
            this.parent = parent;
        }

        public void Update()
        {
            //Eat food
            List<Entity> foodEntityList = parent.batch.GetEntityBucket<Food>();
            for (int i = 0; i < foodEntityList.Count; i++)
            {
                Food food = (Food)foodEntityList[i];

                if (CollisionUtils.IsCirclesColliding(food.Pos, FoodRenderer.FoodRadius, parent.Pos, SnakeRenderer.BodyRadius))
                {
                    food.exists = false;
                    Debug.WriteLine($"Score: {parent.GetTrait<TailHandler>().Anchors.Count}");
                    parent.GetTrait<TailHandler>().MaxAnchors += FoodValue;
                }
            }
        }
    }
}
