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
using Microsoft.Xna.Framework.Input;

namespace lukewarm_snake
{
    public class Title : Entity
    {
        public const string STitle = "SUPERHOT SNAKE";
        public const string Start = "START";

        private float timer = 0f;

        //Title Position Variables
        private static Vector2 TitleStartPos => new(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, -50f);
        private static Vector2 TitleEndPos => new(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, 10f);
        private readonly float[] titleLetterOffsets = new float[STitle.Length];
        private const float TitleLetterOffsetVariation = 0.5f;

        //Start button position variables
        private static Vector2 StartStartPos => new Vector2(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, -50f);
        private static Vector2 StartEndPos => new Vector2(Globals.Camera.Width, Globals.Camera.Height) * EntityBatch.PixelateMultiplier / 2f;
        private readonly float[] startLetterOffsets = new float[Start.Length];
        private const float StartLetterOffsetVariation = 0.5f;
        private const float StartTimerWait = 0.25f;
        private const float BobTime = 1f;
        private const float BobDist = 8f;

        private bool isHoveringOverStart = false;
        private Rectangle startHitbox;
        private static Vector2 StartHitboxBuffer => new(20, 5);

        public Title() : base(Vector2.Zero)
        {
            for (int i = 0; i < titleLetterOffsets.Length; i++)
                titleLetterOffsets[i] = (float)rnd.NextDouble() * TitleLetterOffsetVariation;
            
            for (int i = 0; i < startLetterOffsets.Length; i++)
                startLetterOffsets[i] = ((float)i / startLetterOffsets.Length) * StartLetterOffsetVariation;


            Vector2 startSize = defaultFont.MeasureString(Start) + StartHitboxBuffer;
            startHitbox = new Rectangle(
                (int)((Globals.Camera.Width * EntityBatch.PixelateMultiplier - startSize.X) / 2f),
                (int)(((Globals.Camera.Height - 6) * EntityBatch.PixelateMultiplier + startSize.Y / 2f) / 2f),
                (int)startSize.X, (int)startSize.Y
            );
        }

        public override void Update()
        {
            timer += 0.01f;

            MouseState mouse = Mouse.GetState();

            isHoveringOverStart = startHitbox.Contains(mouse.Position.ToVector2() * EntityBatch.PixelateMultiplier) && timer - StartTimerWait > 1;

            if (mouse.LeftButton == ButtonState.Pressed && isHoveringOverStart)
                GameState = GameStates.StartGame;
            
        }

        public override void Draw()
        {
            if (isHoveringOverStart)
            {
                spriteBatch.Draw(DrawUtils.createTexture(spriteBatch.GraphicsDevice),
                    startHitbox,
                    Color.Black * 0.25f);
            }
        }

        //Will draw with UI shader effect
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

                //Could make this more efficient by caching the width of the characters
                stringWidthProgress += defaultFont.MeasureString(STitle[i].ToString()).X;
            }

            //Draw start button
            stringWidthProgress = 0f;
            fullStringWidth = defaultFont.MeasureString(Start).X;
            for (int i = 0; i < Start.Length; i++)
            {
                Vector2 curDrawPos = Vector2.Lerp(StartStartPos, StartEndPos, EasingFunctions.OutBounce(MathHelper.Clamp(timer - StartTimerWait - startLetterOffsets[i], 0, 1)));

                if (timer - StartLetterOffsetVariation > 0)
                {
                    float yOffsetProgress = (timer - startLetterOffsets[i]) % (BobTime);
                    yOffsetProgress = 1 - MathF.Abs(yOffsetProgress - 0.5f) * 2f;

                    yOffsetProgress = EasingFunctions.InOutCubic(yOffsetProgress);
                    curDrawPos += new Vector2(0, yOffsetProgress * BobDist);
                }

                spriteBatch.DrawString(defaultFont,
                    Start[i].ToString(),
                    curDrawPos + new Vector2(stringWidthProgress - fullStringWidth / 2f, 0f),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One,
                    SpriteEffects.None,
                    0f);

                stringWidthProgress += defaultFont.MeasureString(STitle[i].ToString()).X;
            }

            //Draw exit button
            //TODO
        }
    }
}
