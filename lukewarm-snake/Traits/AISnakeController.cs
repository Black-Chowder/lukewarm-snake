using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using static BlackMagic.Globals;

namespace lukewarm_snake
{
    public class AISnakeController : TUpdates
    {
        private Entity parent;

        public const float Speed = 12f;
        public const float MaxTurnDist = MathF.PI / 32f;
        public float HeadingAngle { get; set; } = 0f;

        public int Priority => Trait.defaultPriority;

        public AISnakeController(Entity parent)
        {
            this.parent = parent;
            HeadingAngle = parent.GetTrait<SnakeRenderer>().HeadAngle;
        }

        public void Update()
        {
            TimeMod = EntityBatch.TimeStep;

            //Calculate heading angle:
            if (MainEntityBatch.GetEntityBucket<FoodManager>()?.First() is not FoodManager foodManager)
                return;
            Food food = foodManager.Food;
            float angleToFood = (parent.Pos - food.Pos).Atan2();

            float headAngle = parent.GetTrait<SnakeRenderer>().HeadAngle;

            HeadingAngle += MathF.Min(MathF.Abs(headAngle - angleToFood), MaxTurnDist) * 
                (angleToFood > headAngle ? 1 : -1);

            parent.DeltaPos = HeadingAngle.ToVector2() * Speed;
        }
    }
}
