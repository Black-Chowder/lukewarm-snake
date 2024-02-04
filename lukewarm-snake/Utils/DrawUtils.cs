using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackMagic
{
    public class DrawUtils
    {
        /// <summary>
        /// Loads Sprite Sheet Into List Of Rectangles To Be Used 
        /// As Parameters In spriteBatch.Draw() as the parameter sourceRectangle.  
        /// 
        /// Example:
        /// init(){
        ///     Rectangle[] sprites = new Rectangle[200];
        ///     sprites = spriteSheetLoader(32, 32, 320, 320);
        /// }
        /// draw(){
        ///     spriteBatch.Begin();
        ///     spriteBatch.Draw(texture, 
        ///         destinationRectangle: new Rectangle(this.x, this.y, this.width, this.height), 
        ///         sourceRectangle: sprites[0],   <<<======
        ///         color: Color.White);
        ///     spriteBatch.End();
        /// }
        /// </summary>
        /// <param name="spriteWidth"></param>
        /// <param name="spriteHeight"></param>
        /// <param name="spriteSheetWidth"></param>
        /// <param name="spriteSheetHeight"></param>
        /// <returns> Rectangle[] </returns>
        public static Rectangle[] spriteSheetLoader(int spriteWidth, int spriteHeight, int columns, int rows)
        {
            int spritesInSpriteSheet = columns * rows;

            Rectangle[] placeholder = new Rectangle[0];
            return spriteSheetLoader(placeholder, spriteWidth, spriteHeight, columns, rows, 0, spritesInSpriteSheet, false);
        }
        public static Rectangle[] spriteSheetLoader(int spriteWidth, int spriteHeight, int columns, int rows, int endingSprite)
        {
            Rectangle[] placeholder = new Rectangle[0];
            return spriteSheetLoader(placeholder, spriteWidth, spriteHeight, columns, rows, 0, endingSprite, false);
        }
        public static Rectangle[] spriteSheetLoader(int spriteWidth, int spriteHeight, int columns, int rows, int startingSprite, int endingSprite)
        {
            Rectangle[] placeholder = new Rectangle[0];
            return spriteSheetLoader(placeholder, spriteWidth, spriteHeight, columns, rows, startingSprite, endingSprite, false);
        }
        public static Rectangle[] spriteSheetLoader(int spriteWidth, int spriteHeight, int columns, int rows, int startingSprite, int endingSprite, Boolean inReverse)
        {
            Rectangle[] placeholder = new Rectangle[0];
            return spriteSheetLoader(placeholder, spriteWidth, spriteHeight, columns, rows, startingSprite, endingSprite, inReverse);
        }
        public static Rectangle[] spriteSheetLoader(Rectangle[] spriteSheet, int spriteWidth, int spriteHeight, int columns, int rows, int startingSprite, int endingSprite, Boolean inReverse)
        {
            //TODO: Implement Starting Sprite Functionality!!! <<<========  High Priority
            Rectangle[] toReturn = new Rectangle[spriteSheet.Count() + Math.Abs(endingSprite - startingSprite)];

            Boolean wantToBreak = false;
            int spriteCounter = 0;

            //Writes loaded spriteSheet to toReturn
            for (int i = 0; i < spriteSheet.Count(); i++)
            {
                toReturn[i] = spriteSheet[i];
            }

            //FOR GOING NORMAL DIRECTION
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    if (spriteCounter >= startingSprite)
                    {
                        toReturn[(spriteCounter - startingSprite) + spriteSheet.Count()] = new Rectangle(
                            x * spriteWidth,
                            y * spriteHeight,
                            spriteWidth,
                            spriteHeight);
                    }
                    if (spriteCounter + 2 > endingSprite)
                    {
                        wantToBreak = true;
                        break;
                    }

                    spriteCounter++;
                }
                if (wantToBreak)
                {
                    break;
                }
            }

            if (inReverse)
            {
                Rectangle[] reverseReturn = new Rectangle[toReturn.Count()];

                //Loads previous sprite sheet
                for (int i = 0; i < spriteSheet.Count(); i++)
                {
                    reverseReturn[i] = spriteSheet[i];
                }

                //Reverses new sprites
                for (int i = spriteSheet.Count(); i < toReturn.Count(); i++)
                {
                    reverseReturn[reverseReturn.Count() + spriteSheet.Count() - i - 1] = toReturn[i];
                }
                return reverseReturn;
            }

            return toReturn;
        }

        //Draws Lines
        private static Texture2D GetTexture(SpriteBatch spriteBatch)
        {
            return createTexture(spriteBatch.GraphicsDevice);
            //new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(spriteBatch, point1, distance, angle, color, thickness);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Texture2D texture, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(spriteBatch, texture, point1, distance, angle, color, thickness);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(GetTexture(spriteBatch), point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Texture2D texture, Vector2 point, float length, float angle, Color color, float thickness = 1f)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(texture, point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }

        public static Texture2D createCircleTexture(GraphicsDevice GraphicsDevice, int circumference)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, circumference, circumference);
            Color[] colorData = new Color[circumference * circumference];

            float diam = circumference / 2f;
            float diamsq = diam * diam;

            for (int x = 0; x < circumference; x++)
            {
                for (int y = 0; y < circumference; y++)
                {
                    int index = x * circumference + y;
                    Vector2 pos = new Vector2(x - diam, y - diam);
                    if (pos.LengthSquared() <= diamsq)
                    {
                        colorData[index] = Color.White;
                    }
                    else
                    {
                        colorData[index] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colorData);
            return texture;
        }

        private static Texture2D texture = null;
        public static Texture2D createTexture(GraphicsDevice graphicsDevice)
        {
            if (texture != null) return texture;
            texture = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            texture.SetData<Color>(new Color[] { Color.White });
            return texture;
        }

        public static void fillRect(SpriteBatch spriteBatch, int x, int y, int width, int height, Color color, Texture2D texture = null)
        {
            if (width <= 0 || height <= 0) return;
            texture ??= createTexture(spriteBatch.GraphicsDevice);

            if (width > height)
            {
                for (int i = 0; i < width / height; i++)
                    spriteBatch.Draw(texture,
                        new Vector2(x + i * height, y),
                        new Rectangle(0, 0, 1, 1),
                        color,
                        0,
                        new Vector2(0, 0),
                        height,
                        SpriteEffects.None,
                        0f);
                spriteBatch.Draw(texture,
                    new Vector2(x + width - height, y),
                    new Rectangle(0, 0, 1, 1),
                    color,
                    0,
                    new Vector2(0, 0),
                    height,
                    SpriteEffects.None,
                    0f);
            }

            else
            {
                for (int i = 0; i < height / width; i++)
                    spriteBatch.Draw(texture,
                        new Vector2(x, y + i * width),
                        new Rectangle(0, 0, 1, 1),
                        color,
                        0,
                        new Vector2(0, 0),
                        width,
                        SpriteEffects.None,
                        0f);
                spriteBatch.Draw(texture,
                    new Vector2(x, y + height - width),
                    new Rectangle(0, 0, 1, 1),
                    color,
                    0,
                    new Vector2(0, 0),
                    width,
                    SpriteEffects.None,
                    0f);
            }
        }
    
        public static void DrawRectBorder(SpriteBatch spriteBatch, Rectangle rect, Color color, float thickness = 1)
        {
            //Draw top
            DrawLine(spriteBatch, new Vector2(rect.X, rect.Y), new Vector2(rect.X + rect.Width, rect.Y), color, thickness);

            //Draw right
            DrawLine(spriteBatch, new Vector2(rect.X + rect.Width, rect.Y), new Vector2(rect.X + rect.Width, rect.Y + rect.Height), color, thickness);

            //Draw bottom
            DrawLine(spriteBatch, new Vector2(rect.X + rect.Width, rect.Y + rect.Height), new Vector2(rect.X, rect.Y + rect.Height), color, thickness);

            //Draw left
            DrawLine(spriteBatch, new Vector2(rect.X, rect.Y + rect.Height), new Vector2(rect.X, rect.Y), color, thickness);
        }

        //Source: https://gamedev.stackexchange.com/questions/46775/xna-why-is-texture-getdata-one-dimensional
        public static Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colorsOne = new Color[texture.Width * texture.Height]; //The hard to read,1D array
            texture.GetData(colorsOne); //Get the colors and add them to the array

            Color[,] colorsTwo = new Color[texture.Width, texture.Height]; //The new, easy to read 2D array
            for (int x = 0; x < texture.Width; x++) //Convert!
                for (int y = 0; y < texture.Height; y++)
                    colorsTwo[x, y] = colorsOne[x + y * texture.Width];

            return colorsTwo; //Done!
        }
    }
}
