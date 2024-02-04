using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using Microsoft.Xna.Framework;

namespace ProjectAstrid
{
    public class TAnimatorFSM : Trait
    {
        public new const byte priority = TAnimator.priority - 1;
        public AnimatorFSM animatorFSM { get; set; }

        public TAnimatorFSM(Entity parent, AnimatorFSM animatorFSM) : base(parent, priority)
        {
            this.animatorFSM = animatorFSM;
        }

        public override void Update(GameTime gameTime)
        {
            animatorFSM.Update();
        }
    }
}
