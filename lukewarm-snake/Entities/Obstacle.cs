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
        public bool IsActive { get; set; } = false;

        ObstacleMovement movement;
        ObstacleRenderer renderer;
        ObstacleRippleHandler rippleHandler;
        public Obstacle() : base(-new Vector2(Globals.Camera.Width, Globals.Camera.Height))
        {
            movement = new(this, Vector2.Zero);
            AddTrait(movement);

            renderer = new(this);
            AddTrait(renderer);

            rippleHandler = new(this);
            AddTrait(rippleHandler);

            IsActive = false;
        }

        public void Init(Vector2 pos, Vector2 heading)
        {
            IsActive = true;
            Pos = prevPos = pos;
            movement.Heading = heading;
        }
    }
}
