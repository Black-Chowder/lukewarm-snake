using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using Microsoft.Xna.Framework;

namespace lukewarm_snake
{
    public class Obstacle : Entity
    {
        ObstacleMovement movement;
        ObstacleRenderer renderer;
        ObstacleRippleHandler rippleHandler;
        public Obstacle(Vector2 pos, Vector2 heading) : base(pos)
        {
            movement = new(this, heading);
            AddTrait(movement);

            renderer = new(this);
            AddTrait(renderer);

            rippleHandler = new(this);
            AddTrait(rippleHandler);
        }
    }
}
