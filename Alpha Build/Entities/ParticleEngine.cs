using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alpha_Build.Entities
{
    public class ParticleEngine : Entity
    {
        private Random random;
        public Vector2 EmitterLocation { get; set; }
        private List<Particle> particles;
        private Texture2D texture;

        public ParticleEngine(Game1 game, string textureLocation) :base(game, textureLocation)
        {
            texture = Content.Load<Texture2D>(textureLocation);
            this.particles = new List<Particle>();
            random = new Random();
        }

        public void Update(GameTime gameTime, int total, bool boosting, Vector2 position, Vector2 linearVelocity )
        {
            EmitterLocation = position;
            if (boosting)
            {
                for (int i = 0; i < total; i++)
                {
                    particles.Add(GenerateNewParticle(linearVelocity));
                }
            }
            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update(gameTime);
                if (particles[particle].TTL <= 0)
                {
                    particles.RemoveAt(particle);
                    particle--;
                }
            }
        }

        private Particle GenerateNewParticle(Vector2 velocity2)
        {
            velocity2.Normalize();
            Vector2 position = EmitterLocation;
            Vector2 velocity =  new Vector2(
                                    (float)(random.NextDouble() * 2 - 1),
                                    (float)(random.NextDouble() * 2 - 1));
            float angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);
            Color color =  new Color(Color.OrangeRed, .5f);
            float size = (float)random.NextDouble()*.4f;
            int ttl = 20 + random.Next(40);
            float angle = 0;
            return new Particle(texture, position, (velocity + velocity2), angle, angularVelocity, color, size, ttl);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            particles.ForEach(a => a.Draw(spriteBatch));
        }
    }
}
