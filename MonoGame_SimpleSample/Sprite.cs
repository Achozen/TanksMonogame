using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGame_SimpleSample
{

    struct Triangle
    {
        public Vector3 A, B, C;

        public Triangle(Vector3 a, Vector3 b, Vector3 c)
        {
            A = a;
            B = b;
            C = c;
        }
    }
    class Sprite
    {
        public Boolean shouldDraw = true;
        protected Texture2D texture;
        public Vector2 position;
        public SpriteFont font;
        protected float rotation;
        protected SpriteEffects effects;

        private Texture2D rect;

        protected BoundingBox boundingBox;
        public BoundingBox BoundingBox
        {
            get
            {
                return this.boundingBox;
            }

        }

        protected BoundingBox bottomBoundingBox;
        public BoundingBox BottomBoundingBox
        {
            get
            {
                return this.bottomBoundingBox;
            }

        }
        protected BoundingBox topBoundingBox;
        public BoundingBox TopBoundingBox
        {
            get
            {
                return this.topBoundingBox;
            }

        }

        protected BoundingBox leftBoundingBox;
        public BoundingBox LeftBoundingBox
        {
            get
            {
                return this.leftBoundingBox;
            }

        }
        protected BoundingBox rightBoundingBox;
        public BoundingBox RightBoundingBox
        {
            get
            {
                return this.rightBoundingBox;
            }

        }

        protected int frameWidth;
        protected int frameHeight;

        public Sprite(Texture2D texture, Vector2 startingPosition, float rotation)
        {
            this.rotation = (float)(Math.PI * rotation / 180.0);
            position = startingPosition;
            this.texture = texture;
            frameHeight = texture.Height;
            frameWidth = texture.Width;
            updateBoundingBoxes();
        }

        public Sprite(Texture2D texture, Vector2 startingPosition):this(texture, startingPosition, 0f)
        {
        }


        public void Update(GameTime gameTime)
        {
            if (!shouldDraw) return;
            updateBoundingBoxes();
        }

        protected void updateBoundingBoxes()
        {
            boundingBox = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + frameWidth, position.Y + frameHeight, 0));
            bottomBoundingBox = new BoundingBox(new Vector3(position.X + 2, position.Y + frameHeight - 2, 0), new Vector3(position.X + frameWidth - 2, position.Y + frameHeight, 0));
            topBoundingBox = new BoundingBox(new Vector3(position.X + 2, position.Y, 0), new Vector3(position.X + frameWidth - 2, position.Y + 2, 0));
            leftBoundingBox = new BoundingBox(new Vector3(position.X, position.Y + 2, 0), new Vector3(position.X + 2, position.Y + frameHeight - 2, 0));
            rightBoundingBox = new BoundingBox(new Vector3(position.X + frameWidth - 2, position.Y + 2, 0), new Vector3(position.X + frameWidth, position.Y + frameHeight - 2, 0));

        }


        public void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
           if (!shouldDraw) return;
            var origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            spriteBatch.Draw(texture, new Rectangle ( (int)(position.X + origin.X), (int)(position.Y + origin.Y),  frameWidth, frameHeight), null, Color.White, rotation, origin, effects, 0f);
            Debug_DrawBounds(graphicsDevice, spriteBatch);
            if (font != null)
                spriteBatch.DrawString(font, position.ToString(), position+origin, Color.Black, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
        }
//(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth);

        public bool IsCollidingWith(Sprite otherSprite)
        {
            return this.boundingBox.Intersects(otherSprite.BoundingBox) ? true : false;
        }


        public void Debug_DrawBounds(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            if (rect == null)
                rect = new Texture2D(graphicsDevice, 1, 1);
            rect.SetData(new[] { Color.White });

            DrawTriangle(spriteBatch, position, position + new Vector2(texture.Width, 0), position + new Vector2(0, texture.Height), Color.Aqua);
            DrawTriangle(spriteBatch, position + new Vector2(texture.Width, 0), position + new Vector2(texture.Width, texture.Height), position + new Vector2(0, texture.Height), Color.Yellow);

            DrawRectangle(graphicsDevice, spriteBatch, BottomBoundingBox, Color.Red);
            DrawRectangle(graphicsDevice, spriteBatch, TopBoundingBox, Color.Green);
            DrawRectangle(graphicsDevice, spriteBatch, LeftBoundingBox, Color.Blue);
            DrawRectangle(graphicsDevice, spriteBatch, RightBoundingBox, Color.Violet);
        }

        private void DrawRectangle(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, BoundingBox boundingBox, Color color)
        {
             int rectWidth = (int)(boundingBox.Max.X - boundingBox.Min.X);
            int rectHeight = (int)(boundingBox.Max.Y - boundingBox.Min.Y);

            Rectangle coords = new Rectangle((int)boundingBox.Min.X, (int)boundingBox.Min.Y, rectWidth, rectHeight);
            
            //DrawLine(spriteBatch, position, position + new Vector2(texture.Width, texture.Height), Color.Aqua);
           // DrawLine(spriteBatch, position, position + new Vector2(texture.Width, texture.Height), Color.LimeGreen);
            //spriteBatch.Draw(rect, new Rectangle((int)boundingBox.Min.X, (int)boundingBox.Min.Y, 2, 2), Color.Aqua);
            //spriteBatch.Draw(rect, new Rectangle((int)boundingBox.Min.X+ rectWidth - 2, (int)boundingBox.Min.Y+ rectHeight - 2, 2, 2), Color.Aqua);
            spriteBatch.Draw(rect, coords, color);

        }

        public bool isIntersectingWith(Sprite sprite)
        {

            return false;
        }

        public String toLevelFormat()
        {
            return texture.Name + ";" + position.X + ";" + position.Y+";"+RadianToDegree(rotation);
        }
        double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }
        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        void DrawTriangle(SpriteBatch sb, Vector2 a, Vector2 b, Vector2 c, Color color)
        {
            DrawLine(sb, a, b, color);
            DrawLine(sb, b, c, color);
            DrawLine(sb, c, a, color);
        }

        void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color color)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float)Math.Atan2(edge.Y, edge.X);


            sb.Draw(rect,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    3), //width of line, change this to make thicker line
                null,
                color, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);

        }
    }
}
