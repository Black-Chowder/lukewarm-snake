using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using static BlackMagic.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Kryz.Tweening;
using Microsoft.Xna.Framework.Graphics;

namespace lukewarm_snake
{
    public class Scoreboard : Entity
    {
        public const string STitle = "TOP SCORES";

        private float timer = 0f;
        private const float TimerStep = 0.01f;

        //Title position variables
        private static Vector2 TitleStartPos => new(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, -50f);
        private static Vector2 TitleEndPos => new(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, 10f);
        private readonly float[] titleLetterOffsets = new float[STitle.Length];
        private const float TitleLetterOffsetVariation = 0.5f;

        //Scores position variables
        private static Vector2 ScoreStartPos => new(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, -50f);
        private static Vector2 ScoreEndPos => new(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, 30f);
        private const float ScoreSpacing = 17f;
        private const float TimerWaitOffset = 0.25f;


        public Scoreboard() : base(Vector2.Zero)
        {
            //Initialize title
            for (int i = 0; i < titleLetterOffsets.Length; i++)
                titleLetterOffsets[i] = (float)rnd.NextDouble() * TitleLetterOffsetVariation;
        }

        public override void Update()
        {
            timer += TimerStep;

            MouseState mouse = Mouse.GetState();

            //TODO Handle back button hover
        }

        public override void Draw()
        {
            //TODO: Handle back button
        }

        public override void DrawRaw()
        {
            //Draw title
            float stringWidthProgress = 0f;
            float fullStringWidth = defaultFont.MeasureString(STitle).X;
            for (int i = 0; i < STitle.Length; i++)
            {
                Vector2 curDrawPos = Vector2.Lerp(TitleStartPos, TitleEndPos, EasingFunctions.OutBack(MathHelper.Clamp(timer, 0, 1)));

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

            
            //Draw scores
            for (int i = 0; i < ScoreboardScores.Count; i++)
            {
                string curScoreStr = $"{ScoreboardScores[i].Item1}  {ScoreboardScores[i].Item2}";

                stringWidthProgress = 0f;
                fullStringWidth = defaultFont.MeasureString(curScoreStr).X;

                for (int j = 0; j < curScoreStr.Length; j++)
                {
                    Vector2 curDrawPos = Vector2.Lerp(
                        ScoreStartPos, 
                        ScoreEndPos + new Vector2(0, ScoreSpacing * i), 
                        EasingFunctions.OutBack(MathHelper.Clamp(timer - TimerWaitOffset * i, 0, 1))
                    );

                    spriteBatch.DrawString(defaultFont,
                        curScoreStr[j].ToString(),
                        curDrawPos + new Vector2(stringWidthProgress - fullStringWidth / 2f, 0),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        Vector2.One * (j > 3 ? 1.001f : 1f),
                        SpriteEffects.None,
                        0f);

                    stringWidthProgress += defaultFont.MeasureString(curScoreStr[j].ToString()).X;
                }
                
            }
        }
    }
}
