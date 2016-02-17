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
    class ColonyShip : Entity
    {
        private Texture2D _texture;
        private Sprite _colonyShipSprite;
        public BreakableBody colonyShipBreakableBody;
        private Player player;
        public ColonyShip(Game1 game, World world, float ratio, string texturePath, Player player) : base(game, world, texturePath)
        {
            _texture = Content.Load<Texture2D>(texturePath);
            _ratio = ratio;
            this.player = player;
        }

        public float _ratio { get; private set; }

        public bool Broken { get { return colonyShipBreakableBody.Broken; } }


        public void CreateColonyShip(float strength, Vector2 position)
        {
            uint[] data = new uint[_texture.Width * _texture.Height];
            _texture.GetData(data);
            List<Vertices> list = PolygonTools.CreatePolygon(data, _texture.Width, 1.55f, 1, false, false); //Play around with hullTolerance
            for (int i = 0; i < list.Count; i++)
            {
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

                colonyShipBreakableBody = new BreakableBody(world, triangulation, 20, position: position, rotation: -MathHelper.PiOver2);
                colonyShipBreakableBody.Strength = strength;
                colonyShipBreakableBody.MainBody.CollisionCategories = Category.Cat1;
                colonyShipBreakableBody.MainBody.CollidesWith = Category.All;
                world.AddBreakableBody(colonyShipBreakableBody);

                _colonyShipSprite = new Sprite(_texture) { Position = colonyShipBreakableBody.MainBody.Position * _ratio, Rotation = colonyShipBreakableBody.MainBody.Rotation };

                colonyShipBreakableBody.MainBody.OnSeparation += Reload_OnSeparation; 

            }
        }

        private void Reload_OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            if (fixtureB.CollisionCategories == Category.Cat1)
            {
                player.Reload = true;
                System.Diagnostics.Debug.WriteLine("We have reloaded");
            }
            if (fixtureB.CollisionCategories == Category.Cat2)
            {
                
            }
            
        }

        public override void Update(GameTime gameTime)
        {
            if (!colonyShipBreakableBody.Broken)
            {
                _colonyShipSprite.Position = colonyShipBreakableBody.MainBody.Position * _ratio;
                _colonyShipSprite.Rotation = colonyShipBreakableBody.MainBody.Rotation;
            }
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            if (!colonyShipBreakableBody.Broken)
            {
                _spriteBatch.Draw(_colonyShipSprite);
            }
        }
    }
}
