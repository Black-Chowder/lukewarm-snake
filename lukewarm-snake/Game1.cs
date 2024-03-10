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

        //Background effect variables
        private Effect BackgroundEffect;
        public RenderTarget2D BackgroundBuffer1;
        private RenderTarget2D BackgroundBuffer2;
        private RenderTarget2D BackgroundRt;
        private Point iResolution;
        private const float Damping = 0.99f;
        private const int BrushSize = 1;

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
            Globals.GameState = Globals.GameStates.StartGame;
            IsMouseVisible = true;

            iResolution = new Point((int)(Globals.Camera.Width * EntityBatch.PixelateMultiplier), (int)(Globals.Camera.Height * EntityBatch.PixelateMultiplier));

            BackgroundBuffer1 = new RenderTarget2D(GraphicsDevice, iResolution.X, iResolution.Y);
            BackgroundBuffer2 = new RenderTarget2D(GraphicsDevice, iResolution.X, iResolution.Y);
            BackgroundRt = BackgroundBuffer2;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            content = Content;
            defaultFont = Content.Load<SpriteFont>(@"DefaultFont");

            BackgroundEffect = Content.Load<Effect>(@"Effects/Background");

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

                    /*  <Handle Input>  */
                    //Get mouse input
                    MouseState mouseState = Mouse.GetState();
                    Point mousePos = mouseState.Position;
                    bool isMousePressed = mouseState.LeftButton == ButtonState.Pressed;

                    if (isMousePressed)
                    {
                        //Handle input for shader-based effect
                        RenderTarget2D newPrev = new RenderTarget2D(GraphicsDevice, iResolution.X, iResolution.Y);
                        GraphicsDevice.SetRenderTarget(newPrev);
                        spriteBatch.Begin();
                        spriteBatch.Draw(BackgroundBuffer1, Vector2.Zero, Color.White);
                        spriteBatch.Draw(DrawUtils.createTexture(GraphicsDevice),
                            new Rectangle((int)(mousePos.X * EntityBatch.PixelateMultiplier), (int)(mousePos.Y * EntityBatch.PixelateMultiplier), BrushSize, BrushSize),
                            Color.White);
                        spriteBatch.End();
                        GraphicsDevice.SetRenderTarget(null);

                        BackgroundBuffer1.Dispose();
                        BackgroundBuffer1 = newPrev;
                    }

                    /*  </Handle Input>  */

                    /*  <Itterate Background Effect>  */

                    //Set effect parameters
                    BackgroundEffect.Parameters["iResolution"].SetValue(iResolution.ToVector2());
                    BackgroundEffect.Parameters["damping"].SetValue(Damping);
                    BackgroundEffect.Parameters["Previous"].SetValue(BackgroundBuffer1);

                    //Calculate new frame
                    BackgroundRt = new RenderTarget2D(GraphicsDevice, iResolution.X, iResolution.Y);
                    GraphicsDevice.SetRenderTarget(BackgroundRt);
                    GraphicsDevice.Clear(Color.Black);
                    spriteBatch.Begin(effect: BackgroundEffect);
                    spriteBatch.Draw(BackgroundBuffer2, Vector2.Zero, Color.White);
                    spriteBatch.End();
                    GraphicsDevice.SetRenderTarget(null);

                    //Set current render target to new frame
                    BackgroundBuffer2.Dispose();
                    BackgroundBuffer2 = BackgroundRt;

                    //Swap
                    (BackgroundBuffer2, BackgroundBuffer1) = (BackgroundBuffer1, BackgroundBuffer2);

                    /*  </Itterate Background Effect>  */

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
            GraphicsDevice.Clear(Color.Black/*new Color(118, 59, 54)*/);

            switch (Globals.GameState)
            {
                case Globals.GameStates.TestLoop:


                    GraphicsDevice.SetRenderTarget(null);
                    spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                    spriteBatch.Draw(BackgroundRt,
                        new Rectangle(0, 0, (int)(iResolution.X / EntityBatch.PixelateMultiplier), (int)(iResolution.Y / EntityBatch.PixelateMultiplier)),
                        Color.White);
                    spriteBatch.End();

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
                Color.White);
            Globals.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}