using CPI411.SimpleEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Assignment2
{
    public class Assignment2 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Assignment2
        float reflectivity = 0.5f;
        float fresnelBias = 0.3f;
        float fresnelPower = 1.8f;
        float fresnelScale = 2.4f;
        Vector3 etaRatio = new Vector3(0.77f, 0.77f, 0.77f);

        // Lab05
        Skybox[] skybox = new Skybox[4];

        // Lab06
        Texture2D heliTexture;

        // Assignment1
        int help = 1;
        int info = 1;

        // Lab04
        int mode;
        int modelnum;
        int skyboxnum = 0;

        // Lab03
        Model[] models = new Model[6];
        Model currModel;
        SpriteFont font;
        MouseState previousMouseState;
        KeyboardState previousKeyboardState;

        // Lab02
        float angleXZ = 0;
        float angleYZ = 0;
        float distance = 20;
        //Vector3 cameraPosition = new Vector3(0, 0, 2);
        Vector3 camera;
        Vector3 cameraTarget;
        Matrix world, view, projection;

        // Lab 01
        Effect effect;

        public Assignment2()
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

            string[] skyboxTextures1 =
            {
                "EnvironmentMaps/test_negx", "EnvironmentMaps/test_posx",
                "EnvironmentMaps/test_negy", "EnvironmentMaps/test_posy",
                "EnvironmentMaps/test_negz", "EnvironmentMaps/test_posz",
            };

            skybox[0] = new Skybox(skyboxTextures1, Content, GraphicsDevice);

            string[] skyboxTextures2 =
            {
                "EnvironmentMaps/nvlobby_new_negx", "EnvironmentMaps/nvlobby_new_posx",
                "EnvironmentMaps/nvlobby_new_negy", "EnvironmentMaps/nvlobby_new_posy",
                "EnvironmentMaps/nvlobby_new_negz", "EnvironmentMaps/nvlobby_new_posz",
            };

            skybox[1] = new Skybox(skyboxTextures2, Content, GraphicsDevice);

            string[] skyboxTextures3 =
            {
                "EnvironmentMaps/grandcanyon_negx", "EnvironmentMaps/grandcanyon_posx",
                "EnvironmentMaps/grandcanyon_negy", "EnvironmentMaps/grandcanyon_posy",
                "EnvironmentMaps/grandcanyon_negz", "EnvironmentMaps/grandcanyon_posz",
            };

            skybox[2] = new Skybox(skyboxTextures3, Content, GraphicsDevice);

            string[] skyboxTextures4 =
            {
                "EnvironmentMaps/vz_clear_ocean_negx", "EnvironmentMaps/vz_clear_ocean_posx",
                "EnvironmentMaps/vz_clear_ocean_negy", "EnvironmentMaps/vz_clear_ocean_posy",
                "EnvironmentMaps/vz_clear_ocean_negz", "EnvironmentMaps/vz_clear_ocean_posz",
            };

            skybox[3] = new Skybox(skyboxTextures4, Content, GraphicsDevice);

            heliTexture = Content.Load<Texture2D>("HelicopterTexture");

            models[0] = Content.Load<Model>("box");
            models[1] = Content.Load<Model>("sphere");
            models[2] = Content.Load<Model>("torus");
            models[3] = Content.Load<Model>("teapot");
            models[4] = Content.Load<Model>("bunnyUV");
            models[5] = Content.Load<Model>("Helicopter");

            currModel = models[5];

            effect = Content.Load<Effect>("Shaders");
            font = Content.Load<SpriteFont>("Font");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.F7)) mode = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.F8)) mode = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.F9)) mode = 2;
            if (Keyboard.GetState().IsKeyDown(Keys.F10)) mode = 3;

            if (Keyboard.GetState().IsKeyDown(Keys.D1)) modelnum = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.D2)) modelnum = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.D3)) modelnum = 2;
            if (Keyboard.GetState().IsKeyDown(Keys.D4)) modelnum = 3;
            if (Keyboard.GetState().IsKeyDown(Keys.D5)) modelnum = 4;
            if (Keyboard.GetState().IsKeyDown(Keys.D6)) modelnum = 5;
            //if (Keyboard.GetState().IsKeyDown(Keys.D7)) modelnum = 6;

            if (Keyboard.GetState().IsKeyDown(Keys.D7)) skyboxnum = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.D8)) skyboxnum = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.D9)) skyboxnum = 2;
            if (Keyboard.GetState().IsKeyDown(Keys.D0)) skyboxnum = 3;

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
                Vector3 ViewRight = Vector3.Transform(Vector3.UnitX, Matrix.CreateRotationX(angleYZ) * Matrix.CreateRotationY(angleXZ));
                Vector3 ViewUp = Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angleYZ) * Matrix.CreateRotationY(angleXZ));
                cameraTarget -= ViewRight * (currentMouseState.X - previousMouseState.X) / 10f;
                cameraTarget -= ViewUp * (currentMouseState.Y - previousMouseState.Y) / 10f;
            }




            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                if (reflectivity <= 0.00f)
                {
                    reflectivity = 0.00f;
                } 
                
                else
                {
                    reflectivity -= 0.01f;
                }
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                if (reflectivity >= 1.00f)
                {
                    reflectivity = 1.00f;
                }

                else
                {
                    reflectivity += 0.01f;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.R) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                if (etaRatio.X <= 0.01f)
                    etaRatio.X = 0.01f;
                else
                    etaRatio.X -= 0.01f;
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                etaRatio.X += 0.01f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.G) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                if (etaRatio.Y <= 0.01f)
                    etaRatio.Y= 0.01f;
                else
                    etaRatio.Y -= 0.01f;
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.G))
            {
                etaRatio.Y += 0.01f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.B) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                if (etaRatio.Z <= 0.01f)
                    etaRatio.Z = 0.01f;
                else
                    etaRatio.Z -= 0.01f;
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.B))
            {
                etaRatio.Z += 0.01f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Q) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                if (fresnelPower <= 0.0f)
                    fresnelPower = 0.0f;
                else
                    fresnelPower -= 0.1f;
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                fresnelPower += 0.1f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                if (fresnelScale <= 0.0f)
                    fresnelScale = 0.0f;
                else
                    fresnelScale -= 0.1f;
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                fresnelScale += 0.1f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.E) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                fresnelBias -= 0.01f;
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                fresnelBias += 0.01f;
            }




            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                distance = 20;
                camera = new Vector3(0, 0, distance);
                cameraTarget = Vector3.Zero;
                angleXZ = 0;
                angleYZ = 0;

                reflectivity = 0.5f;
                etaRatio = new Vector3(0.77f, 0.77f, 0.77f);
                fresnelPower = 1.8f;
                fresnelBias = 0.3f;
                fresnelScale = 2.4f;

            }

            camera = Vector3.Transform(
               new Vector3(0, 0, distance),
               Matrix.CreateRotationX(angleYZ) * Matrix.CreateRotationY(angleXZ) * Matrix.CreateTranslation(cameraTarget)
               );

            view = Matrix.CreateLookAt(camera, cameraTarget, Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angleYZ) * Matrix.CreateRotationY(angleXZ)));

            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(90),
                GraphicsDevice.Viewport.AspectRatio,
                0.1f,
                100
            );

            previousMouseState = Mouse.GetState();
            previousKeyboardState = Keyboard.GetState();

            currModel = models[modelnum];

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.Opaque;

            RasterizerState originalRasterizerState = GraphicsDevice.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            skybox[skyboxnum].Draw(view, projection, camera);

            GraphicsDevice.RasterizerState = originalRasterizerState;

            GraphicsDevice.DepthStencilState = new DepthStencilState();

            //DrawModelWithEffect();

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

                        effect.Parameters["environmentMap"].SetValue(skybox[skyboxnum].skyBoxTexture);
                        effect.Parameters["decalMap"].SetValue(heliTexture);

                        effect.Parameters["reflectivity"].SetValue(reflectivity);
                        effect.Parameters["bias"].SetValue(fresnelBias);
                        effect.Parameters["power"].SetValue(fresnelPower);
                        effect.Parameters["scale"].SetValue(fresnelScale);
                        effect.Parameters["etaRatio"].SetValue(etaRatio);

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

                _spriteBatch.DrawString(font, ("Shader:" + effect.CurrentTechnique.Name).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 9) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Reflectivity:" + reflectivity).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 11) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Fresnel Power:" + fresnelPower).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 13) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Fresnel Scale:" + fresnelScale).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 15) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Fresnel Bias:" + fresnelBias).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 17) * 12, Color.White);

                _spriteBatch.DrawString(font, ("etaRatio - Red Value (default etaRatio):" + etaRatio.X).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 19) * 12, Color.White);
                _spriteBatch.DrawString(font, ("etaRatio - Green Value:" + etaRatio.Y).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 21) * 12, Color.White);
                _spriteBatch.DrawString(font, ("etaRatio - Blue Value:" + etaRatio.Z).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 23) * 12, Color.White);
            }

            if (help == 1)
            {
                _spriteBatch.DrawString(font, ("Rotate Camera: Mouse Left Drag").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Camera Distance: Mouse Right Drag").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 3) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Translate Camera: Mouse Middle Drag").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 5) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Reset Camera: S").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 7) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Model: Num Keys 1-6").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 9) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Skybox: Num Keys 7-9,0").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 11) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Shader: Function Keys F7-F10").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 13) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Change Reflectivity: +/-").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 15) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change etaRatio of Red Light (default etaRatio").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 17) * 12, Color.White);
                _spriteBatch.DrawString(font, ("  for refraction without dispersion): R/LShift+R").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 19) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change etaRatio of Green Light: G/LShift+G").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 21) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change etaRatio of Blue Light: B/LShift+B").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 23) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Change Fresnel Power: Q/LShift+Q").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 25) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Fresnel Scale: W/LShift+W").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 27) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Fresnel Bias: E/LShift+E").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 29) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Show/Hide Shader Values: H").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 31) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Show/Hide Help: ?").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 33) * 12, Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
