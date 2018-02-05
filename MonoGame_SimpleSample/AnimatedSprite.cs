using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MonoGame_SimpleSample
{
    enum WalkingDirection
    {
        up = 0,
        left = 1,
        down = 2,
        right = 3,
        idle = 4
    }

    class TankKeyMap
    {
        public Keys up { get; set; }
        public Keys down { get; set; }
        public Keys left { get; set; }
        public Keys right { get; set; }
    }

    class TankSprite : Sprite
    {
        int boxSize;
        int playerNumber;

        double currentFrameTime = 0;
        double expectedFrameTime = 200.0f;
        WalkingDirection currentWalkingDirection = WalkingDirection.down;
        bool isMoving = false;
        TankKeyMap keyMap;

        public TankSprite(TankKeyMap keyMap, Texture2D texture, Vector2 startingPosition, int playerNumber) : base(texture, startingPosition)
        {
            this.keyMap = keyMap;
            boxSize = Math.Max(frameWidth, frameHeight);
            this.playerNumber = playerNumber;
            base.frameHeight = boxSize;
            base.frameWidth = boxSize;

            boundingBox = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + boxSize, position.Y + boxSize, 0));
        }


        new public void Update(GameTime gameTime)
        {
            currentFrameTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (currentFrameTime >= expectedFrameTime)
            {
                currentFrameTime = 0;
            }

            updateInput();
            if (isMoving)
            {
                updateMovement(gameTime);
                base.updateBoundingBoxes();
            }
        }

        void updateInput()
        {
            var keyboardState = Keyboard.GetState();
            var pressedKeys = keyboardState.GetPressedKeys();

            isMoving = false;
            if (pressedKeys.Length != 0)
            {
                foreach (var Key in pressedKeys)
                {
                    if (keyMap.left.Equals(Key))
                    {
                        currentWalkingDirection = WalkingDirection.left;
                        isMoving = true;
                    }
                    else if (keyMap.right.Equals(Key))
                    {
                        currentWalkingDirection = WalkingDirection.right;
                        isMoving = true;
                    }
                    else if (keyMap.down.Equals(Key))
                    {
                        currentWalkingDirection = WalkingDirection.down;
                        isMoving = true;
                    }
                    else if (keyMap.up.Equals(Key))
                    {
                        currentWalkingDirection = WalkingDirection.up;
                        isMoving = true;
                    }
                }
            }
        }

        void updateMovement(GameTime gameTime)
        {
            int pixelsPerSecond = 80;
            float movementSpeed = (float)(pixelsPerSecond * (gameTime.ElapsedGameTime.TotalSeconds));
            Vector2 movementVector = Vector2.Zero;
            switch (currentWalkingDirection)
            {
                case WalkingDirection.left:
                        movementVector = new Vector2(-movementSpeed, 0);
                        break;
                case WalkingDirection.right:
                        movementVector = new Vector2(movementSpeed, 0);
                        break;
                case WalkingDirection.up:
                        movementVector = new Vector2(0, -movementSpeed);
                        break;
                case WalkingDirection.down:
                        movementVector = new Vector2(0, movementSpeed);
                        break;
            }
            position += movementVector;
        }

        new public void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch) {
            float rotation = getRotation();

            var origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            spriteBatch.Draw(texture, position + new Vector2(boxSize / 2, boxSize / 2), new Rectangle(0, 0, boxSize, boxSize), Color.White, rotation, origin, 1f, SpriteEffects.FlipVertically, 0f);

            Debug_DrawBounds(graphicsDevice, spriteBatch);
        }

        public float getRotation()
        {
            switch (currentWalkingDirection) {
                case WalkingDirection.right:
                    return 1.57f * (int)WalkingDirection.left;
                case WalkingDirection.left:
                    return 1.57f * (int)WalkingDirection.right;
                default:
                    return 1.57f * (int)currentWalkingDirection;

            }
        }

        new public bool IsCollidingWith(Sprite otherSprite)
        {
            //collision top - bottom -> stop the gravity momentum
            if (this.bottomBoundingBox.Intersects(otherSprite.TopBoundingBox))
            {
                ///
            }
            //collsion left/right -> stop the left/right momentum
            if (this.leftBoundingBox.Intersects(otherSprite.RightBoundingBox) || this.rightBoundingBox.Intersects(otherSprite.RightBoundingBox))
            {
                //TODO: FInish this code
            }

            return this.boundingBox.Intersects(otherSprite.BoundingBox) ? true : false;

        }
    }

    class BulletSprite : Sprite
    {
        int boxSize;
        WalkingDirection direction = WalkingDirection.idle;
        double currentFrameTime = 0;
        double expectedFrameTime = 200.0f;
        public BulletSprite(Texture2D texture, Vector2 startingPosition, WalkingDirection direction) : base(texture, startingPosition)
        {
            boxSize = Math.Max(frameWidth, frameHeight);
            this.direction = direction;
            base.frameHeight = boxSize;
            base.frameWidth = boxSize;

            boundingBox = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + boxSize, position.Y + boxSize, 0));
        }


        new public void Update(GameTime gameTime)
        {

            currentFrameTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (currentFrameTime >= expectedFrameTime)
            {
                currentFrameTime = 0;
            }

            updateMovement(gameTime);
            base.updateBoundingBoxes();
        }

        void updateMovement(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var pressedKeys = keyboardState.GetPressedKeys();

            int pixelsPerSecond = 80;
            float movementSpeed = (float)(pixelsPerSecond * (gameTime.ElapsedGameTime.TotalSeconds));

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

                default:
                    {
                        break;
                    }
            }
            position += movementVector;
        }

        new public void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            float rotation = getRotation();

            var origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            spriteBatch.Draw(texture, position + new Vector2(boxSize / 2, boxSize / 2), new Rectangle(0, 0, boxSize, boxSize), Color.White, rotation, origin, 1f, SpriteEffects.FlipVertically, 0f);

            Debug_DrawBounds(graphicsDevice, spriteBatch);
        }

        public float getRotation()
        {
            switch (direction)
            {
                case WalkingDirection.right:
                    return 1.57f * (int)WalkingDirection.left;
                case WalkingDirection.left:
                    return 1.57f * (int)WalkingDirection.right;
                default:
                    return 1.57f * (int)direction;

            }
        }

        new public bool IsCollidingWith(Sprite otherSprite)
        {
            //collision top - bottom -> stop the gravity momentum
            if (this.bottomBoundingBox.Intersects(otherSprite.TopBoundingBox))
            {
                ///
            }
            //collsion left/right -> stop the left/right momentum
            if (this.leftBoundingBox.Intersects(otherSprite.RightBoundingBox) || this.rightBoundingBox.Intersects(otherSprite.RightBoundingBox))
            {
                //TODO: FInish this code
            }

            return this.boundingBox.Intersects(otherSprite.BoundingBox) ? true : false;

        }
    }
}
