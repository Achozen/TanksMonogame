using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


namespace MonoGame_SimpleSample
{
    class BulletSprite : Sprite
    {
        WalkingDirection direction;
        double currentFrameTime;
        double expectedFrameTime = 200.0f;
        public int bulletOwner;

        public BulletSprite(Texture2D texture, Vector2 startingPosition, WalkingDirection direction, int bulletOwner) : base(texture,
            startingPosition)
        {
            this.bulletOwner = bulletOwner;
            this.direction = direction;
            frameHeight = texture.Height;
            frameWidth = texture.Width;
        }


        new public void Update(GameTime gameTime)
        {
            currentFrameTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (currentFrameTime >= expectedFrameTime)
            {
                currentFrameTime = 0;
            }

            updateMovement(gameTime);
        }

        void updateMovement(GameTime gameTime)
        {
            rotation = getRotation();
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
                    return (float)Math.PI/2 * (int) WalkingDirection.left;
                case WalkingDirection.left:
                    return (float)Math.PI/2 * (int) WalkingDirection.right;
                default:
                    return (float)Math.PI/2 * (int) direction;
            }
        }
    }
}