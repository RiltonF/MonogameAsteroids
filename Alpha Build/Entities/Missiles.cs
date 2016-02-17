using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;

namespace Alpha_Build.Entities
{
    class Missiles : Entity
    {
        private Texture2D _texture;
        public List<Sprite> missiles;
        public List<Body> missileBodies;
        private List<Body> _pellets;
        public Missiles(Game1 game,World world, float ratio, string texturePath) : base (game, world,texturePath)
        {
            _texture = Content.Load<Texture2D>(texturePath);
            missiles = new List<Sprite>();
            missileBodies = new List<Body>();
            _pellets = new List<Body>();
            _ratio = ratio;       
        }

        public void CreateBullet( Vector2 position, float rotation, Vector2 velocity)
        {
            var body = BodyFactory.CreateRectangle(world, .8f, .2f, 1f, position: position, bodyType: BodyType.Dynamic);
            body.IsBullet = true;
            body.CollisionCategories = Category.Cat3;
            body.CollidesWith = Category.Cat1 | Category.Cat3 | Category.Cat2; //colides with missiles and player/enemy

            
            //var body = BodyFactory.CreateCircle(_world, 1f, 1f, position: position, bodyType: BodyType.Dynamic);
            body.Rotation = rotation;
            body.ApplyLinearImpulse(velocity);
            body.LinearVelocity = velocity;


            //explosions!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            body.OnCollision += OnCollisionExplosion;
            //body.OnSeparation += OnSeparationExplosion;

            var sprite = new Sprite(_texture) { Position = position, Rotation = rotation, Scale = new Vector2(.5f) };

            missileBodies.Add(body);
            missiles.Add(sprite);
            
        }

        private void OnSeparationExplosion(Fixture fixtureA, Fixture fixtureB)
        {
            System.Diagnostics.Debug.WriteLine("Separation");

            //fixtureA.Body.Dispose();

            if (fixtureA.Body.IsDisposed == true)
            {
                return;
            }

            else if (fixtureB.CollisionCategories == Category.Cat1 && fixtureA.Body.IsDisposed == false)
            {

                fixtureA.Body.Dispose();

                Explosion(fixtureA);
            }

            System.Diagnostics.Debug.WriteLine("removed missile");
        }

        private bool OnCollisionExplosion(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            System.Diagnostics.Debug.WriteLine("Collision");

            //fixtureA.Body.Dispose();



            if (fixtureA.Body.IsDisposed == true)
            {
                return true;
            }

            else if (fixtureB.CollisionCategories == Category.Cat1 || fixtureB.CollisionCategories == Category.Cat3 || fixtureB.CollisionCategories == Category.Cat2 && fixtureA.Body.IsDisposed == false)
            {

                fixtureA.Body.Dispose();

                Explosion(fixtureA);
            }

            System.Diagnostics.Debug.WriteLine("removed missile");
            return true;
            
            
            
        }
        float numRays = 20;
        float blastPower = 1000;
        private void Explosion (Fixture fixtureA)
        {
            System.Diagnostics.Debug.WriteLine(fixtureA.Body.BodyId +"body id");

            for (int i = 0; i < numRays; i++)
            {
                float angle = (i / numRays) * MathHelper.TwoPi;
                Vector2 rayDir  = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));
                Body body = BodyFactory.CreateCircle(world, 0.05f, 60f);
                body.BodyType = BodyType.Dynamic;
                body.FixedRotation = true;
                body.IsBullet = true;
                body.LinearDamping = 10;
                body.GravityScale = 0;
                body.Position = fixtureA.Body.Position;
                body.LinearVelocity = blastPower * rayDir;
                body.CollisionGroup = -4;
                body.CollisionCategories = Category.Cat4;
                body.CollidesWith = Category.All;
                body.Restitution = 1.4f;
                body.IgnoreGravity = true;
                _pellets.Add(body);
            }


            System.Diagnostics.Debug.WriteLine("Explosion!!");
        }

        public float _ratio { get; private set; }

        public override void Update(GameTime gameTime)
        {
            foreach (var pellet in _pellets)
            {
                if (pellet.LinearVelocity == Vector2.Zero) pellet.Dispose();// for removing debri  
            }

            for (int i = 0; i < missileBodies.Count; i++)
            {
                //System.Diagnostics.Debug.WriteLine(missiles.Count + "count begin");
                //System.Diagnostics.Debug.WriteLine(i + " I value");
                
                
                if (missileBodies[i].IsDisposed)
                {
                   // System.Diagnostics.Debug.WriteLine(missiles.Count +"before");
                    missiles.RemoveAt(i);
                    missileBodies.RemoveAt(i);
                  //  System.Diagnostics.Debug.WriteLine(missiles.Count + "after");

                }
                else
                {
                    //System.Diagnostics.Debug.WriteLine(missiles.Count + "else");

                    missiles[i].Position = missileBodies[i].Position * _ratio;
                    missiles[i].Rotation = missileBodies[i].Rotation;
                }
            }
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            //foreach (var data in missiles)
            //{
            //    _spriteBatch.Draw(data);
            //}
            missiles.ForEach(a => _spriteBatch.Draw(a));
            
        }


    }
}
