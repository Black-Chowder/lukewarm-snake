using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackMagic
{
    public interface Hitbox
    {
        public float GetDist(Vector2 point);
    }
}
