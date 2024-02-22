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

namespace lukewarm_snake
{
    public class Game1 : Game
    {
        public static GraphicsDeviceManager graphics;

        private Effect testShader;
        private Texture2D noiseTexture;
        private RenderTarget2D rt;
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
            //Globals.GameState = Globals.GameStates.StartGame;
            Globals.GameState = Globals.GameStates.Test;
            IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Globals.spriteBatch = new SpriteBatch(GraphicsDevice);
            Globals.content = Content;
            Globals.defaultFont = Content.Load<SpriteFont>(@"DefaultFont");

            testShader = Content.Load<Effect>(@"Effects/Background");

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
                case Globals.GameStates.Test:

                    Globals.GameState = Globals.GameStates.TestLoop;
                    goto case Globals.GameStates.TestLoop;

                case Globals.GameStates.TestLoop:

                    break;

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
                case Globals.GameStates.TestLoop: break;
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
                case Globals.GameStates.TestLoop:

                    iTimer += 0.001f;
                    testShader.Parameters["iTimer"].SetValue(iTimer);
                    testShader.CurrentTechnique.Passes[0].Apply();

                    float testBodySize = 200f;
                    RenderTarget2D rt = new RenderTarget2D(Globals.spriteBatch.GraphicsDevice, (int)(testBodySize * EntityBatch.PixelateMultiplier), (int)(testBodySize * EntityBatch.PixelateMultiplier));
                    Globals.spriteBatch.GraphicsDevice.SetRenderTarget(rt);
                    Globals.spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: testShader);
                    Globals.spriteBatch.Draw(DrawUtils.createTexture(Globals.spriteBatch.GraphicsDevice, Color.White),
                        new Rectangle(0, 0, (int)(testBodySize * EntityBatch.PixelateMultiplier), (int)(testBodySize * EntityBatch.PixelateMultiplier)),
                        new Color(0f, 0.2f, 0f, 1f));
                    Globals.spriteBatch.End();
                    Globals.spriteBatch.GraphicsDevice.SetRenderTarget(null);
                    Globals.spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: testShader);
                    Globals.spriteBatch.Draw(rt,
                        new Rectangle(0, 0, Globals.Camera.Height, Globals.Camera.Height),
                        Color.CornflowerBlue);
                    Globals.spriteBatch.End();

                    rt.Dispose();
                    break;

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