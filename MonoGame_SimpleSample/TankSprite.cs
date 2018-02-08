using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;


namespace MonoGame_SimpleSample
{
    class TankSprite : Sprite
    {
        int boxSize;
        public int playerNumber;
        double fireTime = 0;
        double fireTimeMax = 500;
        WalkingDirection currentWalkingDirection = WalkingDirection.down;
        bool isMoving = false;
        bool isFiring = false;
        TankKeyMap keyMap;
        public Vector2 startingPosition;
        public int score = 0;
        private Vector2 screenSize;

        TankActionListener tankActionListener;

        public TankSprite(TankKeyMap keyMap, Texture2D texture, Vector2 startingPosition, int playerNumber,
            TankActionListener tankActionListener, Vector2 screenSize) : base(texture, startingPosition)
        {
            this.screenSize = screenSize;
            this.startingPosition = startingPosition;
            this.keyMap = keyMap;
            boxSize = Math.Max(frameWidth, frameHeight);
            this.playerNumber = playerNumber;
            base.frameHeight = boxSize;
            base.frameWidth = boxSize;
            this.tankActionListener = tankActionListener;
            effects = SpriteEffects.FlipVertically;
        }


        new public void Update(GameTime gameTime, List<Sprite> level)
        {
            updateInput();
            updateFiring(gameTime);
            updateMovement(gameTime, level);
            base.Update(gameTime);
        }

        void updateFiring(GameTime gameTime)
        {
            fireTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (fireTime >= fireTimeMax && isFiring)
            {

                Vector2 bulletPosition = new Vector2(position.X+texture.Width/2, position.Y+5); 
                if (currentWalkingDirection == WalkingDirection.down)
                {
                    bulletPosition = new Vector2(position.X+texture.Width/2 - 6, position.Y+5);
                }
                if (currentWalkingDirection == WalkingDirection.up)
                {
                    bulletPosition = new Vector2(position.X+texture.Width/2 - 6, position.Y+5);
                }
                if (currentWalkingDirection == WalkingDirection.left)
                {
                    bulletPosition = new Vector2(position.X-5, position.Y+texture.Height/2 -13);
                }
                if (currentWalkingDirection == WalkingDirection.right)
                {
                    bulletPosition = new Vector2(position.X+5, position.Y+texture.Height/2 -13);
                }
               
                tankActionListener.OnFire(playerNumber, bulletPosition, currentWalkingDirection);
                fireTime = 0;
            }
        }

        void updateInput()
        {
            var keyboardState = Keyboard.GetState();
            var pressedKeys = keyboardState.GetPressedKeys();

            isMoving = false;
            isFiring = false;
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
                    else if (keyMap.fire.Equals(Key))
                    {
                        isFiring = true;
                    }
                }
            }
        }

        void updateMovement(GameTime gameTime, List<Sprite> level)
        {
            if (!isMoving)
                return;
            int pixelsPerSecond = 80;
            float movementSpeed = (float) (pixelsPerSecond * (gameTime.ElapsedGameTime.TotalSeconds));
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

            var tempPosition = position + movementVector;
            var temp = new List<Triangle>();
            foreach (var triangle in initialCollisionTriangles)
            {
                var aaa = RotateVector(triangle.A + tempPosition, tempPosition + origin, rotation);
                var bbb = RotateVector(triangle.B + tempPosition, tempPosition + origin, rotation);
                var ccc = RotateVector(triangle.C + tempPosition, tempPosition + origin, rotation);
                temp.Add(new Triangle(aaa, bbb, ccc));
            }
            
            Boolean colided = false;
            foreach (var item in level)
            {
                if (isIntersectingWith(item, initialCollisionTriangles, temp, item.collisionTriangles) ||
                    item.isIntersectingWith(this, item.initialCollisionTriangles, item.collisionTriangles, temp) ||
                    reachedMapBoundaries(tempPosition))
                {
                    colided = true;
                    break;
                   
                }
            }

            if (!colided )
            {
                position = tempPosition;
            }
        }

        private bool reachedMapBoundaries(Vector2 tempPosition)
        {
            return tempPosition.X < 0 || tempPosition.Y < 0 || tempPosition.X > screenSize.X - texture.Width ||
                   tempPosition.Y > screenSize.Y - texture.Height;        }

        new public void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            rotation = getRotation();

            base.Draw(graphicsDevice, spriteBatch);
        }

        public float getRotation()
        {
            switch (currentWalkingDirection)
            {
                case WalkingDirection.right:
                    return (float)Math.PI/2 * (int) WalkingDirection.left;
                case WalkingDirection.left:
                    return (float)Math.PI/2 * (int) WalkingDirection.right;
                default:
                    return (float)Math.PI/2 * (int) currentWalkingDirection;
            }
        }
    }
}