﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI411.SimpleEngine;

namespace Lab10
{
    public class Lab10 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Lab 10
        ParticleManager particleManager;
        System.Random random;
        Vector3 particlePosition;

        SpriteFont font;
        Effect effect;
        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(
            new Vector3(0, 0, 20),
            new Vector3(0, 0, 0),
            Vector3.UnitY);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(45),
            800f / 600f,
            0.1f,
            100f);
        Vector3 cameraPosition, cameraTarget, lightPosition;
        Matrix lightView;
        Matrix lightProjection;
        float angle = 0;
        float angle2 = 0;
        float angleL = 0;
        float angleL2 = 0;
        float distance = 20;
        MouseState preMouse;
        Model model;
        Texture2D texture;

        public Lab10()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            effect = Content.Load<Effect>("ParticleShader");
            texture = Content.Load<Texture2D>("fire");
            model = Content.Load<Model>("torus");
            random = new System.Random();
            particleManager = new ParticleManager(GraphicsDevice, 100);
            particlePosition = new Vector3(0, 0, 0);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                Particle particle = particleManager.getNext();
                particle.Position = particlePosition;
                particle.Velocity = Vector3.Transform(new Vector3(random.Next(-2, 2), random.Next(-2, 2), random.Next(-2, 2)), Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                //particle.Acceleration = new Vector3(0, 0, 0);
                particle.Acceleration = Vector3.Transform(new Vector3(1, 2, 0), Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                particle.MaxAge = 5;
                particle.Init();
            }

            particleManager.Update(gameTime.ElapsedGameTime.Milliseconds * 0.001f);


            if (Keyboard.GetState().IsKeyDown(Keys.Left)) angleL += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) angleL -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) angleL2 += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) angleL2 -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.S)) { angle = angle2 = angleL = angleL2 = 0; distance = 10; cameraTarget = Vector3.Zero; }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                angle -= (Mouse.GetState().X - preMouse.X) / 100f;
                angle2 += (Mouse.GetState().Y - preMouse.Y) / 100f;
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                distance += (Mouse.GetState().Y - preMouse.Y) / 100f;
            }

            if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
            {
                Vector3 ViewRight = Vector3.Transform(Vector3.UnitX,
                    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                Vector3 ViewUp = Vector3.Transform(Vector3.UnitY,
                    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                cameraTarget -= ViewRight * (Mouse.GetState().X - preMouse.X) / 10f;
                cameraTarget += ViewUp * (Mouse.GetState().Y - preMouse.Y) / 10f;
            }
            preMouse = Mouse.GetState();
            cameraPosition = Vector3.Transform(new Vector3(0, 0, distance),
                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle) * Matrix.CreateTranslation(cameraTarget));
            view = Matrix.CreateLookAt(
                cameraPosition,
                cameraTarget,
                Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle)));
            lightPosition = Vector3.Transform(
                new Vector3(0, 0, 10),
                Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL));
            //lightView = Matrix.CreateLookAt(lightPosition, Vector3.Zero, Vector3.UnitY);

            lightView = Matrix.CreateLookAt(
                lightPosition,
                Vector3.Zero,
                Vector3.Transform(
                    Vector3.UnitY,
                    Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL)
                )
            );

            lightProjection =
                Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1f, 1f, 50f);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = new DepthStencilState();
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    model.Draw(mesh.ParentBone.Transform, view, projection);
                }
            }          

            effect.CurrentTechnique = effect.Techniques["Particle"];
            effect.CurrentTechnique.Passes[0].Apply();
            effect.Parameters["World"].SetValue(Matrix.Identity);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
            //effect.Parameters["InverseCamera"].SetValue(Matrix.Identity);
            effect.Parameters["InverseCamera"].SetValue(Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
            effect.Parameters["Texture"].SetValue(texture);

            particleManager.Draw(GraphicsDevice);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            base.Draw(gameTime);
        }
    }
}