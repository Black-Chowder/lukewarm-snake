using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using static BlackMagic.Globals;
using Kryz.Tweening;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace lukewarm_snake
{
    public class Score : Entity
    {
        public int PlayerScore { get => Globals.PlayerScore; set => Globals.PlayerScore = value; }

        //Score grow on score change variables
        private float maxGrowPercent = 1.25f;
        private float growMultiplier = 1f;
        private float growthTimer = 0f;
        private const float growthTimerSet = 25f;
        
        public Score() : base(Vector2.Zero)
        {
            Pos = prevPos = new Vector2(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, 0);
        }

        public override void Update()
        {
            //Handle growth multiplier
            growthTimer--;
            growMultiplier = MathHelper.Clamp(EasingFunctions.OutCubic(growthTimer / growthTimerSet) * (maxGrowPercent - 1f) + 1f, 1f, maxGrowPercent);

            //Set Score
            Snake snake = batch.GetEntityBucket<Snake>()?.First() as Snake;
            int newScore = snake.GetTrait<FoodEater>().FoodEaten;
            if (newScore == PlayerScore) //If score isn't new, return early
                return;
            PlayerScore = newScore;

            growthTimer = growthTimerSet; //Reset score growth multiplier timer
        }

        public override void DrawRaw()
        {
            Vector2 textSize = defaultFont.MeasureString(PlayerScore.ToString());
            spriteBatch.DrawString(defaultFont,
                PlayerScore.ToString(),
                DrawPos,
                Color.White,
                0f,
                new Vector2(textSize.X / 2f, 0f),
                Vector2.One * 1.001f * growMultiplier,
                SpriteEffects.None,
                0f);
        }
    }
}
