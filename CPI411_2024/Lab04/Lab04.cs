using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab04
{
    public class Lab04 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Lab04
        int mode;

        // Lab03
        Model model;
        SpriteFont font;
        MouseState previousMouseState;
        float translateX;
        float translateY;

        /* Color Data Set*/
        Vector4 ambient = new Vector4(0, 0, 0, 0);
        float ambientIntensity = 0.1f;
        Vector4 diffuseColor = new Vector4(1, 1, 1, 1);
        float diffuseIntensity = 0.8f;
        Vector3 lightPos = new Vector3(10, 10, 10);

        float shininess = 10.0f;
        Vector4 specularColor = new Vector4(1, 1, 1, 1);

        // Lab02
        float angleXZ;
        float angleYZ;
        float distance = 13;
        //Vector3 cameraPosition = new Vector3(0, 0, 2);
        Vector3 camera;
        Matrix world, view, projection;

        // Lab 01
        Effect effect;

        public Lab04()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            // Lab 01
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            //
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            model = Content.Load<Model>("torus");
            effect = Content.Load<Effect>("SimpleShading");
            font = Content.Load<SpriteFont>("Font");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.D0)) mode = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.D1)) mode = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.D2)) mode = 2;

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

            if (Mouse.GetState().MiddleButton == ButtonState.Pressed && previousMouseState.MiddleButton == ButtonState.Pressed)
            {
                translateX -= (currentMouseState.X - previousMouseState.X) / 10f;
                translateY -= (currentMouseState.Y - previousMouseState.Y) / 10f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.G) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                shininess -= 0.2f;
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.G))
            {
                shininess += 0.2f;
            }

            camera = Vector3.Transform(
               new Vector3(0, 0, distance),
               Matrix.CreateRotationX(angleYZ) * Matrix.CreateRotationY(angleXZ) * Matrix.CreateTranslation(translateX, translateY, 0)
               );

            view = Matrix.CreateLookAt(camera, Vector3.Zero, Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angleYZ) * Matrix.CreateRotationY(angleXZ)));
            //view = Matrix.CreateLookAt(camera, Vector3.Zero, Vector3.UnitY);

            /*cameraPosition = distance * new Vector3(
                (float)System.Math.Sin(angleXZ), (float)System.Math.Sin(angleYZ), (float)System.Math.Cos(angleYZ)
            );*/

            //world = Matrix.Identity;
            /*view = Matrix.CreateLookAt(
                cameraPosition,
                new Vector3(),
                new Vector3(0, 1, 0)
            );*/
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(90),
                GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                100
            );

            previousMouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            //model.Draw(world, view, projection);

            effect.CurrentTechnique = effect.Techniques[mode];
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
                        effect.Parameters["CameraPosition"].SetValue(camera);
                        effect.Parameters["AmbientColor"].SetValue(ambient);
                        effect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);
                        effect.Parameters["DiffuseLightDirection"].SetValue(lightPos);
                        effect.Parameters["DiffuseColor"].SetValue(diffuseColor);
                        effect.Parameters["DiffuseIntensity"].SetValue(diffuseIntensity);
                        effect.Parameters["SpecularColor"].SetValue(specularColor);
                        effect.Parameters["Shininess"].SetValue(shininess);
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