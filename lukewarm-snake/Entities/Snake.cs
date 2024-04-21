using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using Microsoft.Xna.Framework;

namespace lukewarm_snake
{
    public class Snake : Entity
    {
        SnakeController snakeController;
        TailHandler tailHandler;
        SnakeRenderer tailRenderer;
        SnakeRippleHandler rippleHandler;
        FoodEater snakeCollider;
        SnakeHealth health;
        public Snake(int startSnakeLength = TailHandler.minAnchors) : base(new Vector2(Globals.Camera.Width, Globals.Camera.Height) / 2f)
        {
            health = new(this);
            AddTrait(health);

            tailHandler = new(this, startAnchors: startSnakeLength);
            AddTrait(tailHandler);

            tailRenderer = new(this, tailHandler, health);
            AddTrait(tailRenderer);

            rippleHandler = new(this);
            AddTrait(rippleHandler);

            snakeController = new(this, tailHandler);
            AddTrait(snakeController);

            snakeCollider = new(this);
            AddTrait(snakeCollider);

        }
    }
}
