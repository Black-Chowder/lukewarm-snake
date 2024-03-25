using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using static BlackMagic.Globals;
using Microsoft.Xna.Framework;

namespace lukewarm_snake
{
    public class StartSnake : Entity
    {
        TailHandler tail;
        SnakeRenderer renderer;
        SnakeRippleHandler rippleHandler;
        FoodEater foodEater;
        AISnakeController AIController;
        public StartSnake(int startSnakeLength = TailHandler.minAnchors) : base(new Vector2(Globals.Camera.Width, Globals.Camera.Height) / 2f)
        {
            tail = new(this, startAnchors: startSnakeLength);
            AddTrait(tail);

            renderer = new(this, tail);
            AddTrait(renderer);

            rippleHandler = new(this);
            AddTrait(rippleHandler);

            foodEater = new(this);
            AddTrait(foodEater);

            AIController = new(this);
            AddTrait(AIController);
        }
    }
}
