using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System.Linq;
using Alpha_Build.Levels;
using MonoGame.Extended.Sprites;
using Microsoft.Xna.Framework.Media;

namespace Alpha_Build
{

    public class Game1 : Game
    {
        private SpriteBatch _spriteBatch;
        private BitmapFont _bitmapFont;
        private BitmapFont _bitmapFont2;
        private WindowMetadata[] _levels;
        private int _selectedIndex;
        private KeyboardState _previousKeyboardState;
        private GamePadState _previousGamePadState;
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        public Level CurrentLevel { get; private set; }
        private Sprite _background;
        private Texture2D bagroudTexture;
        public bool ConnectedGamePad { get; set; }
        public Game1()
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            LoadDefaults();
        }

        public void Reset()
        {
            CurrentLevel = null;
            LoadDefaults();
        }

        private void LoadDefaults()
        {
            Window.Title = "Invaders Space 2 by B.V.E";
            Window.AllowUserResizing = false;
            IsMouseVisible = true;
            //It's better is the game is in fullscreen.
            //GraphicsDeviceManager.PreferredBackBufferHeight = 720;
            //GraphicsDeviceManager.PreferredBackBufferHeight = 720;
            GraphicsDeviceManager.IsFullScreen = true;
            GraphicsDeviceManager.ApplyChanges();

        }

        protected override void LoadContent()
        {
            GraphicsDeviceManager.IsFullScreen = true;

            //Load out assets through the pipeling and extended.

            _background = new Sprite(Content.Load<Texture2D>("Miscellaneous/starry-night-background"));
            bagroudTexture = Content.Load<Texture2D>("Miscellaneous/starry-night-background");
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _bitmapFont = Content.Load<BitmapFont>("Fonts/montserrat-32");
            _bitmapFont2 = Content.Load<BitmapFont>("Fonts/enigma-42");


            _levels = new[]
                {
                    new WindowMetadata("Level 1", ()=>new Level_1(this)),
                    new WindowMetadata("Level 2", ()=>new Level_2(this)),
                    new WindowMetadata("Practice", ()=>new Level_Practice(this))
                }
                .OrderBy(i => i.Name).ToArray();
            _selectedIndex = 0;
           
        }

        protected override void UnloadContent()
        {

        }


        protected override void Update(GameTime gameTime)
        {
            if (CurrentLevel != null)
            {
                CurrentLevel.OnUpdate(gameTime);
                return;
            }

            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                ConnectedGamePad = true;
                var gamePadState = GamePad.GetState(PlayerIndex.One);
                GamePadController(gamePadState);
            }
            else
            {
                ConnectedGamePad = false;
            }


            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
            {
                _selectedIndex++;

                if (_selectedIndex == _levels.Length)
                    _selectedIndex--;
            }
            else if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
            {
                _selectedIndex--;

                if (_selectedIndex < 0)
                    _selectedIndex++;
            }
            else if (keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
            {
                var sample = _levels[_selectedIndex].CreateLevelFunction();
                sample.OnLoad();
                CurrentLevel = sample;
            }
            if (keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
            {
                //Exit(); //menu?
            }

            _previousKeyboardState = keyboardState;
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);
            if (CurrentLevel != null) { CurrentLevel.OnDraw(gameTime); return; }

            _spriteBatch.Begin();
            //_spriteBatch.Draw(_background);
            _spriteBatch.Draw(bagroudTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            _spriteBatch.DrawString(_bitmapFont2, "Invaders Space!", new Vector2(50, 50), new Color(Color.White, 1f), wrapWidth: 750); //wrapWidth??
            _spriteBatch.DrawString(_bitmapFont, "Choose your level:", new Vector2(50, 180), new Color(Color.Silver, 1f), wrapWidth: 750); //wrapWidth??
            _spriteBatch.DrawString(_bitmapFont, string.Format("Controller connected: {0}", ConnectedGamePad), new Vector2(5f, GraphicsDevice.Viewport.Height - 40f), Color.White);
            for (int i = 0; i < _levels.Length; i++)
            {
                _spriteBatch.DrawString(_bitmapFont, _levels[i].Name, new Vector2(50, 220 + i * _bitmapFont.LineHeight + 15), (i == _selectedIndex) ? Color.Silver * 1f : new Color(Color.Black, 0.8f), wrapWidth: 750); //if i is selected go white
            }
            _spriteBatch.End();



            base.Draw(gameTime);
        }

        public virtual void GamePadController(GamePadState state)
        {
            if (state.DPad.Down == ButtonState.Pressed && !(_previousGamePadState.DPad.Down == ButtonState.Pressed))
            {
                _selectedIndex++;

                if (_selectedIndex == _levels.Length)
                    _selectedIndex--;
            }
            else if (state.DPad.Up == ButtonState.Pressed && !(_previousGamePadState.DPad.Up == ButtonState.Pressed))
            {
                _selectedIndex--;

                if (_selectedIndex < 0)
                    _selectedIndex++;
            }
            else if (state.Buttons.A == ButtonState.Pressed &&!(_previousGamePadState.Buttons.A ==ButtonState.Pressed))
            {
                var sample = _levels[_selectedIndex].CreateLevelFunction();
                sample.OnLoad();
                CurrentLevel = sample;
            }

            _previousGamePadState = state;
        }
    }
}
