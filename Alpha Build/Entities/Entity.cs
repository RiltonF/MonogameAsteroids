using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alpha_Build.Entities
{
     public abstract class Entity
    {
        private readonly Game1 _mainGame;
        public ContentManager Content { get { return _mainGame.Content; } }
        public World world { get; private set; }
        public Entity() { }
        public Entity(Game1 game)
        {
            _mainGame = game;
        }
        public Entity(Game1 game, World world)
        {
            _mainGame = game;
            this.world = world;
        }

        public Entity(Game1 game, string texture1)
        {
            _mainGame = game;
        }
        public Entity(Game1 game, World world, string texture1)
        {
            _mainGame = game;
            this.world = world;
        }

        public Entity(Game1 game, World world, string texture1, string texture2)
        {
            _mainGame = game;
            this.world = world;
        }

        public Entity(Game1 game, World world, string texture1, string texture2, string texture3)
        {
            _mainGame = game;
            this.world = world;
        }

        public Entity(Game1 game, World world, string texture1, string texture2, string texture3, string texture4)
        {
            _mainGame = game;
            this.world = world;
        }

        public Entity(Game1 game, World world, string texture1, string texture2, string texture3, string texture4, string texture5)
        {
            _mainGame = game;
            this.world = world;
        }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(SpriteBatch _spriteBatch) { }


    }

    
}
