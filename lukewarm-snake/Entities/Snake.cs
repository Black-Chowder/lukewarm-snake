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
        TailHandler tailHandler;
        public Snake() : base(Vector2.Zero)
        {
            tailHandler = new(this);
            AddTrait(tailHandler);
        }
    }
}
