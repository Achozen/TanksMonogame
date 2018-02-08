using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MonoGame_SimpleSample
{
    
    class MouseStateComponent
    {
        private MouseState _lastMouseState;
        private MouseState _currentMouseState;
        private bool _leftClickOccurred;
        private bool _rightClickOccurred;

        public void Update()
        {
            _lastMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();
            _leftClickOccurred = _lastMouseState.LeftButton == ButtonState.Released && _currentMouseState.LeftButton == ButtonState.Pressed;
            _rightClickOccurred = _lastMouseState.RightButton == ButtonState.Released && _currentMouseState.RightButton == ButtonState.Pressed;
        }

        public void OnLeftClick(Action<MouseState> handler)
        {
            if(_leftClickOccurred)
                handler.Invoke(_currentMouseState);
        }

        public void OnRightClick(Action<MouseState> handler)
        {
            if(_rightClickOccurred)
                handler.Invoke(_currentMouseState);
        }

        public void CleanUp()
        {
            _leftClickOccurred = false;
            _rightClickOccurred = false;
        }
    }

    class KeyStateComponent
    {
        private KeyboardState _lastKeyboardState;
        private KeyboardState _currentKeyboardState;
        private Dictionary<Keys, bool> _occured = new Dictionary<Keys, bool>();

        public void Update()
        {
            _lastKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();
            

            foreach (var Key in _currentKeyboardState.GetPressedKeys())
            {
                _occured.Add(Key, _lastKeyboardState.IsKeyUp(Key) && _currentKeyboardState.IsKeyDown(Key));
            }
        }

        public void OnKey(Keys key, Action consume)
        {
            if (_occured.ContainsKey(key) && _occured[key])
            {
                consume.Invoke();
            }
        }
        
        public void CleanUp()
        {
            _occured.Clear();
        }
        
    }
    
    public class Game1 : Game, TankActionListener
    {
        public static bool DEBUG = true;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D playerTexture;
        Texture2D bulletTexture;
        Texture2D explosionTexture;
        TankSprite playerSprite;
        TankSprite playerSprite2;

        GameState currentGameState = GameState.playing;
        bool isPauseKeyHeld;
        string scoreText = "";
        SpriteFont HUDFont;
        List<Sprite> Level;
        List<Sprite> MapEditorItems;
        List<Texture2D> MapEditorAvailableItems;
        List<BulletSprite> bulllets;
        List<AnimatedSprite> explosions;

        Texture2D rect;
        int lastScrollWheelValue;
        int indexer;
        
        private SoundEffect explosionSound;
        private Song backgroundMusic;
        private MouseStateComponent _mouseStateComponent = new MouseStateComponent();
        private KeyStateComponent _keyStateComponent = new KeyStateComponent();
        private int mapEditorBrushX = 1;
        private int mapEditorBrushY = 1;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 500;
            graphics.PreferredBackBufferWidth = 1000;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            MapEditorItems = new List<Sprite>();
            MapEditorAvailableItems = new List<Texture2D>();
            Level = new List<Sprite>();
            bulllets = new List<BulletSprite>();
            explosions = new List<AnimatedSprite>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            bulletTexture = Content.Load<Texture2D>("Bullets/bulletBeige");
            playerTexture = Content.Load<Texture2D>("Default size/tank_green");
            explosionSound = Content.Load<SoundEffect>("SoundFX/explosion");
 /*           backgroundMusic = Content.Load<Song>("SoundFX/background_music");
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.Volume = 0.1f;*/

            explosionTexture = Content.Load<Texture2D>("explosion/exp2_0");
            var lines = File.ReadAllLines(@"Content/Level1.txt");
            foreach (var line in lines)
            {
                var data = line.Split(';');
                Texture2D tempTexture = Content.Load<Texture2D>(data[0]);
                Vector2 tempPos = new Vector2(float.Parse(data[1]), float.Parse(data[2]));

                if (data.Length == 5 && !data[4].Equals("auto") && !data[4].Equals("none"))
                {
                        var collisionPoints = data[4].Split(',');
                        var collisionTriangles = new List<Triangle>();
                        for (var i = 0; i < collisionPoints.Length; i += 6)
                        {
                            collisionTriangles.Add(new Triangle(
                                    new Vector2(float.Parse(collisionPoints[i]), float.Parse(collisionPoints[i + 1])),
                                    new Vector2(float.Parse(collisionPoints[i + 2]),
                                        float.Parse(collisionPoints[i + 3])),
                                    new Vector2(float.Parse(collisionPoints[i + 4]),
                                        float.Parse(collisionPoints[i + 5]))
                                )
                            );
                        }

                        Level.Add(new Sprite(tempTexture, tempPos, float.Parse(data[3]), collisionTriangles));
                 }
                else if(data.Length == 5 && data[4].Equals("auto"))
                {    
                    Level.Add(new Sprite(tempTexture, tempPos, float.Parse(data[3])));
                }
                else
                {
                    Level.Add(new Sprite(tempTexture, tempPos, float.Parse(data[3]), null));
                }

            }

            var player1Keys =
                new TankKeyMap {up = Keys.W, down = Keys.S, left = Keys.A, right = Keys.D, fire = Keys.Space};
            playerSprite = new TankSprite(player1Keys, playerTexture, new Vector2(0, 0), 1, this, new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));

            var player2Keys = new TankKeyMap
            {
                up = Keys.Up,
                down = Keys.Down,
                left = Keys.Left,
                right = Keys.Right,
                fire = Keys.Enter
            };
            playerSprite2 = new TankSprite(player2Keys, playerTexture, new Vector2(graphics.PreferredBackBufferWidth - playerTexture.Width,
                graphics.PreferredBackBufferHeight - playerTexture.Height), 2, this, new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));


            HUDFont = Content.Load<SpriteFont>("HUDFont");
            playerSprite.font = HUDFont;
            playerSprite2.font = HUDFont;

            foreach (var sprite in Level)
            {
                sprite.font = HUDFont;
            }

            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Obstacles/barrelGreen_up"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Obstacles/barrelRed_up"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Obstacles/oil"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Obstacles/sandbagBeige"));    
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Obstacles/sandbagBrown"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Environment/treeLarge"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Environment/treeSmall"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Environment/grass"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Environment/sand"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Environment/dirt"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/barrelBlack_top"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/barrelGreen_top"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/barrelRed_top"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/barrelRust_top"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/barricadeMetal"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/fenceRed"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/fenceYellow"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/barricadeWood"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/oilSpill_large"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/sandbagBeige"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/sandbagBeige_open"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass1"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/barricadeWood"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_roadCornerLL"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_roadCornerLR"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_roadCornerUL"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_roadCornerUR"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_roadCrossing"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_roadCrossingRound"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_roadEast"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_roadNorth"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_roadSplitE"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_roadSplitN"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_roadSplitS"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_roadSplitW"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_roadTransitionE"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_roadTransitionE_dirt"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_roadTransitionN"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_roadTransitionN_dirt"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_roadTransitionS"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_roadTransitionS_dirt"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_roadTransitionW"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_roadTransitionW_dirt"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_transitionE"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_transitionN"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_transitionS"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass_transitionW"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass1"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileGrass2"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileSand_roadCornerLL"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileSand_roadCornerLR"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileSand_roadCornerUL"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileSand_roadCornerUR"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileSand_roadCrossing"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileSand_roadCrossingRound"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileSand_roadEast"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileSand_roadNorth"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileSand_roadSplitE"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileSand_roadSplitN"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileSand_roadSplitS"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileSand_roadSplitW"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileSand1"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tileSand2"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tracksDouble"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tracksLarge"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Default size/tracksSmall"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Tanks/barrelRed"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Tanks/barrelRed_outline"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Tanks/barrelBeige"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Tanks/barrelBeige_outline"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Tanks/barrelBlack"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Tanks/barrelBlack_outline"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Tanks/barrelBlue"));
            MapEditorAvailableItems.Add(Content.Load<Texture2D>("Tanks/barrelBlue_outline"));



        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.P) && !isPauseKeyHeld)
            {
                if (currentGameState == GameState.mapEditor)
                {
                    Level = new List<Sprite>(MapEditorItems);
                    MapEditorItems.Clear();
                }

                if (currentGameState == GameState.playing)
                    currentGameState = GameState.paused;
                else currentGameState = GameState.playing;
            }

            if (keyboardState.IsKeyDown(Keys.M))
            {
                currentGameState = GameState.mapEditor;
                MapEditorItems = new List<Sprite>(Level);
            }

            if (keyboardState.IsKeyDown(Keys.F11))
            {
                File.WriteAllText(@"Content/Level1.txt",string.Empty);
                foreach (var sprite in MapEditorItems)
                {
                    File.AppendAllText(@"Content/Level1.txt", sprite.toLevelFormat() + Environment.NewLine);
                }
            }

            isPauseKeyHeld = !keyboardState.IsKeyUp(Keys.P);
            switch (currentGameState)
            {
                case GameState.playing:
                {
                    foreach (var sprite in Level)
                    {
                        sprite.Update(gameTime);
                    }

                    foreach (var sprite in explosions)
                    {
                        sprite.Update(gameTime);
                    }

                    playerSprite.Update(gameTime, Level);
                    playerSprite2.Update(gameTime, Level);

                    bulletsToLeveCollision(gameTime);
                    handleTanksFight();
                    break;
                }
                case GameState.paused:
                    break;
                case GameState.mapEditor:

                    if (Mouse.GetState().ScrollWheelValue - lastScrollWheelValue > 0 &&
                        Keyboard.GetState().IsKeyUp(Keys.LeftControl))
                    {
                        indexer = nextIndexer(indexer);
                    }
                    else if (lastScrollWheelValue - Mouse.GetState().ScrollWheelValue > 0 &&
                             Keyboard.GetState().IsKeyUp(Keys.LeftControl))
                    {
                        indexer = prevIndexer(indexer);
                    }

                    if (Mouse.GetState().ScrollWheelValue - lastScrollWheelValue > 0 &&
                        Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                        {
                            degrees -= 15;
                        }
                        else
                        {
                            degrees--;
                        }
                    }
                    else if (lastScrollWheelValue - Mouse.GetState().ScrollWheelValue > 0 &&
                             Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                        {
                            degrees += 15;
                        }
                        else
                        {
                            degrees++;
                        }
                    }

                    lastScrollWheelValue = Mouse.GetState().ScrollWheelValue;
                    break;
            }

            base.Update(gameTime);
        }

        private void handleTanksFight()
        {
            foreach (var bullet in bulllets)
            {
                CheckColisionForPlayer(bullet, playerSprite, playerSprite2);
                CheckColisionForPlayer(bullet, playerSprite2, playerSprite);
            }
            scoreText = "Player1 - "+ playerSprite.score + " | "+ playerSprite2.score + " - Player2";
        }

        private void CheckColisionForPlayer(BulletSprite bullet, TankSprite player, TankSprite enemy)
        {
            if (bullet.shouldDraw && bullet.IsCollidingWith(player) && bullet.bulletOwner != player.playerNumber)
            {
                var explosion = new AnimatedSprite(explosionTexture, 
                    new Vector2(player.position.X, player.position.Y), 4, 4);

                player.position = player.startingPosition;
                enemy.score++;
                explosions.Add(explosion);
                bullet.shouldDraw = false;
                explosionSound.Play();
            }
        }

        private void bulletsToLeveCollision(GameTime gameTime)
        {
            var toBeDeleted = new List<BulletSprite>();
            var toBeDeletedFromLevel = new List<Sprite>();
            foreach (var sprite in bulllets)
            {
                sprite.Update(gameTime);
                foreach (var level in Level)
                {
                    if (sprite.shouldDraw && level.shouldDraw && sprite.IsCollidingWith(level))
                    {
                        var explosion = new AnimatedSprite(explosionTexture,
                            new Vector2(sprite.position.X, sprite.position.Y), 4, 4);
                        explosions.Add(explosion);
                        explosionSound.Play();
                        toBeDeleted.Add(sprite);
                        toBeDeletedFromLevel.Add(level);// = false;
                    }
                }
            }

            foreach (var el in toBeDeleted)
            {
                bulllets.Remove(el);
            }
            toBeDeleted.Clear();
            foreach (var el in toBeDeletedFromLevel)
            {
                Level.Remove(el);
            }
            toBeDeletedFromLevel.Clear();
        }

        protected override void Draw(GameTime gameTime)
        {
            _mouseStateComponent.Update();
            _keyStateComponent.Update();
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            _keyStateComponent.OnKey(Keys.F1, () => DEBUG = !DEBUG);
            switch (currentGameState)
            {
                case GameState.playing:
                {
                    foreach (var sprite in Level)
                    {
                        sprite.Draw(GraphicsDevice, spriteBatch);
                    }

                    foreach (var sprite in bulllets)
                    {
                        sprite.Draw(GraphicsDevice, spriteBatch);
                    }

                    foreach (var sprite in explosions)
                    {
                        sprite.Draw(GraphicsDevice, spriteBatch);
                    }

                    playerSprite.Draw(GraphicsDevice, spriteBatch);
                    playerSprite2.Draw(GraphicsDevice, spriteBatch);
                    spriteBatch.DrawString(HUDFont, scoreText, new Vector2(300, 0), Color.Red);
                }
                    break;
                case GameState.paused:
                {
                    spriteBatch.DrawString(HUDFont, "Game Paused", Vector2.Zero, Color.White);
                }
                    break;
                case GameState.mapEditor:
                    spriteBatch.DrawString(HUDFont, "Map editor", Vector2.Zero, Color.White);
                    _keyStateComponent.OnKey(Keys.Right, () => mapEditorBrushX ++);
                    _keyStateComponent.OnKey(Keys.Left, () => mapEditorBrushX = Math.Max(1, mapEditorBrushX - 1));
                    _keyStateComponent.OnKey(Keys.Down, () => mapEditorBrushY ++);
                    _keyStateComponent.OnKey(Keys.Up, () => mapEditorBrushY = Math.Max(1, mapEditorBrushY - 1));
                    _keyStateComponent.OnKey(Keys.PageUp, () => indexer = nextIndexer(indexer));
                    _keyStateComponent.OnKey(Keys.PageDown, () => indexer = prevIndexer(indexer));
                    _keyStateComponent.OnKey(Keys.Z, () =>
                    {
                        if(MapEditorItems.Count != 0)
                            MapEditorItems.RemoveAt(MapEditorItems.Count - 1);
                    });
                    var texm1 = MapEditorAvailableItems[prevIndexer(prevIndexer(indexer))];
                    var tex = MapEditorAvailableItems[prevIndexer(indexer)];
                    var tex2 = MapEditorAvailableItems[indexer];
                    var tex3 = MapEditorAvailableItems[nextIndexer(indexer)];
                    var tex4 = MapEditorAvailableItems[nextIndexer(nextIndexer(indexer))];
                    Vector2 currentItemVector;
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        currentItemVector=  new Vector2(((Mouse.GetState().X + tex2.Width / 2) / tex2.Width)*tex2.Width,
                            (Mouse.GetState().Y + tex2.Height / 2) / tex2.Height*tex2.Height );
                    }
                    else
                    {
                        currentItemVector=  new Vector2(Mouse.GetState().X, Mouse.GetState().Y );
                    }

                    _mouseStateComponent.OnLeftClick(state =>
                    {
                        for (var i = 0; i < mapEditorBrushX; i++)
                        {
                            for (var j = 0; j < mapEditorBrushY; j++)
                            {
                                MapEditorItems.Add(new Sprite(tex2,(currentItemVector- new Vector2( tex2.Width/2, tex2.Height/2))+new Vector2(i*tex2.Width, j*tex2.Height), (float) degrees));
                            }
                        }
                    });

                    _mouseStateComponent.OnRightClick(state =>
                    {
                        for (var i = 0; i < mapEditorBrushX; i++)
                        {
                            for (var j = 0; j < mapEditorBrushY; j++)
                            {
                                MapEditorItems.Add(new Sprite(tex2,currentItemVector- new Vector2( tex2.Width/2, tex2.Height/2)+new Vector2(i*tex2.Width, j*tex2.Height), (float) degrees, null));
                            }
                        }
                    });
                    


                    foreach (var sprite in Level)
                    {
                        sprite.Draw(GraphicsDevice, spriteBatch);
                    }

                    foreach (var sprite in MapEditorItems)
                    {
                        sprite.Draw(GraphicsDevice, spriteBatch);
                    }
                    foreach (var sprite in bulllets)
                    {
                        sprite.Draw(GraphicsDevice, spriteBatch);
                    }

                    spriteBatch.Draw(texm1,
                        new Rectangle(Mouse.GetState().X, Mouse.GetState().Y - texm1.Height - tex.Height, texm1.Width,
                            texm1.Height), null, new Color(255, 255, 255, 180));
                    spriteBatch.Draw(tex,
                        new Rectangle(Mouse.GetState().X, Mouse.GetState().Y - tex.Height, tex.Width, tex.Height), null,
                        new Color(255, 255, 255, 200));
                    //spriteBatch.Draw(tex2, new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, tex2.Width, tex2.Height), null, Color.White);

                    
                    for (var i = 0; i < mapEditorBrushX; i++)
                    {
                        for (var j = 0; j < mapEditorBrushY; j++)
                        {
                            var XD = new Vector2();
                            spriteBatch.Draw(tex2,
                                new Rectangle(
                                    (int)currentItemVector.X+tex2.Width * i,
                                    (int)currentItemVector.Y+tex2.Height * j,
                                    tex2.Width, tex2.Height), null, Color.White, (float) DegreeToRadian(degrees),
                                new Vector2(tex2.Width / 2, tex2.Height / 2), SpriteEffects.None, 0f);
                        }
                    }
                    
                    
                    spriteBatch.Draw(tex3,
                        new Rectangle(Mouse.GetState().X, Mouse.GetState().Y + tex2.Height, tex3.Width, tex3.Height),
                        null, new Color(255, 255, 255, 200));
                    spriteBatch.Draw(tex4,
                        new Rectangle(Mouse.GetState().X, Mouse.GetState().Y + tex2.Height + tex3.Height, tex4.Width,
                            tex4.Height), null, new Color(255, 255, 255, 180));

                    // spriteBatch.DrawString(HUDFont, "Mouse:" + Mouse.GetState()+ "indekser: "+ (indexer), new Vector2(100, 300), Color.Red);
                    playerSprite.Draw(GraphicsDevice, spriteBatch);
                    playerSprite2.Draw(GraphicsDevice, spriteBatch);
                    spriteBatch.DrawString(HUDFont, scoreText, new Vector2(300, 0), Color.Red);


                    if (rect == null)
                        rect = new Texture2D(GraphicsDevice, 1, 1);
                    rect.SetData(new[] {Color.White});
                    spriteBatch.Draw(rect, new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 10, 10), Color.Black);
                    spriteBatch.Draw(rect, new Rectangle(Mouse.GetState().X + 1, Mouse.GetState().Y + 1, 8, 8),
                        Color.White);
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
            
            _mouseStateComponent.CleanUp();
            _keyStateComponent.CleanUp();
        }

        double degrees = 0f;

        double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        int nextIndexer(int current)
        {
            var ind = current;
            ind++;
            if (ind >= MapEditorAvailableItems.Count) ind = 0;
            return ind;
        }

        int prevIndexer(int current)
        {
            var ind = current;
            ind--;
            if (ind < 0) ind = MapEditorAvailableItems.Count - 1;
            return ind;
        }

        public void OnFire(int playerIndex, Vector2 position, WalkingDirection walkingDirection)
        {
            var bullet = new BulletSprite(bulletTexture, position, walkingDirection, playerIndex);
            bullet.font = HUDFont;
            bulllets.Add(bullet);
        }
    }
}