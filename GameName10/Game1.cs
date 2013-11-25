#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace GameName10
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        float aspectRatio;
        
        Model ball;
        Vector3 ballPosition = Vector3.Zero;
        Vector3 ballVelocity = Vector3.Zero;
        float ballRotation = 0.0f;

        Model ground;
        Vector3 groundPosition = Vector3.Zero;
        float groundRotation = -45.0f;

        Vector3 cameraPosition = new Vector3(0.0f, 50.0f, 500.0f);

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content"; 
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ball = Content.Load<Model>("sphere");
            ground = Content.Load<Model>("groundPlane");
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            UpdateInput();

            // Add velocity to the current position.
            ballPosition += ballVelocity;

            // Bleed off velocity over time.
            ballVelocity *= 0.95f;

            base.Update(gameTime);
        }
        
        protected void UpdateInput()
        {
            // Get the game pad state.
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);
            if (currentState.IsConnected)
            {
                // Rotate the model using the left thumbstick, and scale it down
                ballRotation -= currentState.ThumbSticks.Left.X * 0.10f;

                //Debug.WriteLine(currentState.ThumbSticks.Left.X);

                // Create some velocity if the right trigger is down.
                Vector3 ballVelocityAdd = Vector3.Zero;

                // Find out what direction we should be thrusting, 
                // using rotation.
                ballVelocityAdd.X = (float)Math.Sin(ballRotation);
                ballVelocityAdd.Z = (float)Math.Cos(ballRotation);

                // Now scale our direction by how hard the trigger is down.

                if (currentState.Triggers.Right > currentState.Triggers.Left)
                    ballVelocityAdd *= currentState.Triggers.Right;
                else
                    ballVelocityAdd *= -currentState.Triggers.Left;

                // Finally, add this vector to our velocity.
                ballVelocity += ballVelocityAdd;

                //Vector3 normVel = new Vector3();

                //normVel = modelVelocity;

                //normVel.Normalize();

                //if ((Math.Abs(normVel.X) > .5) || (Math.Abs(normVel.Z) > .5))
                //{
                //    Debug.WriteLine("have some norm vel");
                //}

                //Debug.WriteLine(modelVelocity);

                GamePad.SetVibration(PlayerIndex.One,
                    currentState.Triggers.Right,
                    currentState.Triggers.Right);


                // In case you get lost, press A to warp back to the center.
                if (currentState.Buttons.A == ButtonState.Pressed)
                {
                    ballPosition = Vector3.Zero;
                    ballVelocity = Vector3.Zero;
                    ballRotation = 0.0f;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            Matrix[] transforms = new Matrix[ball.Bones.Count];
            ball.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in ball.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] *
                         Matrix.CreateRotationZ(ballRotation)
                        * Matrix.CreateTranslation(ballPosition) * Matrix.CreateScale(.5f);
                    effect.View = Matrix.CreateLookAt(cameraPosition,
                        Vector3.Zero, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), aspectRatio,
                        1.0f, 10000.0f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }

            Matrix[] groundTransforms = new Matrix[ground.Bones.Count];
            ground.CopyAbsoluteBoneTransformsTo(groundTransforms);

            foreach (ModelMesh mesh in ground.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = groundTransforms[mesh.ParentBone.Index] *
                         Matrix.CreateRotationY(groundRotation)
                        * Matrix.CreateTranslation(groundPosition) * Matrix.CreateScale(1.0f);
                    effect.View = Matrix.CreateLookAt(cameraPosition,
                        Vector3.Zero, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), aspectRatio,
                        1.0f, 10000.0f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
