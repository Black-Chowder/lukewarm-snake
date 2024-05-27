using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BlackMagic;
using System;
using System.Linq;
using static BlackMagic.Globals;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace lukewarm_snake
{
    public class Game1 : Game
    {
        public static GraphicsDeviceManager graphics;

        //Test effect stuff
        private Effect testEffect;
        private RenderTarget2D testRt;
        private float iTimer = 0f;

        //fixed update variables
        private float previousT = 0f;
        private float accumulator = 0f;
        private float maxFrameTime = 250f;

        private Queue<float> fpss = new();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = false;
            IsMouseVisible = true;

            //grpahics.IsFullScreen = true;
            //graphics.PreferredBackBufferWidth = 1920;
            //graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();
            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            Globals.Camera = new Camera();
            Globals.Camera.SetDimensions(graphics, 1600, 900, false);
            //Globals.Camera.SetDimensions(graphics, 1920, 1080, true);

            //Add Penumbra Component
            //penumbra = new PenumbraComponent(this);
            //Components.Add(penumbra);
            //penumbra.AmbientColor = Color.Black;
        }

        protected override void Initialize()
        {
            GameState = GameStates.StartScreen;
            IsMouseVisible = true;

            //Initialize scoreboard
            ScoreboardScores = new();
            if (!File.Exists(ScoreboardPath))
                File.Create(ScoreboardPath).Close();
            using (StreamReader sr = new(File.OpenRead(ScoreboardPath)))
            {
                string line;
                for (int i = 0; (line = sr.ReadLine()) != null && i < MaxScoreboardScores; i++)
                    ScoreboardScores.Add((line[..3], int.Parse(line[3..])));
            }

            ScoreboardScores.Sort((a, b) => b.Item2.CompareTo(a.Item2)); //Sort in ascending order

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            content = Content;
            defaultFont = Content.Load<SpriteFont>(@"DefaultFont");

            testEffect = Content.Load<Effect>(@"Effects/Depth");

            testRt = new RenderTarget2D(GraphicsDevice, 100, 100);

            //Main background ambience handling
            bgAmbience = Content.Load<Song>(@"SFX/24_Roosvelt Island_water_air_birds_distant traffic");
            MediaPlayer.Play(bgAmbience);
            MediaPlayer.IsRepeating = true;
        }

        protected override void Update(GameTime gameTime)
        {
            gt = gameTime;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Background ambience handling
            if (MediaPlayer.State == MediaState.Stopped) MediaPlayer.Play(bgAmbience);
            else if (MediaPlayer.State == MediaState.Paused) MediaPlayer.Resume();

            ClickHandler.Update();

            //  <Handle Fixed Update>  //
            if (previousT == 0)
                previousT = (float)gameTime.TotalGameTime.TotalMilliseconds;

            float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
            float frameTime = now - previousT;
            if (frameTime > maxFrameTime)
                frameTime = maxFrameTime;

            previousT = now;
            accumulator += frameTime;

            while (accumulator >= fixedUpdateDelta)
            {
                FixedUpdate();
                accumulator -= fixedUpdateDelta;
            }

            ALPHA = (accumulator / fixedUpdateDelta);
            //  </Handle Fixed Update>  //

            //Normal update loop
            switch (GameState)
            {
                case GameStates.Test:
                    MainEntityBatch = new();
                    MainEntityBatch.Add(new TailTester());

                    GameState = GameStates.TestLoop;
                    goto case GameStates.TestLoop;

                case GameStates.TestLoop:
                    MainEntityBatch.Update();
                    break;

                case GameStates.StartScreen:
                    IsMouseVisible = true;
                    MainEntityBatch?.Dispose();
                    MainEntityBatch = new();
                    MainEntityBatch.InitEntityBucket<FoodManager>();
                    MainEntityBatch.Add(new StartSnake(50));
                    MainEntityBatch.Add(new FoodManager());
                    MainEntityBatch.Add(new Title());
                    GameState = GameStates.GameLoop;
                    goto case GameStates.GameLoop;

                case GameStates.StartGame:
                    IsMouseVisible = false;
                    MainEntityBatch?.Dispose();
                    MainEntityBatch = new();
                    MainEntityBatch.InitEntityBucket<Food>();
                    MainEntityBatch.InitEntityBucket<FoodManager>();
                    MainEntityBatch.InitEntityBucket<ObstacleManager>();
                    MainEntityBatch.InitEntityBucket<Snake>();
                    MainEntityBatch.Add(new Snake());
                    MainEntityBatch.Add(new ObstacleManager());
                    MainEntityBatch.Add(new FoodManager());
                    MainEntityBatch.Add(new Score());
                    GameState = GameStates.GameLoop;
                    goto case GameStates.GameLoop;

                case GameStates.GameOver:
                    Debug.WriteLine("In game over state!");
                    IsMouseVisible = true;
                    MainEntityBatch?.Dispose();
                    MainEntityBatch = new();
                    MainEntityBatch.Add(new GameOverUI());

                    GameState = GameStates.GameLoop;
                    goto case GameStates.GameLoop;


                case GameStates.Scoreboard:
                    MainEntityBatch?.Dispose();
                    MainEntityBatch = new();
                    MainEntityBatch.Add(new Scoreboard());

                    GameState = GameStates.GameLoop;
                    goto case GameStates.GameLoop;

                case GameStates.GameLoop:
                    MainEntityBatch.Update();
                    break;
            }

            base.Update(gameTime);
        }

        private void FixedUpdate()
        {
            switch (GameState)
            {
                case GameStates.TestLoop: break;
                case GameStates.GameLoop:
                    MainEntityBatch.FixedUpdate();
                    break;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            //penumbra.BeginDraw();
            GraphicsDevice.Clear(Color.Black/*new Color(118, 59, 54)*/);

            switch (GameState)
            {
                case GameStates.TestLoop:

                    /*
                    iTimer += MathF.Min(0.001f, 1f);

                    //testEffect.Parameters["iTimer"].SetValue(iTimer);
                    //testEffect.Parameters["iTime"].SetValue(iTimer);
                    //testEffect.Parameters["iResolution"].SetValue(new Vector2(testRt.Width, testRt.Height));
                    testEffect.Parameters["depth"].SetValue(iTimer);

                    spriteBatch.GraphicsDevice.SetRenderTarget(testRt);
                    spriteBatch.GraphicsDevice.Clear(Color.Transparent);
                    spriteBatch.Begin(effect: testEffect);
                    spriteBatch.Draw(DrawUtils.createTexture(spriteBatch.GraphicsDevice, Color.Red),
                        new Rectangle(0, 0, testRt.Width, testRt.Height),
                        Color.Gray);
                    spriteBatch.End();

                    spriteBatch.GraphicsDevice.SetRenderTarget(null);
                    spriteBatch.GraphicsDevice.Clear(Color.CornflowerBlue);
                    spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                    spriteBatch.Draw(testRt,
                        new Rectangle(
                            Globals.Camera.Width / 2 - Globals.Camera.Height / 2, 
                            0, 
                            Globals.Camera.Height, 
                            Globals.Camera.Height
                        ),
                        Color.White);
                    spriteBatch.End();
                    */
                    MainEntityBatch.Draw();
                    break;

                case GameStates.GameLoop:
                    MainEntityBatch.Draw();
                    break;
            }

            //Display average fps in top-left corner
            /*
            spriteBatch.Begin();
            float FPS = MathF.Round(1f / (float)gt.ElapsedGameTime.TotalSeconds);
            if (fpss.Count >= 100)
                fpss.Dequeue();
            fpss.Enqueue(FPS);
            FPS = fpss.Average();

            spriteBatch.DrawString(defaultFont,
                $"FPS:{FPS}",
                Vector2.Zero,
                Color.White);
            spriteBatch.End();
            */

            base.Draw(gameTime);
        }
    }
}