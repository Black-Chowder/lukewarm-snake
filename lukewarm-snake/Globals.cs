using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackMagic
{
    public class Globals
    {
        public static ContentManager content;
        public static SpriteBatch spriteBatch;
        public static GameTime gt;

        public const float fixedUpdateDelta = (int)(1000f / 30f);
        //stores how far we are in current frame
        public static float ALPHA = 0f;

        public static SpriteFont defaultFont;

        public enum GameStates
        {
            Test,
            TestLoop,
            StartScreen,
            StartRound,
            StartGame,
            GameLoop,
            GameOver
        }
        public static GameStates GameState = GameStates.Test;
        public static Camera Camera;

        public static EntityBatch MainEntityBatch;

        public static Random rnd = new Random();

        public static float TimeMod = 1f;
    }
}
