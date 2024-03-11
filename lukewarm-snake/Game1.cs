﻿using Microsoft.Xna.Framework;
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
            GameState = GameStates.StartGame;
            IsMouseVisible = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            content = Content;
            defaultFont = Content.Load<SpriteFont>(@"DefaultFont");

        }

        protected override void Update(GameTime gameTime)
        {
            gt = gameTime;
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
                    GameState = GameStates.TestLoop;
                    goto case GameStates.TestLoop;

                case GameStates.TestLoop:
                    break;

                case GameStates.StartGame:
                    MainEntityBatch?.Dispose();
                    MainEntityBatch = new();
                    MainEntityBatch.InitEntityBucket<Food>();
                    MainEntityBatch.InitEntityBucket<Obstacle>();
                    MainEntityBatch.Add(new Snake());
                    MainEntityBatch.Add(new SpawnManager());
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
                    break;

                case GameStates.GameLoop:
                    MainEntityBatch.Draw();
                    break;
            }

            //Display average fps in top-left corner
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

            base.Draw(gameTime);
        }
    }
}