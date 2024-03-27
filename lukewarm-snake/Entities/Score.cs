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
        public int PlayerScore { get; set; } = 0;

        
        public Score() : base(Vector2.Zero)
        {
            CalcPos();
        }

        public override void Update()
        {
            Snake snake = batch.GetEntityBucket<Snake>()?.First() as Snake;
            PlayerScore = snake.GetTrait<FoodEater>().FoodEaten;
            CalcPos();
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
                Vector2.One * 1.001f,
                SpriteEffects.None,
                0f);
        }

        private void CalcPos() =>
            Pos = prevPos = new Vector2(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, 0);
    }
}
