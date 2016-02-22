using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// Base class for crating entities. 
// I might've gone a bit overboard with the constructors
// In order to load the textures in you'll need give a path to the contructor.
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
            //define the game, we'll need this in order manipulate anything of the game class
            _mainGame = game;
        }
        public Entity(Game1 game, World world)
        {
            _mainGame = game;
            //define the world for the farseer physics engine
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
