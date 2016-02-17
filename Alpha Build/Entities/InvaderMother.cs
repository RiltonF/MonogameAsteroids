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
    class InvaderMother:Entity
    {
        private Texture2D _texture;
        public Sprite motherTexture;
        public BreakableBody target;
        private Missiles _missiles;
        private float _angleToTarget;
        public ParticleEngine particles;
        public InvaderMother(Game1 game, World world, float ratio, string texturePath, string textureMissilePath, string textureParticlePath, BreakableBody target) : base(game, world, texturePath)
        {
            _texture = Content.Load<Texture2D>(texturePath);

            _ratio = ratio;
            this.target = target;
            _missiles = new Missiles(game, world, ratio, textureMissilePath);
           // particles = new ParticleEngine(game, textureParticlePath);
            Practice = false;
            Alive = false;
        }
        public float _ratio { get; private set; }
        public bool Practice { get; set; }
        public BreakableBody MotherBreakableBody { get; set; }
        public bool Alive { get; set; }

        public void CreateMother(float strength, Vector2 position, float angle = 0f)
        {
            Alive = true;
            uint[] data = new uint[_texture.Width * _texture.Height];
            _texture.GetData(data);
            List<Vertices> list = PolygonTools.CreatePolygon(data, _texture.Width, 1.55f, 1, true, true); //Play around with hullTolerance
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

                MotherBreakableBody = new BreakableBody(world, triangulation, 10, position: position, rotation: angle);
                MotherBreakableBody.Strength = strength;
                //MotherBreakableBody.MainBody.Restitution = 0.00010f;
                MotherBreakableBody.MainBody.CollisionCategories = Category.Cat2 & ~Category.Cat5;
                MotherBreakableBody.MainBody.CollidesWith = Category.All & ~Category.Cat5;
                //MotherBreakableBody.MainBody.SleepingAllowed = false;
                //System.Diagnostics.Debug.WriteLine("mass of invader: " + MotherBreakableBody.MainBody.Mass);
                world.AddBreakableBody(MotherBreakableBody);

                motherTexture= new Sprite(_texture) { Position = MotherBreakableBody.MainBody.Position * _ratio, Rotation = MotherBreakableBody.MainBody.Rotation };

            }
        }

        public override void Update(GameTime gameTime)
        {
            //var random = new Random().Next(0, MotherBreakableBody.Count);
            if(Alive)
            if (!MotherBreakableBody.Broken)
            {
                    motherTexture.Position = MotherBreakableBody.MainBody.Position * _ratio + new Vector2(0,-20f);
                    motherTexture.Rotation = MotherBreakableBody.MainBody.Rotation;

                    var anglepos = (double)((target.MainBody.Position.Y - MotherBreakableBody.MainBody.Position.Y) / (target.MainBody.Position.X - MotherBreakableBody.MainBody.Position.X));
                    _angleToTarget = (float)Math.Atan(anglepos);

                    if (!(MotherBreakableBody.MainBody.Rotation >= _angleToTarget - MathHelper.ToRadians(15f) && MotherBreakableBody.MainBody.Rotation <= _angleToTarget + MathHelper.ToRadians(15f)))
                    {
                        System.Diagnostics.Debug.WriteLine("out of reach!!");
                        if (MotherBreakableBody.MainBody.Rotation <= _angleToTarget + Math.PI) MotherBreakableBody.MainBody.ApplyTorque(500f);
                        else MotherBreakableBody.MainBody.ApplyTorque(-500f); //MotherBreakableBody.MainBody.AngularVelocity += .1f;
                    }
                    else
                    {

                        System.Diagnostics.Debug.WriteLine("in reach!!");
                        var velocity = (MotherBreakableBody.MainBody.Position - target.MainBody.Position);
                        velocity.Normalize();
                        MotherBreakableBody.MainBody.ApplyLinearImpulse(-velocity);
                        if (MotherBreakableBody.MainBody.Rotation >= _angleToTarget - MathHelper.ToRadians(10f) && MotherBreakableBody.MainBody.Rotation <= _angleToTarget + MathHelper.ToRadians(10f))
                            MotherBreakableBody.MainBody.AngularVelocity *= .0001f;

                    }

                    //System.Diagnostics.Debug.WriteLine(MotherBreakableBody.MainBody.Rotation + " --> invader rotation");
                    //System.Diagnostics.Debug.WriteLine((_angleToTarget - MathHelper.ToRadians(15f)) + " --> low rotation");
                    //System.Diagnostics.Debug.WriteLine((_angleToTarget + MathHelper.ToRadians(15f)) + " --> high rotation");
                    if (gameTime.TotalGameTime.Seconds % 7 == 0 && gameTime.TotalGameTime.Milliseconds % 500 == 0)
                        if (!Practice)
                            FireMissile(MotherBreakableBody, .8f); //random fire every seven seconds
                    var distanceFromRotationX = (float)(MotherBreakableBody.MainBody.Position.X + 1.5 * Math.Cos(MotherBreakableBody.MainBody.Rotation - MathHelper.Pi));
                    var distanceFromRotationY = (float)(MotherBreakableBody.MainBody.Position.Y + 1.5 * Math.Sin(MotherBreakableBody.MainBody.Rotation - MathHelper.Pi));
                    var cosAngle = -(float)Math.Cos(MotherBreakableBody.MainBody.Rotation);
                    var sinAngle = -(float)Math.Sin(MotherBreakableBody.MainBody.Rotation);
                    //particles.Update(gameTime, 1, true, new Vector2(distanceFromRotationX, distanceFromRotationY) * _ratio, new Vector2(cosAngle, sinAngle));

                
                }
            _missiles.Update(gameTime);


        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            if(Alive) if(!MotherBreakableBody.Broken)
            _spriteBatch.Draw(motherTexture);
            _missiles.Draw(_spriteBatch);
            //particles.Draw(_spriteBatch);
        }

        public void FireMissile(BreakableBody body, float speed)
        {
            var cosAngle = (float)Math.Cos(body.MainBody.Rotation);
            var sinAngle = (float)Math.Sin(body.MainBody.Rotation);
            var distanceFromRotationX = (float)(body.MainBody.Position.X + 7.3 * Math.Cos(body.MainBody.Rotation));
            var distanceFromRotationY = (float)(body.MainBody.Position.Y + 7.3 * Math.Sin(body.MainBody.Rotation));
            _missiles.CreateBullet(new Vector2(distanceFromRotationX, distanceFromRotationY), body.MainBody.Rotation, new Vector2(speed * cosAngle * 30, speed * sinAngle * 30) + body.MainBody.LinearVelocity);

        }
    }
}
