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
    class Sprite
    {
        public Boolean shouldDraw = true;
        protected Texture2D texture;
        protected Vector2 position;

        private Texture2D rect;
        public Vector2 Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
            }
        }

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

        public Sprite(Texture2D texture, Vector2 startingPosition)
        {
            position = startingPosition;
            this.texture = texture;
            frameHeight = texture.Height;
            frameWidth = texture.Width;
            updateBoundingBoxes();

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
            spriteBatch.Draw(texture, position, Color.White);
            Debug_DrawBounds(graphicsDevice, spriteBatch);

        }


        public bool IsCollidingWith(Sprite otherSprite)
        {
            return this.boundingBox.Intersects(otherSprite.BoundingBox) ? true : false;
        }


        public void Debug_DrawBounds(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            DrawRectangle(graphicsDevice, spriteBatch, BottomBoundingBox, Color.Red);
            DrawRectangle(graphicsDevice, spriteBatch, TopBoundingBox, Color.Green);
            DrawRectangle(graphicsDevice, spriteBatch, LeftBoundingBox, Color.Blue);
            DrawRectangle(graphicsDevice, spriteBatch, RightBoundingBox, Color.Violet);
        }

        private void DrawRectangle(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, BoundingBox boundingBox, Color color)
        {
            if (rect == null)
                rect = new Texture2D(graphicsDevice, 1, 1);
            rect.SetData(new[] { Color.White });
            int rectWidth = (int)(boundingBox.Max.X - boundingBox.Min.X);
            int rectHeight = (int)(boundingBox.Max.Y - boundingBox.Min.Y);

            Rectangle coords = new Rectangle((int)boundingBox.Min.X, (int)boundingBox.Min.Y, rectWidth, rectHeight);

            spriteBatch.Draw(rect, coords, color);
        }


    }
}
