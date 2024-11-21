using CPI411.SimpleEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Assignment3
{
    public class Assignment3 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Assignment 3
        Texture2D[] normalMaps = new Texture2D[8];
        Texture2D currNormalMap;
        int mipMapToggle = 1;
        float scaleU = 1.0f;
        float scaleV = 1.0f;
        float bumpHeight = 5.0f;
        int normalizeTangentToggle = 1;
        int normalizeNormalToggle = 1;
        int showNormalMap = 0;

        // Assignment2
        float reflectivity = 0.5f;
        float etaRatio = 0.77f;

        // Lab05
        Skybox skybox;

        // Assignment1
        int help = 1;
        int info = 1;

        // Lab04
        int mode;
        int modelnum = 4;

        // Lab03
        Model model;
        SpriteFont font;
        MouseState previousMouseState;
        KeyboardState previousKeyboardState;

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
        Vector3 cameraTarget;
        Matrix world, view, projection;

        // Lab 01
        Effect effect;

        public Assignment3()
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

            string[] skyboxTextures =
            {
                "EnvironmentMaps/nvlobby_new_negx", "EnvironmentMaps/nvlobby_new_posx",
                "EnvironmentMaps/nvlobby_new_negy", "EnvironmentMaps/nvlobby_new_posy",
                "EnvironmentMaps/nvlobby_new_negz", "EnvironmentMaps/nvlobby_new_posz",
            };

            skybox = new Skybox(skyboxTextures, Content, GraphicsDevice);

            model = Content.Load<Model>("torus");

            effect = Content.Load<Effect>("Shaders");
            font = Content.Load<SpriteFont>("Font");

            normalMaps[0] = Content.Load<Texture2D>("NormalMaps/art");
            normalMaps[1] = Content.Load<Texture2D>("NormalMaps/BumpTest");
            normalMaps[2] = Content.Load<Texture2D>("NormalMaps/crossHatch");
            normalMaps[3] = Content.Load<Texture2D>("NormalMaps/monkey");
            normalMaps[4] = Content.Load<Texture2D>("NormalMaps/round");
            normalMaps[5] = Content.Load<Texture2D>("NormalMaps/saint");
            normalMaps[6] = Content.Load<Texture2D>("NormalMaps/science");
            normalMaps[7] = Content.Load<Texture2D>("NormalMaps/square");

            currNormalMap = normalMaps[4];
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                showNormalMap = 0;
                mode = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F2))
            {
                showNormalMap = 0;
                mode = 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F3))
            {
                showNormalMap = 0;
                normalizeNormalToggle = 1;
                normalizeTangentToggle = 1;
                mode = 2;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F4))
            {
                showNormalMap = 0;
                mode = 3;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F5))
            {
                showNormalMap = 0;
                mode = 4;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F6))
            {
                showNormalMap = 0;
                normalizeNormalToggle = 1;
                normalizeTangentToggle = 0;
                mode = 2;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F7))
            {
                showNormalMap = 0;
                normalizeNormalToggle = 0;
                normalizeTangentToggle = 0;
                mode = 2;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F8))
            {
                showNormalMap = 0;
                normalizeNormalToggle = 0;
                normalizeTangentToggle = 1;
                mode = 2;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F9))
            {
                showNormalMap = 0;
                normalizeNormalToggle = 2;
                normalizeTangentToggle = 1;
                mode = 2;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F10) && !previousKeyboardState.IsKeyDown(Keys.F10) && showNormalMap == 0) showNormalMap = 1;
            else if (Keyboard.GetState().IsKeyDown(Keys.F10) && !previousKeyboardState.IsKeyDown(Keys.F10) && showNormalMap == 1) showNormalMap = 0;


            if (Keyboard.GetState().IsKeyDown(Keys.D1)) modelnum = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.D2)) modelnum = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.D3)) modelnum = 2;
            if (Keyboard.GetState().IsKeyDown(Keys.D4)) modelnum = 3;
            if (Keyboard.GetState().IsKeyDown(Keys.D5)) modelnum = 4;
            if (Keyboard.GetState().IsKeyDown(Keys.D6)) modelnum = 5;
            if (Keyboard.GetState().IsKeyDown(Keys.D7)) modelnum = 6;
            if (Keyboard.GetState().IsKeyDown(Keys.D8)) modelnum = 7;

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


            if (Keyboard.GetState().IsKeyDown(Keys.M) && !previousKeyboardState.IsKeyDown(Keys.M))
            {
                if (mipMapToggle == 1)
                    mipMapToggle = 0;
                else
                    mipMapToggle = 1;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.U) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                scaleU -= 0.02f;
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.U))
            {
                scaleU += 0.02f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.V) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                scaleV -= 0.02f;
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.V))
            {
                scaleV += 0.02f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                if (bumpHeight <= 1.0f)
                    bumpHeight = 1.0f;
                else
                    bumpHeight -= 0.1f;
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                if (bumpHeight >= 10.0f)
                    bumpHeight = 10.0f;
                else
                    bumpHeight += 0.1f;
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

            if (Keyboard.GetState().IsKeyDown(Keys.E) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                if (etaRatio <= 0.01f)
                    etaRatio = 0.01f;
                else
                    etaRatio -= 0.01f;
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                etaRatio += 0.01f;
            }




            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                distance = 20;
                camera = new Vector3(0, 0, distance);
                lightPos = new Vector3(0, 10, 0);
                cameraTarget = Vector3.Zero;
                angleXZ = 0;
                angleYZ = 0;
                lightAngleXZ = 0;
                lightAngleYZ = 0;

                reflectivity = 0.5f;
                etaRatio = 0.77f;

                diffuseColor = new Vector4(1, 1, 1, 1);
                diffuseIntensity = 0.8f;

                shininess = 10.0f;
                specularColor = new Vector4(1, 1, 1, 1);
                specularIntensity = 1.0f;

                mipMapToggle = 1;
                scaleU = 1.0f;
                scaleV = 1.0f;
                bumpHeight = 5.0f;

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

            lightPos = Vector3.Transform(new Vector3(0, 0, 10), Matrix.CreateRotationX(lightAngleYZ) * Matrix.CreateRotationY(lightAngleXZ));

            previousMouseState = Mouse.GetState();
            previousKeyboardState = Keyboard.GetState();

            if (modelnum != 8)
                currNormalMap = normalMaps[modelnum];

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (showNormalMap != 0)
            {
                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
                using (SpriteBatch sprite = new SpriteBatch(GraphicsDevice))
                {
                    sprite.Begin();
                    sprite.Draw(currNormalMap, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 1);
                    sprite.End();
                }
            }
            else
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);

                GraphicsDevice.BlendState = BlendState.Opaque;

                RasterizerState originalRasterizerState = GraphicsDevice.RasterizerState;
                RasterizerState rasterizerState = new RasterizerState();
                rasterizerState.CullMode = CullMode.None;
                GraphicsDevice.RasterizerState = rasterizerState;

                skybox.Draw(view, projection, camera);

                GraphicsDevice.RasterizerState = originalRasterizerState;

                GraphicsDevice.DepthStencilState = new DepthStencilState();

                //DrawModelWithEffect();

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
                            effect.Parameters["SpecularColor"].SetValue(diffuseColor);
                            effect.Parameters["SpecularIntensity"].SetValue(specularIntensity);
                            effect.Parameters["Shininess"].SetValue(shininess);

                            effect.Parameters["environmentMap"].SetValue(skybox.skyBoxTexture);
                            //effect.Parameters["decalMap"].SetValue(heliTexture);

                            effect.Parameters["reflectivity"].SetValue(reflectivity);
                            //effect.Parameters["bias"].SetValue(fresnelBias);
                            //effect.Parameters["power"].SetValue(fresnelPower);
                            //effect.Parameters["scale"].SetValue(fresnelScale);
                            effect.Parameters["etaRatio"].SetValue(etaRatio);

                            effect.Parameters["normalMap"].SetValue(currNormalMap);
                            effect.Parameters["mipmap"].SetValue(mipMapToggle);
                            effect.Parameters["ScaleU"].SetValue(scaleU);
                            effect.Parameters["ScaleV"].SetValue(scaleV); ;
                            effect.Parameters["BumpHeight"].SetValue(bumpHeight);

                            effect.Parameters["NormalizeTangentFrame"].SetValue(normalizeTangentToggle);
                            effect.Parameters["NormalizeNormalMap"].SetValue(normalizeNormalToggle);

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

                _spriteBatch.DrawString(font, ("Reflectivity:" + reflectivity).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 11) * 12, Color.White);

                _spriteBatch.DrawString(font, ("etaRatio:" + etaRatio).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 13) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Mip-Map Toggle:" + (mipMapToggle == 1).ToString()).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 17) * 12, Color.White);
                _spriteBatch.DrawString(font, ("U Texture Scale:" + scaleU).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 19) * 12, Color.White);
                _spriteBatch.DrawString(font, ("V Texture Scale:" + scaleV).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 21) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Bump Map Height Multiplier:" + (1.0 + 0.2 * (bumpHeight - 5)).ToString()).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 23) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Tangent Frame Normalization Toggle:" + (normalizeTangentToggle == 1).ToString()).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 27) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Diffuse Normal Normalization Toggle:" + (normalizeNormalToggle % 2 == 1).ToString()).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 29) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Specular Normal Normalization Toggle:" + (normalizeNormalToggle > 0).ToString()).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 31) * 12, Color.White);
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
                _spriteBatch.DrawString(font, ("Change Normal Map Texture: Num Keys 1-8").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 9) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Shader: Function Keys F1-F9").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 13) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Show/Hide Normal Map: Function Key F10").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 15) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Change Reflectivity: +/-").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 17) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change etaRatio: E/LShift+E").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 19) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Toggle Mip-Mapping: M").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 23) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change U Scale of Texture: U/LShift+U").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 25) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change V Scale of Texture: V/LShift+V").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 27) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Bump Height Multiplier: W/LShift+W").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 29) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Show/Hide Shader Values: H").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 33) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Show/Hide Help: ?").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 35) * 12, Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
