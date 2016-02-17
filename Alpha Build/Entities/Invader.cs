
using Alpha_Build.Entities;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.PolygonManipulation;
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
    class Invader : Entity
    {
        private Texture2D _texture;
        public List<Sprite> invaders;
        public List<BreakableBody> invaderBreakableBody;
        public BreakableBody target;
        private Missiles _missiles;
        private float _angleToTarget;
        public ParticleEngine particles;
        public Invader(Game1 game, World world, float ratio, string texturePath, string textureMissilePath, string textureParticlePath, BreakableBody target ) : base(game, world, texturePath)
        {
            _texture = Content.Load<Texture2D>(texturePath);
            invaders = new List<Sprite>();
            invaderBreakableBody = new List<BreakableBody>();
            _ratio = ratio;
            this.target = target;
            _missiles = new Missiles(game, world, ratio, textureMissilePath);
            particles = new ParticleEngine(game, textureParticlePath);
            Practice = false;
        }
        public float _ratio { get; private set; }
        public bool Practice { get; set; }

        public void CreateInvader(float strength, Vector2 position, float angle = 0f )
        {
            BreakableBody invaderBody;

            uint[] data = new uint[_texture.Width * _texture.Height];
            _texture.GetData(data);
            List<Vertices> list = PolygonTools.CreatePolygon(data, _texture.Width, 1.55f, 1, true, false); //Play around with hullTolerance
            for (int i = 0; i < list.Count; i++)
            {
                //System.Diagnostics.Debug.WriteLine(list.Count + "  List Count");
                Vertices polygon = list[i];
                Vector2 centroid = -polygon.GetCentroid();
                polygon.Translate(centroid);
                polygon = SimplifyTools.CollinearSimplify(polygon);
                polygon = SimplifyTools.ReduceByDistance(polygon, 4);
                List<Vertices> triangulation = Triangulate.ConvexPartition(polygon, TriangulationAlgorithm.Bayazit);

                // Scale to your screen view
                Vector2 vertScale = new Vector2(1 / _ratio);
                foreach (var vertices in triangulation)
                {
                    vertices.Scale(vertScale);
                }

                invaderBody = new BreakableBody(world, triangulation, 10, position: position, rotation: angle);
                invaderBody.Strength = strength;
                //invaderBody.MainBody.Restitution = 0.00010f;
                invaderBody.MainBody.CollisionCategories = Category.Cat2 & ~Category.Cat5;
                invaderBody.MainBody.CollidesWith = Category.All & ~Category.Cat5;
                //invaderBody.MainBody.SleepingAllowed = false;
                //System.Diagnostics.Debug.WriteLine("mass of invader: " + invaderBody.MainBody.Mass);
                world.AddBreakableBody(invaderBody);

                var sprite = new Sprite(_texture) { Position = invaderBody.MainBody.Position * _ratio, Rotation = invaderBody.MainBody.Rotation };
                invaderBreakableBody.Add(invaderBody);
                invaders.Add(sprite);
            }
        }

        public override void Update(GameTime gameTime)
        {
            var random = new Random().Next(0, invaderBreakableBody.Count);
            for (int i = 0; i < invaderBreakableBody.Count; i++)
            {
                if (invaderBreakableBody[i].Broken)
                {
                    invaders.RemoveAt(i);
                    invaderBreakableBody.RemoveAt(i);
                }
                else
                {
                    invaders[i].Position = invaderBreakableBody[i].MainBody.Position * _ratio;
                    invaders[i].Rotation = invaderBreakableBody[i].MainBody.Rotation;
                    var anglepos = (double)((target.MainBody.Position.Y - invaderBreakableBody[i].MainBody.Position.Y) / (target.MainBody.Position.X - invaderBreakableBody[i].MainBody.Position.X));
                    _angleToTarget =(float) Math.Atan(anglepos);

                    if (!(invaderBreakableBody[i].MainBody.Rotation >= _angleToTarget - MathHelper.ToRadians(15f) && invaderBreakableBody[i].MainBody.Rotation <= _angleToTarget + MathHelper.ToRadians(15f)))
                    {
                        System.Diagnostics.Debug.WriteLine("out of reach!!");
                        if (invaderBreakableBody[i].MainBody.Rotation <= _angleToTarget + Math.PI) invaderBreakableBody[i].MainBody.ApplyTorque(500f);
                        else invaderBreakableBody[i].MainBody.ApplyTorque(-500f); //invaderBreakableBody[i].MainBody.AngularVelocity += .1f;
                    }
                    else {
                        
                        System.Diagnostics.Debug.WriteLine("in reach!!");
                        var velocity = (invaderBreakableBody[i].MainBody.Position - target.MainBody.Position);
                        velocity.Normalize();
                        invaderBreakableBody[i].MainBody.ApplyLinearImpulse(-velocity);
                        if (invaderBreakableBody[i].MainBody.Rotation >= _angleToTarget - MathHelper.ToRadians(10f) && invaderBreakableBody[i].MainBody.Rotation <= _angleToTarget + MathHelper.ToRadians(10f))
                            invaderBreakableBody[i].MainBody.AngularVelocity *= .0001f;

                    }

                    //System.Diagnostics.Debug.WriteLine(invaderBreakableBody[i].MainBody.Rotation + " --> invader rotation");
                    //System.Diagnostics.Debug.WriteLine((_angleToTarget - MathHelper.ToRadians(15f)) + " --> low rotation");
                    //System.Diagnostics.Debug.WriteLine((_angleToTarget + MathHelper.ToRadians(15f)) + " --> high rotation");
                    if (gameTime.TotalGameTime.Seconds % 7 == 0 && gameTime.TotalGameTime.Milliseconds % 500 == 0 && i == random)
                        if(!Practice)
                        FireMissile(invaderBreakableBody[i], .8f); //random fire every seven seconds
                    var distanceFromRotationX = (float)(invaderBreakableBody[i].MainBody.Position.X + 1.5 * Math.Cos(invaderBreakableBody[i].MainBody.Rotation - MathHelper.Pi));
                    var distanceFromRotationY = (float)(invaderBreakableBody[i].MainBody.Position.Y + 1.5 * Math.Sin(invaderBreakableBody[i].MainBody.Rotation - MathHelper.Pi));
                    var cosAngle = -(float)Math.Cos(invaderBreakableBody[i].MainBody.Rotation);
                    var sinAngle = -(float)Math.Sin(invaderBreakableBody[i].MainBody.Rotation);
                    //particles.Update(gameTime, 1, true, new Vector2(distanceFromRotationX, distanceFromRotationY) * _ratio, new Vector2(cosAngle, sinAngle));

                }
            }
            _missiles.Update(gameTime);

            
        }
        
        public override void Draw (SpriteBatch _spriteBatch)
        {
            invaders.ForEach(a => _spriteBatch.Draw(a));
            _missiles.Draw(_spriteBatch);
            //particles.Draw(_spriteBatch);
        }

        public void FireMissile(BreakableBody body, float speed)
        {
            var cosAngle = (float)Math.Cos(body.MainBody.Rotation);
            var sinAngle = (float)Math.Sin(body.MainBody.Rotation);
            var distanceFromRotationX = (float)(body.MainBody.Position.X + 4.5 * Math.Cos(body.MainBody.Rotation));
            var distanceFromRotationY = (float)(body.MainBody.Position.Y + 4.5 * Math.Sin(body.MainBody.Rotation));
            _missiles.CreateBullet(new Vector2(distanceFromRotationX, distanceFromRotationY), body.MainBody.Rotation, new Vector2(speed * cosAngle * 30, speed * sinAngle * 30) + body.MainBody.LinearVelocity);



        }

    }
}
