using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Controllers;
using FarseerPhysics.Common;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Common.Decomposition;
using MonoGame.Extended.Sprites;
using FarseerPhysics.Factories;

namespace Alpha_Build.Entities
{
    class Planet:Entity
    {
        public GravityController gravity;
        private Texture2D _textureReference;
        private float _ratio;
        public Sprite planet;
        public Sprite planetRing;
        
        public Planet(Game1 game, World world, float ratio, float strength, Vector2 position, string texturePath, string texturePath2, Player player) : base(game,world,texturePath, texturePath2)
        {
            gravity = new GravityController(.5f, 40f, 0f);
            gravity.GravityType = GravityType.DistanceSquared;
            _textureReference = Content.Load<Texture2D>(texturePath2);
            var texture = Content.Load<Texture2D>(texturePath);
            _ratio = ratio;
            Player = player;
            
            
            CreatePlanetBreakableBody(strength, position);

            planet = new Sprite(texture) { Position = Position,Rotation = Rotation, Scale=new Vector2(0.53f)};
            planetRing = new Sprite(_textureReference) { Position = Position, Rotation = Rotation,Color = new Color(Color.DarkGray ,1f) };
            planetBody = BodyFactory.CreateCircle(world, 6f, 1f,position: Position, bodyType: BodyType.Static);
            gravity.AddBody(planetBody);
            world.AddController(gravity);
        }
        public BreakableBody planetBreakableBody { get; private set; }
        public Body planetBody { get; set; }
        public Vector2 Position { get { return planetBreakableBody.MainBody.Position; } }
        public float Rotation { get { return planetBreakableBody.MainBody.Rotation; } }
        public Player Player { get; set; }

        public bool Broken { get { return planetBreakableBody.Broken; } }
        private void CreatePlanetBreakableBody(float strength, Vector2 position)
        {
            uint[] data = new uint[_textureReference.Width * _textureReference.Height];
            _textureReference.GetData(data);
            List<Vertices> list = PolygonTools.CreatePolygon(data, _textureReference.Width, 40f, 1, true, true); //Play around with hullTolerance 1.55
            for (int i = 0; i < list.Count; i++)
            {
                Vertices polygon = list[i];
                Vector2 centroid = -polygon.GetCentroid();
                polygon.Translate(centroid);
                polygon = SimplifyTools.CollinearSimplify(polygon);
                polygon = SimplifyTools.ReduceByDistance(polygon, 4);
                List<Vertices> triangulation = Triangulate.ConvexPartition(polygon, TriangulationAlgorithm.Bayazit);

                // Scale to your screen view. A little smaller so texture overlaps the body, usually this would be 1f.
                Vector2 vertScale = new Vector2(1f / _ratio);
                foreach (var vertices in triangulation)
                {
                    vertices.Scale(vertScale);
                }

                planetBreakableBody = new BreakableBody(world, triangulation, 100, position: position, rotation: -MathHelper.PiOver2);
                planetBreakableBody.Strength = strength;
                planetBreakableBody.MainBody.Mass = 700f;
                planetBreakableBody.MainBody.CollisionCategories = Category.Cat1;
                planetBreakableBody.MainBody.CollidesWith = Category.All;
                planetBreakableBody.MainBody.LinearDamping = 2000f;//play with this
                planetBreakableBody.MainBody.AngularVelocity = .05f;
                planetBreakableBody.MainBody.OnSeparation += Reload_OnSeparation;
                world.AddBreakableBody(planetBreakableBody);
            }
        }
        public override void Update(GameTime gameTime)
        {
            planet.Rotation = planetBody.Rotation;
            planet.Position = planetBody.Position * _ratio;
            planetRing.Rotation = Rotation;
            planetRing.Position = Position * _ratio + new Vector2(1, 0);

        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            if (!Broken)
            {
                
                _spriteBatch.Draw(planetRing);
            }
            _spriteBatch.Draw(planet);
        }
        private void Reload_OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            if (fixtureB.CollisionCategories == Category.Cat1)
            {
                Player.Reload = true;
                System.Diagnostics.Debug.WriteLine("We have reloaded");
            }
            if (fixtureB.CollisionCategories == Category.Cat2)
            {

            }

        }
    }
}
