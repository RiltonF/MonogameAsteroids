using MonoGame.Extended.InputListeners;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Alpha_Build
{
    [Obsolete("code too slow, causing gitters in movemet")]
    class Listeners
    {
        // InputManager is too slow for outputting changes in classes
        //
        // will have to use methods in as input handler
        //
        // KeyboardHandler is used instead in Level-x.cs

        public InputListenerManager _inputManager;
        public Listeners()
        {
            _inputManager = new InputListenerManager();
        }
        public float deltaTime { get; set; }
        public void SetPlayerKeys (BreakableBody breakableBody, float speed, bool negativeForce = false)
        {
            var playerMovement = _inputManager.AddListener(new KeyboardListenerSettings());
            var cosAngle = (float)Math.Cos(breakableBody.MainBody.Rotation);
            var sinAngle = (float)Math.Sin(breakableBody.MainBody.Rotation);
            //var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!negativeForce) playerMovement.KeyPressed += (sender, arg) => { if (arg.Key == Keys.Down) breakableBody.MainBody.ApplyLinearImpulse(new Vector2(-speed * cosAngle, -speed * sinAngle)); };

            playerMovement.KeyPressed += (sender, arg) => 
            {
                if (arg.Key == Keys.Up) breakableBody.MainBody.ApplyLinearImpulse(new Vector2(speed * cosAngle, speed * sinAngle));
                if (arg.Key == Keys.Left) breakableBody.MainBody.ApplyTorque(10f);
                if (arg.Key == Keys.Right) breakableBody.MainBody.ApplyTorque(-10f);
            };
        }

        public void SetupCameraKeys(Camera2D _camera)
        {
            const float movementSpeed = 200;
            const float rotationSpeed = 0.5f;
            const float zoomSpeed = 0.5f;

            var cameraMovement = _inputManager.AddListener(new KeyboardListenerSettings());
            cameraMovement.KeyTyped += (sender, arg) =>
                {
                    if (arg.Key == Keys.W) _camera.Move(new Vector2(0, -movementSpeed) * deltaTime);
                    if (arg.Key == Keys.S) _camera.Move(new Vector2(0, movementSpeed) * deltaTime);
                    if (arg.Key == Keys.A) _camera.Move(new Vector2(-movementSpeed, 0) * deltaTime);
                    if(arg.Key == Keys.D) _camera.Move(new Vector2(movementSpeed, 0) * deltaTime);
                };
        }

        public void Update(GameTime gameTime)
        {
            _inputManager.Update(gameTime);
        }

    }
}
