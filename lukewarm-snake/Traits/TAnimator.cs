using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.Design;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BlackMagic
{
    public class TAnimator : Trait, TDraws
    {
        public new const byte priority = 100;
        public Animator animator { get; set; }

        public delegate Vector2 DrawPosFunc();
        private DrawPosFunc drawPosFunc;

        public TAnimator(Entity parent, Animator animator, DrawPosFunc drawPosFunc = null) : base(parent, priority)
        {
            this.animator = animator;

            this.drawPosFunc = drawPosFunc ?? (() => (parent.DrawPos - Globals.Camera.Pos) * Globals.Camera.Zoom);
        }

        public override void Update(GameTime gameTime)
        {
            animator.cameraZoom = Globals.Camera.Zoom;
            animator.Update(gameTime);
        }

        public void Draw() { Draw(Globals.spriteBatch, null); }
        public void Draw(Vector2 pos) { Draw(Globals.spriteBatch, pos); }
        public void Draw(SpriteBatch spriteBatch, Vector2? pos)
        {
            if (!parent.isVisible) return;
            if (!pos.HasValue)
                pos = drawPosFunc();
            animator.Draw(spriteBatch, pos.Value.ToPoint().ToVector2());
        }
    }
}
