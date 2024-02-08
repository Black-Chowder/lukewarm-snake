using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace lukewarm_snake
{
    public class SnakeController : TUpdates
    {
        private Entity parent;

        public int Priority => Trait.defaultPriority;

        public SnakeController(Entity parent)
        {
            this.parent = parent;
        }

        public void Update()
        {
            MouseState mouse = Mouse.GetState();
            parent.Pos = mouse.Position.ToVector2();
        }
    }
}
