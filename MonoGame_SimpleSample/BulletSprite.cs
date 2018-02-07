using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


namespace MonoGame_SimpleSample
{
    class BulletSprite : Sprite
    {
        int boxSize;
        WalkingDirection direction;
        double currentFrameTime;
        double expectedFrameTime = 200.0f;

        public BulletSprite(Texture2D texture, Vector2 startingPosition, WalkingDirection direction) : base(texture,
            startingPosition)
        {
            boxSize = Math.Max(frameWidth, frameHeight);
            this.direction = direction;
            frameHeight = boxSize;
            frameWidth = boxSize;
            boundingBox = new BoundingBox(new Vector3(position.X, position.Y, 0),
                new Vector3(position.X + boxSize, position.Y + boxSize, 0));
        }


        new public void Update(GameTime gameTime)
        {
            currentFrameTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (currentFrameTime >= expectedFrameTime)
            {
                currentFrameTime = 0;
            }

            updateMovement(gameTime);
            updateBoundingBoxes();
        }

        void updateMovement(GameTime gameTime)
        {
            int pixelsPerSecond = 200;
            float movementSpeed = (float) (pixelsPerSecond * (gameTime.ElapsedGameTime.TotalSeconds));

            Vector2 movementVector = Vector2.Zero;

            switch (direction)
            {
                case WalkingDirection.left:
                {
                    movementVector = new Vector2(-movementSpeed, 0);
                    break;
                }

                case WalkingDirection.right:
                {
                    movementVector = new Vector2(movementSpeed, 0);
                    break;
                }
                case WalkingDirection.up:
                {
                    movementVector = new Vector2(0, -movementSpeed);
                    break;
                }
                case WalkingDirection.down:
                {
                    movementVector = new Vector2(0, movementSpeed);
                    break;
                }
            }

            position += movementVector;
        }

        public float getRotation()
        {
            switch (direction)
            {
                case WalkingDirection.right:
                    return 1.57f * (int) WalkingDirection.left;
                case WalkingDirection.left:
                    return 1.57f * (int) WalkingDirection.right;
                default:
                    return 1.57f * (int) direction;
            }
        }
    }
}