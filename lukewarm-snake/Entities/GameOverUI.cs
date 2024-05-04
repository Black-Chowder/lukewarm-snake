using BlackMagic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BlackMagic.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Kryz.Tweening;

namespace lukewarm_snake
{
    public class GameOverUI : Entity
    {
        public const string STitle = "GAME OVER";
        public const string SScoreBase = "SCORE: ";

        private float timer = 0f;

        //Title Position Variables
        private static Vector2 TitleStartPos => new(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, -50f);
        private static Vector2 TitleEndPos => new(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, 10f);
        private readonly float[] titleLetterOffsets = new float[STitle.Length];
        private const float TitleLetterOffsetVariation = 0.5f;


        public GameOverUI() : base(Vector2.Zero)
        {
            //Initialize title
            for (int i = 0; i < titleLetterOffsets.Length; i++)
                titleLetterOffsets[i] = (float)rnd.NextDouble() * TitleLetterOffsetVariation;

        }

        public override void Update()
        {
            timer += 0.01f;
        }

        public override void Draw()
        {

        }

        //Draw call with UI shader effect applied
        public override void DrawRaw()
        {
            //Draw title
            float stringWidthProgress = 0f;
            float fullStringWidth = defaultFont.MeasureString(STitle).X;
            for (int i = 0; i < STitle.Length; i++)
            {
                Vector2 curDrawPos = Vector2.Lerp(TitleStartPos, TitleEndPos, EasingFunctions.OutBack(MathHelper.Clamp(timer - titleLetterOffsets[i], 0, 1)));

                spriteBatch.DrawString(defaultFont,
                    STitle[i].ToString(),
                    curDrawPos + new Vector2(stringWidthProgress - fullStringWidth / 2f, 0),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One,
                    SpriteEffects.None,
                    0f);

                stringWidthProgress += defaultFont.MeasureString(STitle[i].ToString()).X;
            }
        }
    }
}
