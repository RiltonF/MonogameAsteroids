using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alpha_Build
{
    class DebugView
    {
        private DebugViewXNA _debugView;

        public DebugView(World _world, Game1 game, float _ratio)
        {
            _debugView = new DebugViewXNA(_world);
            _debugView.AppendFlags(FarseerPhysics.DebugViewFlags.DebugPanel);
            _debugView.DefaultShapeColor = Color.Red;
            _debugView.SleepingShapeColor = Color.DarkRed;
            _debugView.LoadContent(game.GraphicsDevice, game.Content);
            _debugView.AppendFlags(FarseerPhysics.DebugViewFlags.Shape);
            //_debugView.AppendFlags(FarseerPhysics.DebugViewFlags.PerformanceGraph);
            //_debugView.AppendFlags(FarseerPhysics.DebugViewFlags.AABB);
            //_debugView.AppendFlags(FarseerPhysics.DebugViewFlags.CenterOfMass);
            //_debugView.AppendFlags(FarseerPhysics.DebugViewFlags.ContactNormals);
            //_debugView.AppendFlags(FarseerPhysics.DebugViewFlags.ContactPoints);
            //_debugView.AppendFlags(FarseerPhysics.DebugViewFlags.Controllers);
            //_debugView.AppendFlags(FarseerPhysics.DebugViewFlags.Joint);
            //_debugView.AppendFlags(FarseerPhysics.DebugViewFlags.PolygonPoints);
            //
            //_debugView.AppendFlags(FarseerPhysics.DebugViewFlags.PolygonPoints);


            this.game = game;
            this._ratio = _ratio;
            Enabled = true;
        }

        public Game1 game { get; set; }
        public float _ratio { get; private set; }
        public bool Enabled { get; set; }

        public void Draw(ref Matrix projection,ref Matrix view)
        {
            if (Enabled) _debugView.RenderDebugData(ref projection, ref view);
        }
    }
}
