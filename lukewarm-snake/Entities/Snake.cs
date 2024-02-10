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
        TailRenderer tailRenderer;
        FoodEater snakeCollider;
        public Snake() : base(new Vector2(Globals.Camera.Width, Globals.Camera.Height) / 2f)
        {

            tailHandler = new(this);
            AddTrait(tailHandler);

            tailRenderer = new(this, tailHandler);
            AddTrait(tailRenderer);

            snakeController = new(this, tailHandler);
            AddTrait(snakeController);

            snakeCollider = new(this);
            AddTrait(snakeCollider);
        }
    }
}
