using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using Microsoft.Xna.Framework;

namespace lukewarm_snake
{
    public class Food : Entity
    {
        public bool IsActive { get; set; } = false;

        FoodRenderer foodRenderer;
        FoodMovement movement;
        FoodRippleHandler rippleHandler;
        public Food() : base(Vector2.Zero)
        {
            foodRenderer = new FoodRenderer(this);
            AddTrait(foodRenderer);

            movement = new(this);
            AddTrait(movement);

            rippleHandler = new(this);
            AddTrait(rippleHandler);

            IsActive = false;
        }

        public void Init(Vector2 pos)
        {
            IsActive = true;
            Pos = prevPos = pos;
            foodRenderer.Depth = 0f;
        }
    }
}
