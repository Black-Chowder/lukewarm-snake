using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BlackMagic;
using System;
using System.Linq;

namespace lukewarm_snake
{
    public class Game1 : Game
    {
        public static GraphicsDeviceManager graphics;

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
            Globals.GameState = Globals.GameStates.StartGame;
            IsMouseVisible = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Globals.spriteBatch = new SpriteBatch(GraphicsDevice);
            Globals.content = Content;
            Globals.defaultFont = Content.Load<SpriteFont>(@"DefaultFont");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            Globals.gt = gameTime;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

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

            while (accumulator >= Globals.fixedUpdateDelta)
            {
                FixedUpdate();
                accumulator -= Globals.fixedUpdateDelta;
            }

            Globals.ALPHA = (accumulator / Globals.fixedUpdateDelta);
            //  </Handle Fixed Update>  //

            //Normal update loop
            switch (Globals.GameState)
            {
                case Globals.GameStates.StartGame:
                    Globals.MainEntityBatch?.Dispose();
                    Globals.MainEntityBatch = new();
                    Globals.MainEntityBatch.InitEntityBucket<Food>();
                    Globals.MainEntityBatch.InitEntityBucket<Obstacle>();
                    Globals.MainEntityBatch.Add(new Snake());
                    Globals.MainEntityBatch.Add(new SpawnManager());
                    Globals.GameState = Globals.GameStates.GameLoop;
                    goto case Globals.GameStates.GameLoop;

                case Globals.GameStates.GameLoop:
                    Globals.MainEntityBatch.Update();
                    break;
            }

            base.Update(gameTime);
        }

        private void FixedUpdate()
        {
            switch (Globals.GameState)
            {
                case Globals.GameStates.TestLoop:
                case Globals.GameStates.GameLoop:
                    Globals.MainEntityBatch.FixedUpdate();
                    break;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            //penumbra.BeginDraw();
            GraphicsDevice.Clear(new Color(118, 59, 54));

            switch (Globals.GameState)
            {
                case Globals.GameStates.GameLoop:
                    Globals.MainEntityBatch.Draw();
                    break;
            }

            //Display average fps in top-left corner
            Globals.spriteBatch.Begin();
            float FPS = MathF.Round(1f / (float)Globals.gt.ElapsedGameTime.TotalSeconds);
            if (fpss.Count >= 100)
                fpss.Dequeue();
            fpss.Enqueue(FPS);
            FPS = fpss.Average();

            Globals.spriteBatch.DrawString(Globals.defaultFont,
                $"FPS:{FPS}",
                Vector2.Zero,
                Color.Black);
            Globals.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}