using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using static BlackMagic.Globals;

namespace lukewarm_snake
{
    public class AISnakeController : TUpdates
    {
        private Entity parent;

        public int Priority => Trait.defaultPriority;

        public AISnakeController(Entity parent)
        {
            this.parent = parent;
        }

        public void Update()
        {

        }
    }
}
