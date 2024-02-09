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
        SnakeCollider snakeCollider;
        public Snake() : base(Vector2.Zero)
        {
            snakeController = new(this);
            AddTrait(snakeController);

            tailHandler = new(this);
            AddTrait(tailHandler);

            tailRenderer = new(this, tailHandler);
            AddTrait(tailRenderer);

            snakeCollider = new(this);
            AddTrait(snakeCollider);
        }
    }
}
