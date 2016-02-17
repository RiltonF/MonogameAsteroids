using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alpha_Build.Entities
{
    /* Forked from http://rbwhitaker.wikidot.com example with a little improvements of my own */
    class Particle : Entity
    {
        public Sprite sprite { get; set; }
        public Vector2 Velocity { get; set; }
        public float AngularVelocity { get; set; }
        public float Size { get; set; }
        public int TTL { get; set; }
        private float opac;

        public Particle( Texture2D texture, Vector2 position, Vector2 velocity, float angle, float angularVelocity, Color color, float size, int ttl) : base()
        {
            sprite = new Sprite(texture){ Position = position, Rotation = angle, Scale = new Vector2(size)};

            Velocity = velocity;
            AngularVelocity = angularVelocity;
            sprite.Color = color;
            Size = size;
            TTL = ttl;
            opac = 1f;
        }

        public override void Update(GameTime gameTime)
        {
            TTL--;
            sprite.Position += Velocity;
            sprite.Rotation += AngularVelocity;
            sprite.Scale = new Vector2(Size);
            //if (TTL % 200 == 0) sprite.Color = new Color(Color.Crimson, opac-= .2f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite);
        }
    }
}
