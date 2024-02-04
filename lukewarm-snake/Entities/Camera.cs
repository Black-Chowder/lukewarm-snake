using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SimplexNoise;

namespace BlackMagic
{

    public class Camera
    {
        public static Random rand = new Random();

        //Position
        public Vector2 Pos { get; private set;} = Vector2.Zero;
        public float X 
        { 
            get => Pos.X; 
            private set => Pos = new Vector2(value, Pos.Y); 
        }
        public float Y 
        { 
            get => Pos.Y; 
            private set => Pos = new Vector2(Pos.X, value); 
        }

        //Size
        public const int defaultWidth = 1280;
        public const int defaultHeight = 720;
        public int Width { get; private set; } = defaultWidth;
        public int Height { get; private set; } = defaultHeight;
        public Vector2 Size { get => new(Width, Height); set { Width = (int)value.X; Height = (int)value.Y; } }

        //Game scaler
        public float GameScale { get; private set; } = 1f;
        public float Scale { get; private set; } = 2f;

        //Zoom
        public float Zoom { get; set; } = 1f;

        //Target position.  Where the camera wants to go
        public Vector2 Target { get; private set; } = Vector2.Zero;

        //Speed
        public float Speed { get; set; } = 15f;

        //Camera Shake Variables
        public TimeSpan ShakeTimer { get; private set; } = TimeSpan.Zero;
        private TimeSpan requestedDuration = TimeSpan.Zero;
        private float intensity = 0f;
        public bool IsShaking { get => ShakeTimer > TimeSpan.Zero; }

        //Noise variables
        private float[,] noise2;
        private int _noiseIndex = 0;
        private int noiseIndex
        {
            get //ensures _noiseIndex is always a valid index of noise array
            {
                _noiseIndex++;
                if (_noiseIndex >= noise2.GetLength(0))
                    _noiseIndex = 0;
                return _noiseIndex;
            }
        }

        //Manual Mouse Control (Mostly To Be Used For Testing)
        private bool mouseControl = true;
        private Vector2 deltaMouse = Vector2.Zero;
        private int prevScrollVal = 0;

        public Camera()
        {
            //Generate noise to be used for camer shake
            int width = 255;
            float scale = 0.10f;
            noise2 = Noise.Calc2D(width, 3, scale); //(arbitrairy, [x, y, angle], arbitrairy)
            for (int r = 0; r < noise2.GetLength(0); r++)
                for (int c = 0; c < noise2.GetLength(1); c++)
                    noise2[r, c] = (noise2[r, c]) - 128;
        }

        //To run every frame
        public void Update(GameTime gameTime)//TODO: Account for gametime
        {
            camShakeHandler(gameTime);

            //Mouse control stuff
            MouseState mouse = Mouse.GetState();
            if (mouseControl) MouseControlHandler(mouse);
            deltaMouse = mouse.Position.ToVector2();

            //Move camera to requested location based on speed
            Pos += (Target - Pos) / Speed;
        }

        //Update gamescale according to screen dimensions
        private void updateGameScale() =>
            GameScale = Width / (float)defaultWidth;

        //Sets window dimensions to fullscreen (TODO: Figure out if this works if using camera with render target of different size)
        public void SetDimensions(GraphicsDeviceManager graphics, GraphicsDevice GraphicsDevice) =>
            SetDimensions(graphics, GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height, true);

        //Sets window dimensions to requested width and height (TODO: Figure out if this works if using camera with render target of different size)
        public void SetDimensions(GraphicsDeviceManager graphics, int Width = defaultWidth, int Height = defaultHeight, bool isFullScreen = true)
        {
            //Actually change window dimensions
            graphics.PreferredBackBufferWidth = Width;
            graphics.PreferredBackBufferHeight = Height;
            graphics.IsFullScreen = isFullScreen;
            graphics.ApplyChanges();

            //Stores new changes
            this.Width = Width;
            this.Height = Height;

            updateGameScale();
        }

        //Force the camera to go to a specific location
        public void SudoGoTo(float x, float y)
        {
            //Sets both camera and target to same location
            X = x;
            Y = y;
            Target = new(x, y);
        }
        public void SudoGoTo(Vector2 pos) =>
            SudoGoTo(pos.X, pos.Y);

        //Travel to target location according to speed
        public void GoTo(Vector2 pos) =>
            Target = pos;
        public void GoTo(float x, float y) =>
            GoTo(new(x, y));

        //Public method to make camera shake
        public void Shake(float intensity, TimeSpan duration)
        {
            ShakeTimer = requestedDuration = duration;
            this.intensity = intensity;
        }

        //Handle camera shaking (TODO: Implement better camera shaking algorithm)
        private void camShakeHandler(GameTime gt)
        {
            if (ShakeTimer <= TimeSpan.Zero) return;
            ShakeTimer -= gt.ElapsedGameTime;

            //Calculates drop off in intensity over time
            float intensityPercent = (float)(ShakeTimer.TotalMilliseconds / requestedDuration.TotalMilliseconds);

            Vector2 offset = new(noise2[noiseIndex, 0], noise2[noiseIndex, 1]);
            Pos += intensity * intensityPercent * offset;
        }

        private void MouseControlHandler(MouseState mouse)
        {
            if (mouse.MiddleButton == ButtonState.Pressed)
                SudoGoTo(X + (deltaMouse.X - mouse.X) / GameScale, Y + (deltaMouse.Y - mouse.Y) / GameScale);

            
            var curScrollVal = mouse.ScrollWheelValue;
            if (curScrollVal != prevScrollVal)
            {
                Zoom += (curScrollVal - prevScrollVal) / 1000f;
                Debug.WriteLine($"Zoom = {Zoom}");
                prevScrollVal = curScrollVal;
            }
        }
    }
}
