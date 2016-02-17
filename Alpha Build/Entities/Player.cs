using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alpha_Build.Entities
{
    /* Textures are in Player directory */
     class Player : Entity
    {
        public Sprite[] avion;
        private Texture2D _textureReference;
        private float _ratio;
        private Body ball;
        private Missiles _missiles;
        private int _boostTimer;
        public ParticleEngine particles;
        public Player(Game1 game, World world, float ratio, float strength, Vector2 position,string texture_0_path,string texture_1_path, string texture_2_path, string texture_3_path, string textureMissilePath, string textureParticlePath) : base (game, world, texture_0_path,texture_1_path,texture_2_path,texture_3_path)
        {
            var texture_0 = Content.Load<Texture2D>(texture_0_path);
            var texture_1 = Content.Load<Texture2D>(texture_1_path);
            var texture_2 = Content.Load<Texture2D>(texture_2_path);
            var texture_3 = Content.Load<Texture2D>(texture_3_path);

            _missiles = new Missiles(game, world, ratio, textureMissilePath);
            particles = new ParticleEngine(game, textureParticlePath);

            _textureReference = texture_0;

            _ratio = ratio;
            Broken = false;
            Boosting = false;
            _boostTimer = 0;
            _Index = 0;
            Bullets = 4;
            Reload = false;

            CreatePlayerBreakableBody(strength, position);

            //ball = BodyFactory.CreateCircle(world, 5, 1, position: new Vector2(40,36));
            //ball.BodyType = BodyType.Static;
            //ball.CollisionCategories = Category.Cat1;
            //ball.CollidesWith = Category.All;

            avion = new Sprite[4];
            avion[0] = new Sprite(texture_0) { Position = this.Position, Rotation = this.Rotation }; 
            avion[1] = new Sprite(texture_1) { Position = this.Position, Rotation = this.Rotation };
            avion[2] = new Sprite(texture_2) { Position = this.Position, Rotation = this.Rotation };
            avion[3] = new Sprite(texture_3) { Position = this.Position, Rotation = this.Rotation };
        }


        public BreakableBody playerBreakableBody { get; private set; }
        public Vector2 Position
        {
            get { return playerBreakableBody.MainBody.Position; }
            set {
                    for (int i = 0; i < avion.Length; i++)
                    {
                        avion[0].Position = value;
                    }
                }
        }
        public float Rotation
        { 
            get { return playerBreakableBody.MainBody.Rotation; }
            set {
                    for (int i = 0; i < avion.Length; i++)
                    {
                        avion[0].Rotation = value;
                    }
                }
        }
        public bool Broken { get; set; }
        public bool Boosting { get; set; }
        private int _Index { get; set; }
        public int Bullets { get; set; }
        public bool Reload { get; set; }

        private void CreatePlayerBreakableBody(float strength, Vector2 position)
        {
            uint[] data = new uint[_textureReference.Width * _textureReference.Height];
            _textureReference.GetData(data);
            List<Vertices> list = PolygonTools.CreatePolygon(data, _textureReference.Width, 40f, 1, true, false); //Play around with hullTolerance 1.55
            for (int i = 0; i < list.Count; i++)
            {
                Vertices polygon = list[i];
                Vector2 centroid = -polygon.GetCentroid();
                polygon.Translate(centroid);
                polygon = SimplifyTools.CollinearSimplify(polygon);
                polygon = SimplifyTools.ReduceByDistance(polygon, 4);
                List<Vertices> triangulation = Triangulate.ConvexPartition(polygon, TriangulationAlgorithm.Earclip);

                // Scale to your screen view. A little smaller so texture overlaps the body, usually this would be 1f.
                Vector2 vertScale = new Vector2(.9f / _ratio);
                foreach (var vertices in triangulation)
                {
                    vertices.Scale(vertScale);
                }

                playerBreakableBody = new BreakableBody(world, triangulation, 1, position: position, rotation:-MathHelper.PiOver2);
                playerBreakableBody.Strength = strength;
                //playerBreakableBody.MainBody.Restitution = 0.00010f;
                playerBreakableBody.MainBody.CollisionCategories = Category.Cat1;
                playerBreakableBody.MainBody.CollidesWith = Category.All;
                
                world.AddBreakableBody(playerBreakableBody);
            }
        }

        public override void Update(GameTime gameTime)
        {
            avion.Select(c => { c.Position = (playerBreakableBody.MainBody.Position * _ratio); c.Rotation = playerBreakableBody.MainBody.Rotation; return c; }).ToList(); //update body to texture
            this.Broken = playerBreakableBody.Broken;
            _missiles.Update(gameTime);

            if (Boosting) //for animation
            {
                _boostTimer++;
                switch (_boostTimer)
                {
                    case 1: _Index = 1; break;
                    case 40: _Index = 2; break;
                    case 70: _Index = 3; break;                   
                }              
            }
            else { _boostTimer = 0; _Index = 0; }
            if (Reload && Bullets < 4)
            {
                Bullets = 4;
                Reload = false;
                //play sound, reload
            }

            var distanceFromRotationX = (float)(Position.X + 1.5 * Math.Cos(Rotation - MathHelper.Pi));
            var distanceFromRotationY = (float)(Position.Y + 1.5 * Math.Sin(Rotation - MathHelper.Pi));
            var cosAngle = -(float)Math.Cos(Rotation);
            var sinAngle = -(float)Math.Sin(Rotation);
            particles.Update(gameTime, 10, Boosting, new Vector2(distanceFromRotationX, distanceFromRotationY) * _ratio, new Vector2(cosAngle, sinAngle)); // TODO:fix velocity of particles!!
            // System.Diagnostics.Debug.WriteLine(Boosting + "Boosting");
        }
        public override void Draw(SpriteBatch _spriteBatch)
        {
            particles.Draw(_spriteBatch);
            if (!playerBreakableBody.Broken)
            _spriteBatch.Draw(avion[_Index]);
            _missiles.Draw(_spriteBatch);
        }

        public void FireMissile(float speed)
        {
            var cosAngle = (float)Math.Cos(Rotation);
            var sinAngle = (float)Math.Sin(Rotation);
            var distanceFromRotationX = (float)(Position.X + 2.5 * Math.Cos(Rotation));
            var distanceFromRotationY = (float)(Position.Y + 2.5 * Math.Sin(Rotation));
            if (Bullets > 0 && !Broken)
            {
                //fire a missile in accordance to the ship's linearmomentum and angle
                _missiles.CreateBullet(new Vector2(distanceFromRotationX, distanceFromRotationY), Rotation, new Vector2(speed * cosAngle * 30, speed * sinAngle * 30) + playerBreakableBody.MainBody.LinearVelocity);
                //play audio
                Bullets--;
            }
            else {//play audio 
            }
            

        }

    }
}
