using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace MonoGame_SimpleSample
{
    class AnimatedSprite : Sprite
    {
        int numberOfAnimationRows = 4;
        int animationFramesInRow = 4;

        int whichLine;
        private int whichRow;
        double currentFrameTime = 0;
        double expectedFrameTime = 70.0f;

        float dy = 0.0f;
        float dx = 0.0f;
        float gravity = 0.05f;
        Vector2 momentum = Vector2.Zero;


        public AnimatedSprite(Texture2D texture, Vector2 startingPosition, int numberOfAnimationRows,
            int animationFramesInRow) : base(texture, startingPosition)
        {
            base.frameHeight = texture.Height / numberOfAnimationRows;
            base.frameWidth = texture.Width / animationFramesInRow;

            this.numberOfAnimationRows = numberOfAnimationRows;
            this.animationFramesInRow = animationFramesInRow;
        }


        public new void Update(GameTime gameTime)
        {
            if (!shouldDraw) return;

            currentFrameTime += gameTime.ElapsedGameTime.TotalMilliseconds;
           // Console.WriteLine("currentFrameTime:" + currentFrameTime);
            if (currentFrameTime >= expectedFrameTime)
            {
                whichLine = (whichLine < animationFramesInRow - 1) ? whichLine + 1 : 0;
                if (currentFrameTime >= expectedFrameTime / numberOfAnimationRows)
                {
                    whichRow = (whichRow < numberOfAnimationRows - 1) ? whichRow + 1 : 0;
                }

                currentFrameTime = 0;
            }

            if (whichLine == animationFramesInRow - 1)
            {
                shouldDraw = false;
            }
        }

        public new void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            if (!shouldDraw) return;
           // Console.WriteLine("Which frame: " + whichLine);
            spriteBatch.Draw(texture, position,
                new Rectangle(whichLine * frameWidth, whichRow * frameHeight, frameWidth, frameHeight), Color.White);
            Debug_DrawBounds(graphicsDevice, spriteBatch);
        }
    }
}