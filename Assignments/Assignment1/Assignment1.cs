using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Assignment1
{
    public class Assignment1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Assignment1
        int help = 1;
        int info = 1;

        // Lab04
        int mode;
        int modelnum;

        // Lab03
        Model[] models = new Model[5];
        Model currModel;
        SpriteFont font;
        MouseState previousMouseState;
        KeyboardState previousKeyboardState;
        float translateX = 0;
        float translateY = 0;

        /* Color Data Set*/
        Vector4 ambient = new Vector4(0, 0, 0, 0);
        float ambientIntensity = 0.1f;
        Vector4 diffuseColor = new Vector4(1, 1, 1, 1);
        float diffuseIntensity = 0.8f;
        Vector3 lightPos = new Vector3(0, 10, 0);

        float shininess = 10.0f;
        Vector4 specularColor = new Vector4(1, 1, 1, 1);
        float specularIntensity = 1.0f;

        // Lab02
        float angleXZ = 0;
        float angleYZ = 0;
        float lightAngleXZ = 0;
        float lightAngleYZ = 0;
        float distance = 20;
        //Vector3 cameraPosition = new Vector3(0, 0, 2);
        Vector3 camera;
        Matrix world, view, projection;

        // Lab 01
        Effect effect;

        public Assignment1()
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

            models[0] = Content.Load<Model>("box");
            models[1] = Content.Load<Model>("sphere");
            models[2] = Content.Load<Model>("torus");
            models[3] = Content.Load<Model>("teapot");
            models[4] = Content.Load<Model>("bunny");

            currModel = models[0];

            effect = Content.Load<Effect>("Shaders");
            font = Content.Load<SpriteFont>("Font");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.F1)) mode = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.F2)) mode = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.F3)) mode = 2;
            if (Keyboard.GetState().IsKeyDown(Keys.F4)) mode = 3;
            if (Keyboard.GetState().IsKeyDown(Keys.F5)) mode = 4;
            if (Keyboard.GetState().IsKeyDown(Keys.F6)) mode = 5;

            if (Keyboard.GetState().IsKeyDown(Keys.D1)) modelnum = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.D2)) modelnum = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.D3)) modelnum = 2;
            if (Keyboard.GetState().IsKeyDown(Keys.D4)) modelnum = 3;
            if (Keyboard.GetState().IsKeyDown(Keys.D5)) modelnum = 4;
            if (Keyboard.GetState().IsKeyDown(Keys.D6)) modelnum = 5;

            if (Keyboard.GetState().IsKeyDown(Keys.H) && !previousKeyboardState.IsKeyDown(Keys.H) && info == 1) info = 0;
            else if (Keyboard.GetState().IsKeyDown(Keys.H) && !previousKeyboardState.IsKeyDown(Keys.H)) info = 1;

            if (Keyboard.GetState().IsKeyDown(Keys.OemQuestion) && !previousKeyboardState.IsKeyDown(Keys.OemQuestion) && help == 1) help = 0;
            else if (Keyboard.GetState().IsKeyDown(Keys.OemQuestion) && !previousKeyboardState.IsKeyDown(Keys.OemQuestion)) help = 1;

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
                translateX -= (currentMouseState.X - previousMouseState.X) / 100f;
                translateY -= (currentMouseState.Y - previousMouseState.Y) / 100f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                lightAngleXZ -= 0.1f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                lightAngleXZ += 0.1f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                lightAngleYZ -= 0.1f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                lightAngleYZ += 0.1f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus) && Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                if (shininess - 0.1f < 1.1f && effect.CurrentTechnique.Name == "Schlick")
                {
                    shininess = 1.1f;
                }
                
                else if (shininess - 0.1f < 0.0f)
                {
                    shininess = 0.0f;
                }

                else
                {
                    shininess -= 0.1f;
                }             
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.OemPlus) && Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                shininess += 0.1f;
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                if (specularIntensity - 0.01f < 0.00f)
                {
                    specularIntensity = 0.00f;
                } 
                
                else
                {
                    specularIntensity -= 0.01f;
                }
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                specularIntensity += 0.01f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.L) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                diffuseIntensity -= 0.01f;
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.L))
            {
                diffuseIntensity += 0.01f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.R) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                //diffuseColor -= new Vector4(0.1f, 0, 0, 0);
                diffuseColor.X -= 0.01f;
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                diffuseColor.X += 0.01f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.G) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                diffuseColor.Y -= 0.01f;
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.G))
            {
                diffuseColor.Y += 0.01f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.B) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                diffuseColor.Z -= 0.01f;
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.B))
            {
                diffuseColor.Z += 0.01f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                distance = 20;
                camera = new Vector3(0, 0, distance);
                lightPos = new Vector3(0, 10, 0);

                angleXZ = 0;
                angleYZ = 0;
                lightAngleXZ = 0;
                lightAngleYZ = 0;
                
                translateX = 0;
                translateY = 0;

                diffuseColor = new Vector4(1, 1, 1, 1);
                diffuseIntensity = 0.8f;

                shininess = 10.0f;
                specularColor = new Vector4(1, 1, 1, 1);
                specularIntensity = 1.0f;
            }

            /*Vector3 ax1 = Vector3.Cross(new Vector3(translateX, translateY, distance), Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angleYZ) * Matrix.CreateRotationY(angleXZ)));
            Vector3 ax2 = Vector3.Cross(new Vector3(translateX, translateY, distance), ax1);

            float theta_1_x = (float)System.Math.Acos(Vector3.Dot(Vector3.Normalize(ax1), Vector3.UnitX));
            float theta_1_y = (float) System.Math.Acos(Vector3.Dot(Vector3.Normalize(ax1), Vector3.UnitY));
            float theta_2_x = (float)System.Math.Acos(Vector3.Dot(Vector3.Normalize(ax2), Vector3.UnitX));
            float theta_2_y = (float)System.Math.Acos(Vector3.Dot(Vector3.Normalize(ax2), Vector3.UnitY));

            camera = Vector3.Transform(
               new Vector3(translateX, translateY, distance),
               Matrix.CreateRotationX(theta_1_x +  theta_2_x + angleYZ) * Matrix.CreateRotationY(theta_1_y + theta_2_y + angleXZ)
               );*/

            camera = Vector3.Transform(
               new Vector3(0, 0, distance),
               Matrix.CreateRotationX(angleYZ) * Matrix.CreateRotationY(angleXZ) * Matrix.CreateTranslation(translateX, translateY, 0)
               );

            view = Matrix.CreateLookAt(camera, Vector3.Zero, Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angleYZ) * Matrix.CreateRotationY(angleXZ)));

            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(90),
                GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                100
            );

            lightPos = Vector3.Transform(new Vector3(0, 10, 0), Matrix.CreateRotationX(lightAngleYZ) * Matrix.CreateRotationY(lightAngleXZ));

            previousMouseState = Mouse.GetState();
            previousKeyboardState = Keyboard.GetState();

            currModel = models[modelnum];

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
                foreach (ModelMesh mesh in currModel.Meshes)
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
                        //effect.Parameters["SpecularColor"].SetValue(specularColor);
                        effect.Parameters["SpecularColor"].SetValue(diffuseColor);
                        effect.Parameters["SpecularIntensity"].SetValue(specularIntensity);
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

            if (info == 1)
            {
                _spriteBatch.DrawString(font, ("angleXZ:" + angleXZ).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY * 12, Color.White);
                _spriteBatch.DrawString(font, ("angleYZ:" + angleYZ).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 3) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Light angleXZ:" + lightAngleXZ).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 5) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Light angleYZ:" + lightAngleYZ).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 7) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Shader:" + effect.CurrentTechnique.Name).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 9) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Diffuse Intensity:" + diffuseIntensity).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 11) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Specular Intensity:" + specularIntensity).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 13) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Shininess (Schlick shader restricted to >1):" + shininess).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 15) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Light - Red Value:" + diffuseColor.X).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 17) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Light - Green Value:" + diffuseColor.Y).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 19) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Light - Blue Value:" + diffuseColor.Z).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 21) * 12, Color.White);
            }

            if (help == 1)
            {
                _spriteBatch.DrawString(font, ("Rotate Camera: Mouse Left Drag").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Camera Distance: Mouse Right Drag").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 3) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Translate Camera: Mouse Middle Drag").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 5) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Rotate Light Around X Axis: Up/Down Arrows").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 7) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Rotate Light Around Y Axis: Left/Right Arrows").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 9) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Reset Light and Camera: S").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 11) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Model: Num Keys 1-6").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 13) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Shader: Function Keys F1-F6").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 15) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Change Diffuse Light Intensity: L/LShift+L").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 17) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Specular Light Intensity: +/-").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 19) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Shininess: LCtrl and +/LCtrl and -").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 21) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Red Value of Light Color: R/LShift+R").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 23) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Green Value of Light Color: G/LShift+G").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 25) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Blue Value of Light Color: B/LShift+B").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 27) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Show/Hide Shader Values: H").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 29) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Show/Hide Help: ?").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 31) * 12, Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
