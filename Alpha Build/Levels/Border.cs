using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics.Contacts;

namespace Alpha_Build.Levels
{
    class Border
    {
        private Body _anchorBottom;
        private Body _anchorTop;
        private Body _exitBottom;
        private Body _exitTop;
        
        public Border(World _world)
        {
            _anchorBottom = BodyFactory.CreateLineArc(_world, 2* MathHelper.Pi, 100, 100, position: new Vector2 (40f, 36f));
            _anchorTop = BodyFactory.CreateLineArc(_world, MathHelper.Pi, 100, 100, position: new Vector2(40f, 36f), rotation: MathHelper.Pi);

            _exitBottom = BodyFactory.CreateLineArc(_world, 2 * MathHelper.Pi, 100, 110, position: new Vector2(40f, 36f));
            _exitTop = BodyFactory.CreateLineArc(_world, MathHelper.Pi, 100, 110, position: new Vector2(40f, 36f), rotation: MathHelper.Pi);

            _exitTop.OnCollision += OnCollision;
            _exitBottom.OnCollision += OnCollision;

            _exitTop.CollisionCategories = Category.All;
            _exitTop.CollidesWith = Category.All;

            _exitBottom.CollisionCategories = Category.All;
            _exitBottom.CollidesWith = Category.All;

            _anchorTop.CollisionCategories = Category.Cat5;  //.All & ~Category.Cat3 & ~Category.Cat2;
            _anchorTop.CollidesWith = Category.Cat1;//Category.All & ~Category.Cat3 & ~Category.Cat2; //collides with enemy and player

            _anchorBottom.CollisionCategories = Category.All & ~Category.Cat3;
            _anchorBottom.CollidesWith = Category.Cat1;//Category.All & ~Category.Cat3 & ~Category.Cat2;  //collides with enemy and player

            //_anchorTop.IsSensor = true;
            //_anchorTop.ContactList
            //_anchorBottom.OnSeparation += OnSeparation;
            //System.Diagnostics.Debug.WriteLine( _anchorTop.ContactList+"contact list");
        }

        private bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureB.CollisionCategories == Category.Cat3)
            {
                fixtureB.Body.Dispose();
                System.Diagnostics.Debug.WriteLine("body removed");
            }
            return true;
        }

        private void OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            //fixtureB.Body.LinearVelocity = Vector2.Zero;
            //fixtureA.Body.LinearVelocity = Vector2.Zero;
            //System.Diagnostics.Debug.WriteLine("separated");
        }
    }
}
