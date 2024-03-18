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
        FoodRenderer foodRenderer;
        FoodMovement movement;
        public Food(Vector2 pos) : base(pos)
        {
            foodRenderer = new FoodRenderer(this);
            AddTrait(foodRenderer);

            movement = new(this);
            AddTrait(movement);
        }
    }
}
