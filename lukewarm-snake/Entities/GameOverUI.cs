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
using Microsoft.Xna.Framework.Input;

namespace lukewarm_snake
{
    public class GameOverUI : Entity
    {
        public const string STitle = "GAME OVER";
        public const string SScoreBase = "SCORE: ";
        public const string SRestart = "RESTART";
        public const string SHome = "HOME";

        private float timer = 0f;

        //Title Position Variables
        private static Vector2 TitleStartPos => new(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, -50f);
        private static Vector2 TitleEndPos => new(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, 10f);
        private readonly float[] titleLetterOffsets = new float[STitle.Length];
        private const float TitleLetterOffsetVariation = 0.5f;

        //Score Position Variables
        private static Vector2 ScoreStartPos => new (Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, -50f);
        private static Vector2 ScoreEndPos => new(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, 30f);
        private readonly float[] scoreLetterOffsets;
        private const float ScoreLetterOffsetVariation = 0.5f;
        private string scoreAsString;
        private string SScore;
        private const float SizeChangeTime = 1f;

        //Restart Button Position Variables
        private static Vector2 RestartStartPos => new Vector2(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, -50f);
        private static Vector2 RestartEndPos => new Vector2(Globals.Camera.Width, Globals.Camera.Height + 150f) * EntityBatch.PixelateMultiplier / 2f;
        private readonly float[] restartLetterOffsets = new float[SRestart.Length];
        private const float RestartLetterOffsetVariation = 0.5f;

        private bool isHoveringOverRestart = false;
        private Rectangle restartHitbox;
        private static Vector2 RestartHitboxBuffer => new(20, 5);


        public GameOverUI() : base(Vector2.Zero)
        {
            //Initialize title
            for (int i = 0; i < titleLetterOffsets.Length; i++)
                titleLetterOffsets[i] = (float)rnd.NextDouble() * TitleLetterOffsetVariation;

            //Initialize score
            scoreAsString = PlayerScore.ToString();
            SScore = SScoreBase + scoreAsString;
            scoreLetterOffsets = new float[SScore.Length];
            for (int i = 0; i < scoreLetterOffsets.Length; i++)
                scoreLetterOffsets[i] = (float)rnd.NextDouble() * ScoreLetterOffsetVariation;

            //Initialize restart button
            for (int i = 0; i < restartLetterOffsets.Length; i++)
                restartLetterOffsets[i] = (float)rnd.NextDouble() * RestartLetterOffsetVariation;

            Vector2 restartSize = defaultFont.MeasureString(SRestart) + RestartHitboxBuffer;
            restartHitbox = new Rectangle(
                (int)((Globals.Camera.Width * EntityBatch.PixelateMultiplier - restartSize.X) / 2f),
                (int)(((Globals.Camera.Height - 6) * EntityBatch.PixelateMultiplier + restartSize.Y / 2f) / 2f),
                (int)restartSize.X, (int)restartSize.Y
            );
        }

        public override void Update()
        {
            timer += 0.01f;

            MouseState mouse = Mouse.GetState();

            isHoveringOverRestart = restartHitbox.Contains(mouse.Position.ToVector2() * EntityBatch.PixelateMultiplier);

            if (mouse.LeftButton == ButtonState.Pressed && isHoveringOverRestart)
                GameState = GameStates.StartGame;
        }

        public override void Draw()
        {
            if (isHoveringOverRestart)
            {
                spriteBatch.Draw(DrawUtils.createTexture(spriteBatch.GraphicsDevice),
                    restartHitbox,
                    Color.Black * 0.25f);
            }
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

            //Draw score
            stringWidthProgress = 0f;
            fullStringWidth = defaultFont.MeasureString(SScoreBase + scoreAsString).X;
            for (int i = 0; i < SScoreBase.Length; i++)
            {
                Vector2 curDrawPos = Vector2.Lerp(ScoreStartPos, ScoreEndPos, EasingFunctions.OutBack(MathHelper.Clamp(timer - scoreLetterOffsets[i], 0, 1)));

                spriteBatch.DrawString(defaultFont,
                    SScore[i].ToString(),
                    curDrawPos + new Vector2(stringWidthProgress - fullStringWidth / 2f, 0),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One,
                    SpriteEffects.None,
                    0f);

                stringWidthProgress += defaultFont.MeasureString(SScore[i].ToString()).X;
            }

            //Draw score number
            for (int i = SScoreBase.Length; i < SScore.Length; i++)
            {
                Vector2 curDrawPos = Vector2.Lerp(ScoreStartPos, ScoreEndPos, EasingFunctions.OutBack(MathHelper.Clamp(timer - scoreLetterOffsets[i], 0, 1)));

                spriteBatch.DrawString(defaultFont,
                    SScore[i].ToString(),
                    curDrawPos + new Vector2(stringWidthProgress - fullStringWidth / 2f, 0),
                    Color.White,
                    0f,
                    Vector2.One ,
                    Vector2.One * 1.001f,
                    SpriteEffects.None,
                    0f);

                stringWidthProgress += defaultFont.MeasureString(SScore[i].ToString()).X;
            }


            //Draw restart button
            stringWidthProgress = 0f;
            fullStringWidth = defaultFont.MeasureString(SRestart).X;
            for (int i = 0; i < SRestart.Length; i++)
            {
                Vector2 curDrawPos = Vector2.Lerp(RestartStartPos, RestartEndPos, EasingFunctions.OutBack(MathHelper.Clamp(timer - restartLetterOffsets[i], 0, 1)));

                spriteBatch.DrawString(defaultFont,
                    SRestart[i].ToString(),
                    curDrawPos + new Vector2(stringWidthProgress - fullStringWidth / 2f, 0),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One,
                    SpriteEffects.None,
                    0f);

                stringWidthProgress += defaultFont.MeasureString(SRestart[i].ToString()).X;
            }
        }
    }
}
