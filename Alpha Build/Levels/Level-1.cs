﻿using Alpha_Build.Entities;
using FarseerPhysics.Collision;
using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alpha_Build.Levels
{
    public class Level_1:Level
    {
        private Player _player;
        private SpriteBatch _spriteBatch;
        private Sprite _background;
        private Sprite _Missile;
        private Camera2D _camera;
        //private Listeners _listeners;
        private KeyboardState _previousState;
        private GamePadState _previousStateGamepad;
        public World _world;
        private const float _ratio = 10;
        private DebugView _debugView;
        private Border _border;
        private Invader _invaders;
        private BitmapFont _bitmapFont;
        private BitmapFont _bitmapFont2;
        private FramesPerSecondCounter _frames;
        private ColonyShip _colonyShip;
        public bool negativeForce { get; set; }
        public bool GameOver { get; set; }

        public Level_1(Game1 game) : base(game)
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            this.game = game;
            _previousState = Keyboard.GetState();
            _world = new World(Vector2.Zero); //There's zero gravity in outerspace.
            _frames = new FramesPerSecondCounter();
            
        }
        public Game1 game { get; private set; } //needed for loading contents in _player.cs

        protected override void Initialize()
        {
            base.Initialize();
            _camera = new Camera2D(GraphicsDevice);
            _camera.MaximumZoom = 1f;
            _camera.MinimumZoom = .5f;//.3f?
            _camera.Zoom = .8f;
            _debugView = new DebugView(_world, game,_ratio);
            IsMouseVisible = true;
            negativeForce = false;
            Pause = false;
            GameOver = false;
            #region Obsolete
            //_listeners = new Listeners();
            //_listeners.SetupCameraKeys(_camera);
            //_listeners.SetPlayerKeys()
            #endregion
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            /* Fonts */
            _bitmapFont = Content.Load<BitmapFont>("Fonts/montserrat-32");
            _bitmapFont2 = Content.Load<BitmapFont>("Fonts/enigma-42");

            /* Textures and create entities for the game through the classes*/
            _background = new Sprite(Content.Load<Texture2D>("Miscellaneous/Deep-Space-4K-Wallpaper"));
            _Missile = new Sprite(Content.Load<Texture2D>("Player/Missile")){ Scale = new Vector2(0.3f) };
            _player = new Player(game, _world, _ratio, 50f,new Vector2(40f, 30f), "Player/avion_0", "Player/avion_1", "Player/avion_2", "Player/avion_3", "Player/Missile_0", "Miscellaneous/CircleParticle"); //load textures and _player
            _border = new Border( _world);
            _camera.LookAt(_player.Position * _ratio);
            _colonyShip = new ColonyShip(game, _world, _ratio, "Passive/ColonyShip",_player);
            _colonyShip.CreateColonyShip(400, new Vector2(40f, 36f));
            _invaders = new Invader(game, _world, _ratio, "Enemy/EnemyShip", "Enemy/MissileEnemy_0", "Miscellaneous/CircleParticle", _colonyShip.colonyShipBreakableBody);
            //_invaders.CreateInvader(300, new Vector2(30f,40f) + _colonyShip.colonyShipBreakableBody.MainBody.Position, angle: MathHelper.ToRadians(240));
            _invaders.CreateInvader(300, new Vector2(80f * (float)Math.Cos(MathHelper.ToRadians(0 + 90)), -80f * (float)Math.Sin(MathHelper.ToRadians(0 + 90))) + _colonyShip.colonyShipBreakableBody.MainBody.Position, angle: MathHelper.PiOver2);
            _invaders.CreateInvader(300, new Vector2(80f * (float)Math.Cos(MathHelper.ToRadians(120 + 90)), -80f * (float)Math.Sin(MathHelper.ToRadians(120 + 90))) + _colonyShip.colonyShipBreakableBody.MainBody.Position, angle: MathHelper.ToRadians(-30));
            _invaders.CreateInvader(300, new Vector2(80f * (float)Math.Cos(MathHelper.ToRadians(-120 + 90)), -80f * (float)Math.Sin(MathHelper.ToRadians(-120 + 90))) + _colonyShip.colonyShipBreakableBody.MainBody.Position, angle: MathHelper.ToRadians(220));//Something's wrong with this.
            System.Diagnostics.Debug.WriteLine(new Vector2(80f * (float)Math.Cos(MathHelper.ToRadians(-120 + 90)), -80f * (float)Math.Sin(MathHelper.ToRadians(-120 + 90))) + _colonyShip.colonyShipBreakableBody.MainBody.Position + "position invader");
            _invaders.Practice = false;
        }

        protected override void UnloadContent(){}

        protected override void Update(GameTime gameTime)
        {
            if (_player.Broken == true || _colonyShip.Broken == true) GameOver = true;
            base.Update(gameTime);
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            #region Obsolete
            //_listeners.deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //_listeners.Update(gameTime);
            //_listeners._inputManager.Update(gameTime);
            //System.Diagnostics.Debug.WriteLine((float)gameTime.ElapsedGameTime.TotalSeconds);
            #endregion
            if (!Pause) _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f); //run farseer if game is not paused
            //_world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));
            KeyboardHandlerPlayer(Keyboard.GetState(), gameTime, _player.playerBreakableBody);
            KeyboardHandlerCamera(Keyboard.GetState(), gameTime);
            /* update individual entities */
            _player.Update(gameTime);
            _invaders.Update(gameTime);
            _colonyShip.Update(gameTime);
            if (!_player.Broken && !Pause)
            _camera.Move(_player.playerBreakableBody.MainBody.LinearVelocity / _ratio * .95f); //move camera with same velocity as player with parallax effect, yay..
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                GamePadController(GamePad.GetState(PlayerIndex.One), gameTime, _player.playerBreakableBody);
            }
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            var projection = Matrix.CreateOrthographicOffCenter(0f, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0f, 0f, 1f);
            var transformMatrix = _camera.GetViewMatrix();
            var view = Matrix.CreateScale(_ratio) * transformMatrix; //this'll scale the farseer bodies to the realtive meters/pixel values.


            //play with this
            //_spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(new Vector2(2f)));
            //_spriteBatch.Draw(_background);
            //_spriteBatch.End();

            _debugView.Draw(ref projection, ref view); //load the farseer debug view mode

            /* Main Draw functions */
            _spriteBatch.Begin(transformMatrix: transformMatrix);
            _player.Draw(_spriteBatch);
            _invaders.Draw(_spriteBatch);
            _colonyShip.Draw(_spriteBatch);
            _spriteBatch.End();

            /* Draw text */
            _spriteBatch.Begin();

            //_spriteBatch.DrawString(_bitmapFont, string.Format("Player position: {0:0}, {1:0}", _player.Position.X, _player.Position.Y), new Vector2(5, 5), Color.DarkBlue);

            if (GameOver)
            {
                //Pause = true;
                if (_player.Broken == true)
                {
                    _spriteBatch.DrawString(_bitmapFont, "Your ship was destroyed!", _camera.Origin + new Vector2(-170f, 0), Color.Silver);
                }
                if (_colonyShip.Broken == true)
                {
                    _spriteBatch.DrawString(_bitmapFont, "The Colony Ship was destroyed!", _camera.Origin + new Vector2(-200f, 40), Color.Silver);
                }
                if (gameTime.TotalGameTime.Milliseconds < 500)
                    _spriteBatch.DrawString(_bitmapFont2, "Game Over!", _camera.Origin + new Vector2(-100f, -100f), Color.Crimson);
                else if (gameTime.TotalGameTime.Milliseconds > 490)
                    _spriteBatch.DrawString(_bitmapFont2, "Game Over!", _camera.Origin + new Vector2(-100f, -100f), Color.LightGoldenrodYellow);
            }
            if (!GameOver && _invaders.invaderBreakableBody.Count == 0)
            {
                _spriteBatch.DrawString(_bitmapFont2, "You won!", _camera.Origin + new Vector2(-100f, -100f), Color.Crimson);

            }

            if (_player.Bullets == 0)
            {
                if (gameTime.TotalGameTime.Milliseconds < 500)
                    _spriteBatch.DrawString(_bitmapFont, "Reload!", new Vector2(GraphicsDevice.Viewport.Width - 120, GraphicsDevice.Viewport.Height - 30), Color.Yellow);
                else if (gameTime.TotalGameTime.Milliseconds > 490)
                    _spriteBatch.DrawString(_bitmapFont, "Reload!", new Vector2(GraphicsDevice.Viewport.Width - 120, GraphicsDevice.Viewport.Height - 30), Color.Red);
            }
            else
            {
                for (int i = 1; i <= _player.Bullets; i++)
                {
                    _Missile.Position = new Vector2(GraphicsDevice.Viewport.Width - i*25, GraphicsDevice.Viewport.Height - 10);
                    _spriteBatch.Draw(_Missile);
                    //System.Diagnostics.Debug.WriteLine(_Missile.Position + " --> positions");
                }
            }
                

            _spriteBatch.End();


            if (Pause) //pausing the game
            {
                if (!GameOver)
                {
                    _spriteBatch.Begin();
                    _spriteBatch.DrawString(_bitmapFont, "The game is Paused", _camera.Origin - new Vector2(100f, -30), Color.Yellow);
                    _spriteBatch.End();
                }
                
            }

            base.Draw(gameTime);
        }

        private void KeyboardHandlerCamera(KeyboardState keyboardState, GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // the camera properties of the camera can be conrolled to move, zoom and rotate
            const float movementSpeed = 200;
            //const float rotationSpeed = 0.5f; // no need for rotation
            const float zoomSpeed = 0.5f;
            //1028px to 1028px from center screen /* the first cases were used for a fixed screen game, but they haven't given me trouble in full screen mode */
            if (_camera.Position.Y > -668 && keyboardState.IsKeyDown(Keys.W))
                _camera.Move(new Vector2(0, -movementSpeed) * deltaTime);

            if (_camera.Position.X > -628 && keyboardState.IsKeyDown(Keys.A))
                _camera.Move(new Vector2(-movementSpeed, 0) * deltaTime);

            if (_camera.Position.Y - 800 < 1388 && keyboardState.IsKeyDown(Keys.S))
                _camera.Move(new Vector2(0, movementSpeed) * deltaTime);

            if (_camera.Position.X - 720 < 1428 && keyboardState.IsKeyDown(Keys.D))
                _camera.Move(new Vector2(movementSpeed, 0) * deltaTime);

            //No need for ration, but can be implented if needed.
            //if (keyboardState.IsKeyDown(Keys.E))
            //    _camera.Rotation += rotationSpeed * deltaTime;

            //if (keyboardState.IsKeyDown(Keys.Q))
            //    _camera.Rotation -= rotationSpeed * deltaTime;

            if (keyboardState.IsKeyDown(Keys.R))
                _camera.ZoomIn(zoomSpeed * deltaTime);

            if (keyboardState.IsKeyDown(Keys.F))
                _camera.ZoomOut(zoomSpeed * deltaTime);

            _previousState = keyboardState;
        }

        private void KeyboardHandlerPlayer(KeyboardState keyboardState, GameTime gameTime, BreakableBody playerBreakableBody,float speed = 1f)
        {
            var cosAngle = (float)Math.Cos(playerBreakableBody.MainBody.Rotation);
            var sinAngle = (float)Math.Sin(playerBreakableBody.MainBody.Rotation);
            /*this doesn't work if camera controlls called before. Still have to figure out why. */
            if (keyboardState.IsKeyDown(Keys.Escape) & !_previousState.IsKeyDown(Keys.Escape))
                Exit();

            if (keyboardState.IsKeyDown(Keys.N) & !_previousState.IsKeyDown(Keys.N)) negativeForce = !negativeForce;
            if (keyboardState.IsKeyDown(Keys.P) & !_previousState.IsKeyDown(Keys.P)) Pause = !Pause;
            if (!Pause)
            {
                if (keyboardState.IsKeyDown(Keys.Space) && !_previousState.IsKeyDown(Keys.Space)) //Player fired a missile
                {
                    _player.FireMissile(speed);
                    System.Diagnostics.Debug.WriteLine("bullet");
                }

                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    _player.Boosting = true;
                    playerBreakableBody.MainBody.ApplyLinearImpulse(new Vector2(speed * cosAngle, speed * sinAngle));
                }
                else
                {
                    if (!GamePad.GetState(PlayerIndex.One).IsConnected) _player.Boosting = false;
                }



                if (keyboardState.IsKeyDown(Keys.Right))
                    playerBreakableBody.MainBody.ApplyTorque(10f);

                if (keyboardState.IsKeyDown(Keys.Left))
                    playerBreakableBody.MainBody.ApplyTorque(-10f);

                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    if (!negativeForce)//!false
                        playerBreakableBody.MainBody.ApplyLinearImpulse(new Vector2(-speed * cosAngle * 0.3f, -speed * sinAngle * 0.3f));
                    else
                    {
                        playerBreakableBody.MainBody.LinearVelocity = Vector2.Zero;
                        playerBreakableBody.MainBody.AngularVelocity *= .1f; //playwith this
                    }
                }
            }
            

            _previousState = keyboardState;
        }

        public void GamePadController(GamePadState state, GameTime gameTime, BreakableBody playerBreakableBody, float speed = 1f) {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            const float movementSpeed = 200;
            const float zoomSpeed = 0.5f;
            var cosAngle = (float)Math.Cos(playerBreakableBody.MainBody.Rotation);
            var sinAngle = (float)Math.Sin(playerBreakableBody.MainBody.Rotation);
            if (_camera.Position.Y - 800 < 1388 && state.DPad.Down == ButtonState.Pressed )
                _camera.Move(new Vector2(0, movementSpeed) * deltaTime);
            if (_camera.Position.Y > -668 && state.DPad.Up == ButtonState.Pressed )
                _camera.Move(new Vector2(0, -movementSpeed) * deltaTime);
            if (_camera.Position.X > -628 && state.DPad.Left == ButtonState.Pressed )
                _camera.Move(new Vector2(-movementSpeed, 0) * deltaTime);
            if (_camera.Position.X - 720 < 1428 && state.DPad.Right == ButtonState.Pressed )
                _camera.Move(new Vector2(movementSpeed, 0) * deltaTime);
            if (state.Buttons.Y == ButtonState.Pressed )
                _camera.ZoomIn(zoomSpeed * deltaTime);
            if (state.Buttons.X == ButtonState.Pressed)
                _camera.ZoomOut(zoomSpeed * deltaTime);
            if (state.Buttons.Back == ButtonState.Pressed && !(_previousStateGamepad.Buttons.Back == ButtonState.Pressed))
                Exit();
            if (state.Buttons.Start == ButtonState.Pressed && !(_previousStateGamepad.Buttons.Start == ButtonState.Pressed))
                Pause = !Pause;
            if (state.Buttons.LeftShoulder == ButtonState.Pressed && !(_previousStateGamepad.Buttons.LeftShoulder == ButtonState.Pressed))
                negativeForce = !negativeForce;

            if (!Pause)
            {
                if (state.Buttons.RightShoulder == ButtonState.Pressed && !(_previousStateGamepad.Buttons.RightShoulder == ButtonState.Pressed))
                    _player.FireMissile(speed);
                if (state.Buttons.A == ButtonState.Pressed)
                {
                    _player.Boosting = true;
                    playerBreakableBody.MainBody.ApplyLinearImpulse(new Vector2(speed * cosAngle, speed * sinAngle));
                }
                else _player.Boosting = false;

                if (state.Buttons.B == ButtonState.Pressed)
                {
                    if (!negativeForce)//!false
                        playerBreakableBody.MainBody.ApplyLinearImpulse(new Vector2(-speed * cosAngle * 0.3f, -speed * sinAngle * 0.3f));
                    else
                    {
                        playerBreakableBody.MainBody.LinearVelocity = Vector2.Zero;
                        playerBreakableBody.MainBody.AngularVelocity *= .1f; //playwith this
                    }
                }

                playerBreakableBody.MainBody.ApplyTorque(8f * state.ThumbSticks.Left.X);
            }
            

            _previousStateGamepad = state;

        }





    }
}
