using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MonoGame_SimpleSample
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
	enum GameState 
	{
		playing,
		paused
	}
	
	
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D playerTexture;
        AnimatedSprite playerSprite;
        
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

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Level = new List<Sprite>();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            playerTexture = Content.Load<Texture2D>("tank");

            var lines = System.IO.File.ReadAllLines(@"Content/Level1.txt");
            foreach(var line in lines)
            {
                var data = line.Split(';');

                Texture2D tempTexture = Content.Load<Texture2D>(data[0]);
                Vector2 tempPos = new Vector2(int.Parse(data[1]), int.Parse(data[2]));
                Level.Add(new Sprite(tempTexture, tempPos));

            }

            playerSprite = new AnimatedSprite(playerTexture, Vector2.Zero , 4, 9);
            playerSprite.Position = new Vector2(0, graphics.PreferredBackBufferHeight - ((playerSprite.BoundingBox.Max.Y - playerSprite.BoundingBox.Min.Y) + 30));
            HUDFont = Content.Load<SpriteFont>("HUDFont");


            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var keyboardState = Keyboard.GetState();


            if( keyboardState.IsKeyDown(Keys.P) && !isPauseKeyHeld)
            {

                if (currentGameState == GameState.playing)
                        currentGameState = GameState.paused;
                else currentGameState = GameState.playing;
            }


            //This should be in the Input Manager - differentiate between pressed and held
            isPauseKeyHeld = keyboardState.IsKeyUp(Keys.P) ? false : true;



            // TODO: Add your update logic here
            switch (currentGameState)
			{
				case GameState.playing:
				{

                    //Update Level
                    foreach(var sprite in Level)
                    {
                        sprite.Update(gameTime);
                    }

                    playerSprite.Update(gameTime);
                        //check collisions


                        //playerSprite.isFalling = true;


                        collisionText = "there is no collision";


                    foreach (var sprite in Level)
                    {
                            if (playerSprite.IsCollidingWith(sprite))
                                collisionText = "there is a collision";
                    }
                }
				break;
				
				case GameState.paused:
				{
					
				}
				
				break;
				
			}

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();


			
			switch (currentGameState)
			{
				case GameState.playing:
				{

                        //draw the ground

                        foreach (var sprite in Level)
                        {
                            sprite.Draw(GraphicsDevice, spriteBatch);
                        }



                        playerSprite.Draw(GraphicsDevice, spriteBatch);
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
