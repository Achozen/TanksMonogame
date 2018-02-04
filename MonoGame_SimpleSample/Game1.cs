using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MonoGame_SimpleSample
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D playerTexture;
        TankSprite playerSprite;
        TankSprite playerSprite2;

        GameState currentGameState = GameState.playing;
        bool isPauseKeyHeld = false;
        string collisionText = "";
        SpriteFont HUDFont;
        List<Sprite> Level;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 500;
            graphics.PreferredBackBufferWidth = 1000;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Level = new List<Sprite>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            playerTexture = Content.Load<Texture2D>("Default size/tank_green");
            var lines = System.IO.File.ReadAllLines(@"Content/Level1.txt");
            foreach (var line in lines)
            {
                var data = line.Split(';');
                Texture2D tempTexture = Content.Load<Texture2D>(data[0]);
                Vector2 tempPos = new Vector2(int.Parse(data[1]), int.Parse(data[2]));
                Level.Add(new Sprite(tempTexture, tempPos));
            }

            playerSprite = new TankSprite(playerTexture, Vector2.Zero, 1);
            playerSprite.Position = new Vector2(0, graphics.PreferredBackBufferHeight - ((playerSprite.BoundingBox.Max.Y - playerSprite.BoundingBox.Min.Y) + 30));

            playerSprite2 = new TankSprite(playerTexture, Vector2.Zero, 2);
            playerSprite2.Position = new Vector2(0, graphics.PreferredBackBufferHeight - ((playerSprite2.BoundingBox.Max.Y - playerSprite2.BoundingBox.Min.Y) + 30));
            HUDFont = Content.Load<SpriteFont>("HUDFont");
        }
        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.P) && !isPauseKeyHeld)
            {
                if (currentGameState == GameState.playing)
                    currentGameState = GameState.paused;
                else currentGameState = GameState.playing;
            }
            isPauseKeyHeld = keyboardState.IsKeyUp(Keys.P) ? false : true;
            switch (currentGameState) {
                case GameState.playing:
                    { 
                    {
                        foreach (var sprite in Level)
                        {
                            sprite.Update(gameTime);
                        }

                        playerSprite.Update(gameTime);
                        playerSprite2.Update(gameTime);

                        collisionText = "there is no collision";


                        foreach (var sprite in Level)
                        {
                            if (playerSprite.IsCollidingWith(sprite) || playerSprite2.IsCollidingWith(sprite))
                                collisionText = "there is a collision";
                            break;
                        }
                    }
            }
            break;
                case GameState.paused:
                break;

        } 
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            switch (currentGameState)
            {
                case GameState.playing:
                    {
                        foreach (var sprite in Level)
                        {
                            sprite.Draw(GraphicsDevice, spriteBatch);
                        }
                        playerSprite.Draw(GraphicsDevice, spriteBatch);
                        playerSprite2.Draw(GraphicsDevice, spriteBatch);
                        spriteBatch.DrawString(HUDFont, collisionText, new Vector2(300, 0), Color.Red);
                    }
                    break;
                case GameState.paused:
                    {
                        spriteBatch.DrawString(HUDFont, "Game Paused", Vector2.Zero, Color.White);

                    }
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
