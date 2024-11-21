using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI411.SimpleEngine;

namespace Lab06
{
    public class Lab06 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Lab 01
        Effect effect;

        // Lab02
        float angleXZ = 1;
        float angleYZ;
        float distance = 2;
        Vector3 cameraPos = new Vector3(0, 0, 0);
        Matrix world, view, projection;

        // Lab03
        Model model;
        MouseState previousMouseState;
        SpriteFont font;

        // Lab05
        Skybox skybox;

        // Lab06
        Texture2D texture;

        public Lab06()
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

            string[] skyboxTextures =
            {
                "Skybox/SunsetPNG2", "Skybox/SunsetPNG1",
                "Skybox/SunsetPNG4", "Skybox/SunsetPNG3",
                "Skybox/SunsetPNG6", "Skybox/SunsetPNG5",
            };

            skybox = new Skybox(skyboxTextures, Content, GraphicsDevice);

            model = Content.Load<Model>("Helicopter");
            effect = Content.Load<Effect>("Reflection");
            texture = Content.Load<Texture2D>("HelicopterTexture");
            font = Content.Load<SpriteFont>("Font");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseState currentMouseState = Mouse.GetState();

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed)
            {
                angleXZ -= (currentMouseState.X - previousMouseState.X) / 100f;
                angleYZ -= (currentMouseState.Y - previousMouseState.Y) / 100f;
            }

            if (Mouse.GetState().RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Pressed)
            {
                distance += (currentMouseState.Y - previousMouseState.Y) / 10f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                angleXZ += 0.1f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                angleXZ -= 0.1f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                angleYZ += 0.1f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                angleYZ -= 0.1f;
            }

            if (Mouse.GetState().RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Pressed)
            {
                distance += (currentMouseState.Y - previousMouseState.Y) / 10f;
            }

            cameraPos = Vector3.Transform(
              new Vector3(0, 0, distance),
              Matrix.CreateRotationX(angleYZ) * Matrix.CreateRotationY(angleXZ)
            );

            view = Matrix.CreateLookAt(cameraPos, Vector3.Zero, Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angleYZ) * Matrix.CreateRotationY(angleXZ)));

            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(90),
                GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                100
            );

            previousMouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        protected void DrawModelWithEffect()
        {
            effect.CurrentTechnique = effect.Techniques[0];
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        // Set Parameters in Effect
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        effect.Parameters["CameraPosition"].SetValue(cameraPos);
                        effect.Parameters["environmentMap"].SetValue(skybox.skyBoxTexture);
                        effect.Parameters["decalMap"].SetValue(texture);
                        Matrix worldInverseTranspose =
                            Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTranspose);
                        //

                        pass.Apply();
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            part.VertexOffset,
                            part.StartIndex,
                            part.PrimitiveCount
                        );
                    }
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.Opaque;

            RasterizerState originalRasterizerState = GraphicsDevice.RasterizerState;

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            skybox.Draw(view, projection, cameraPos);

            GraphicsDevice.RasterizerState = originalRasterizerState;
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            DrawModelWithEffect();
            //model.Draw(world, view, projection);

            // 2D Drawing
            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, ("angleXZ:" + angleXZ).ToString(), Vector2.UnitX +
            Vector2.UnitY * 12, Color.White);
            _spriteBatch.DrawString(font, ("angleYZ:" + angleYZ).ToString(), Vector2.UnitX +
            Vector2.UnitY + new Vector2(0, 3) * 12, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}