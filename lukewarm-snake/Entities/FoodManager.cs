using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using static BlackMagic.Globals;
using Microsoft.Xna.Framework;

namespace lukewarm_snake
{
    public class FoodManager : Entity
    {
        public Food Food { get; set; }
        public const int FoodSpawnPadding = 50;

        public FoodManager() : base(Vector2.Zero) => Food = new Food();

        public override void Update()
        {
            Food.Update();
            SpawnManager();
        }

        public override void FixedUpdate() => Food.FixedUpdate();

        public override void Draw() => Food.Draw();

        public override void DrawRippleInfluence() => Food.DrawRippleInfluence();

        private void SpawnManager()
        {
            //Spawn food
            if (!Food.IsActive)
            {
                Food.Init(GenerateRandomPosInRect(new Rectangle(
                    FoodSpawnPadding, FoodSpawnPadding, Globals.Camera.Width - FoodSpawnPadding * 2, Globals.Camera.Height - FoodSpawnPadding * 2
                )));
            }
        }

        private Vector2 GenerateRandomPosInRect(Rectangle rectangle)
        {
            int x = rnd.Next(rectangle.Left, rectangle.Right);
            int y = rnd.Next(rectangle.Top, rectangle.Bottom);
            return new Vector2(x, y);
        }

    }
}
