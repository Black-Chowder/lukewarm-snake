using BlackMagic;
using static BlackMagic.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Kryz.Tweening;

namespace lukewarm_snake
{
    public class Title : Entity
    {
        public const string STitle = "SUPERHOT SNAKE";

        private float timer = 0f;
        private static Vector2 StartPos => new(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, -50f);
        private static Vector2 EndPos => new(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, 10f);

        private readonly float[] letterOffsets = new float[STitle.Length];
        private const float LetterOffsetVariation = 0.5f;

        public Title() : base(Vector2.Zero)
        {
            for (int i = 0; i < letterOffsets.Length; i++)
                letterOffsets[i] = (float)rnd.NextDouble() * LetterOffsetVariation;
        }

        public override void DrawRaw()
        {
            if (timer - LetterOffsetVariation < 1) timer += 0.01f;

            float stringWidthProgress = 0f;
            float fullStringWidth = defaultFont.MeasureString(STitle).X;
            for (int i = 0; i < STitle.Length; i++)
            {
                Vector2 curDrawPos = Vector2.Lerp(StartPos, EndPos, EasingFunctions.OutBack(MathHelper.Clamp(timer - letterOffsets[i], 0, 1)));

                spriteBatch.DrawString(defaultFont,
                    STitle[i].ToString(),
                    curDrawPos + new Vector2(stringWidthProgress - fullStringWidth / 2f, 0),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One,
                    SpriteEffects.None,
                    0f);

                //Could make this more efficient by caching the width of the characters
                stringWidthProgress += defaultFont.MeasureString(STitle[i].ToString()).X;
            }
        }
    }
}
