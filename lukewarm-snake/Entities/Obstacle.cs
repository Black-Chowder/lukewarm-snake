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
        ObstacleRenderer renderer;
        public Obstacle(Vector2 pos, float angle) : base(pos)
        {
            renderer = new(this);
            AddTrait(renderer);
        }
    }
}
