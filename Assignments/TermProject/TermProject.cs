using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Windows.Forms.VisualStyles;

namespace TermProject
{
    public class TermProject : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Term Project
        int depthLayerCount = 1;
        RenderTarget2D depthRenderTarget;
        RenderTarget2D normalRenderTarget;
        RenderTarget2D compositeRenderTarget;
        RenderTarget2D edgeMapRenderTarget;

        RenderTarget2D depth2;
        RenderTarget2D normal2;
        RenderTarget2D edgeMap2RenderTarget;

        RenderTarget2D layerBlendTarget;

        Texture2D depthAndColorMap;
        Texture2D testMap;
        Texture2D depthAndColorMap2;
        Texture2D testMap2;

        Texture2D edgeMap;
        Texture2D edgeMap2;
        Texture2D compositeEdgeMap;

        Texture2D layerBlend;
        Texture2D bgTexture;

        Effect edgeEffect;
        Effect testEffect;
        Effect compositeEffect;

        //
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
        float distance = 10;
        MouseState preMouse;
        KeyboardState previousKeyboardState;
        Model model;

        // Lab 12
        
        float offset = 800f / 256f;
        float SSAORad = 0.001f;

        VertexPositionTexture[] vertices =
            {
                new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1))
            };

        public TermProject()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 1200;
            //_graphics.Applychanges();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Font");
            model = Content.Load<Model>("objects");
            effect = Content.Load<Effect>("Shaders");
            edgeEffect = Content.Load<Effect>("EdgeMap");
            testEffect = Content.Load<Effect>("ZTest");
            compositeEffect = Content.Load<Effect>("Composite");
            bgTexture = Content.Load<Texture2D>("blueprint");

            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            depthRenderTarget = new RenderTarget2D(GraphicsDevice, 2048, 2048, false, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);
            normalRenderTarget = new RenderTarget2D(GraphicsDevice, 2048, 2048, false, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);
            compositeRenderTarget = new RenderTarget2D(GraphicsDevice, 2048, 2048, false, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);

            edgeMapRenderTarget = new RenderTarget2D(GraphicsDevice, 2048, 2048, false, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);
            edgeMap2RenderTarget = new RenderTarget2D(GraphicsDevice, 2048, 2048, false, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);
            layerBlendTarget = new RenderTarget2D(GraphicsDevice, 2048, 2048, false, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);

            depth2 = new RenderTarget2D(GraphicsDevice, 2048, 2048, false, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);
            normal2 = new RenderTarget2D(GraphicsDevice, 2048, 2048, false, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //if (Keyboard.GetState().IsKeyDown(Keys.F1)) effect = effectProj;
            //if (Keyboard.GetState().IsKeyDown(Keys.F2)) effect = effectShadow;

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

            if (Keyboard.GetState().IsKeyDown(Keys.D) && Keyboard.GetState().IsKeyDown(Keys.LeftShift) && !previousKeyboardState.IsKeyDown(Keys.D))
            {
                depthLayerCount--;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D) && !previousKeyboardState.IsKeyDown(Keys.D))
            {
                depthLayerCount++;
            }


            if (Keyboard.GetState().IsKeyDown(Keys.R) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                SSAORad -= 0.0001f;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                SSAORad += 0.0001f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.O) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                offset -= 0.1f;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.O))
            {
                offset += 0.1f;
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
                Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 800f / 600f, 1f, 100f);

            previousKeyboardState = Keyboard.GetState();

            base.Update(gameTime);
        }

        private void DrawDepthMap()
        {
            effect.CurrentTechnique = effect.Techniques["DepthMap"];

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                        pass.Apply();
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                        part.VertexOffset, part.StartIndex, part.PrimitiveCount);
                    }
                }
            }
        }

        private void DrawNormalMap()
        {
            effect.CurrentTechnique = effect.Techniques["NormalMap"];

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                        effect.Parameters["DepthTexture"].SetValue(depthAndColorMap);
                        pass.Apply();
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                        part.VertexOffset, part.StartIndex, part.PrimitiveCount);
                    }
                }
            }
        }

        private void DrawDepthLayer()
        {
            effect.CurrentTechnique = effect.Techniques["DepthPeeling"];

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                        effect.Parameters["DepthTexture"].SetValue(depthAndColorMap2);
                        pass.Apply();
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                        part.VertexOffset, part.StartIndex, part.PrimitiveCount);
                    }
                }
            }
        }

        private void DrawNormalLayer()
        {
            effect.CurrentTechnique = effect.Techniques["NormalPeeling"];

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {

                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        effect.Parameters["World"].SetValue(mesh.ParentBone.Transform);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform));
                        effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                        effect.Parameters["DepthTexture"].SetValue(depthAndColorMap2);
                        pass.Apply();
                        GraphicsDevice.SetVertexBuffer(part.VertexBuffer);
                        GraphicsDevice.Indices = part.IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                        part.VertexOffset, part.StartIndex, part.PrimitiveCount);
                    }
                }
            }
        }

        /*
        private void DrawZEdgeMap()
        {
            edgeEffect.CurrentTechnique = edgeEffect.Techniques["ZEdgeMap"];

            foreach (EffectPass pass in edgeEffect.CurrentTechnique.Passes)
            {
                edgeEffect.Parameters["modelTexture"].SetValue(depthAndColorMap2);
                edgeEffect.Parameters["imageWidth"].SetValue((float)depthAndColorMap2.Width);
                edgeEffect.Parameters["imageHeight"].SetValue((float)depthAndColorMap2.Height);

                Matrix projection = Matrix.CreateOrthographicOffCenter(0, 800, 600, 0, 0, 1);
                Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
                edgeEffectt.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);
                //pass.Apply();
            }

            _spriteBatch.Begin(0, null, null, null, null, edgeEffect);
            _spriteBatch.Draw(edgeMap, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            _spriteBatch.End();
        }

        private void DrawNormalEdgeMap()
        {
            edgeEffect.CurrentTechnique = edgeEffect.Techniques["NormalEdgeMap"];

            foreach (EffectPass pass in edgeEffect.CurrentTechnique.Passes)
            {
                edgeEffect.Parameters["modelTexture"].SetValue(testMap2);
                edgeEffect.Parameters["imageWidth"].SetValue((float)testMap2.Width);
                edgeEffect.Parameters["imageHeight"].SetValue((float)testMap2.Height);

                Matrix projection = Matrix.CreateOrthographicOffCenter(0, 800, 600, 0, 0, 1);
                Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
                edgeEffect.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);
                //pass.Apply();
            }

            _spriteBatch.Begin(0, null, null, null, null, edgeEffect);
            _spriteBatch.Draw(edgeMap, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            _spriteBatch.End();
        }
        */

        private void DrawEdgeMap()
        {
            edgeEffect.CurrentTechnique = edgeEffect.Techniques["EdgeDraw"];
            testEffect.CurrentTechnique = testEffect.Techniques["ZEdgeMap"];

            //foreach (EffectPass pass in edgeEffect.CurrentTechnique.Passes)
            //{
            edgeEffect.Parameters["modelTexture"].SetValue(testMap2);
            //testEffect.Parameters["modelTexture"].SetValue(testMap2);
            //edgeEffect.Parameters["modelTexture"].SetValue(depthAndColorMap2);

            //edgeEffect.Parameters["depthTexture"].SetValue(depthAndColorMap2);
            testEffect.Parameters["depthTexture"].SetValue(depthAndColorMap2);
            //edgeEffect.Parameters["depthTexture"].SetValue(testMap2);

            edgeEffect.Parameters["imageWidth"].SetValue((float)testMap2.Width);
            testEffect.Parameters["imageWidth"].SetValue((float)testMap2.Width);

            edgeEffect.Parameters["imageHeight"].SetValue((float)testMap2.Height);
            testEffect.Parameters["imageHeight"].SetValue((float)testMap2.Height);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, 800, 600, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            edgeEffect.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);
            testEffect.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);


            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Red, 1.0f, 0);

            edgeMap = testMap2;
            //edgeMap = depthAndColorMap2;
            edgeMap2 = testMap2;

            /*  
            using (SpriteBatch sprite = new SpriteBatch(GraphicsDevice))
            {
                sprite.Begin(0, null, null, null, null, testEffect);
                //sprite.Draw(depthAndColorMap2, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 0.25f, SpriteEffects.None, 0);
                sprite.Draw(testMap2, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.175f, SpriteEffects.None, 0);
                //sprite.Draw(edgeMap, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.175f, SpriteEffects.None, 0);
                sprite.End();

                
                sprite.Begin(0, null, null, null, null, edgeEffect);
                //sprite.Draw(depthAndColorMap2, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 0.25f, SpriteEffects.None, 0);
                //sprite.Draw(testMap2, new Vector2(400, 0), null, Color.White, 0, Vector2.Zero, 0.175f, SpriteEffects.None, 0);
                sprite.Draw(edgeMap2, new Vector2(400, 0), null, Color.White, 0, Vector2.Zero, 0.175f, SpriteEffects.None, 0);
                sprite.End();
                
            }
            */

            
            GraphicsDevice.SetRenderTarget(edgeMapRenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);
            using (SpriteBatch sprite = new SpriteBatch(GraphicsDevice))
            {
                sprite.Begin(0, null, null, null, null, testEffect);
                //sprite.Draw(depthAndColorMap2, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 0.175f, SpriteEffects.None, 0);
                //sprite.Draw(testMap2, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.175f, SpriteEffects.None, 0);
                sprite.Draw(testMap2, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.175f, SpriteEffects.None, 0);
                sprite.End();
            }

            edgeMap = (Texture2D)edgeMapRenderTarget;

            GraphicsDevice.SetRenderTarget(edgeMap2RenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);
            using (SpriteBatch sprite = new SpriteBatch(GraphicsDevice))
            {
                sprite.Begin(0, null, null, null, null, edgeEffect);
                //sprite.Draw(depthAndColorMap2, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 0.175f, SpriteEffects.None, 0);
                //sprite.Draw(testMap2, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.175f, SpriteEffects.None, 0);
                sprite.Draw(testMap2, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.175f, SpriteEffects.None, 0);
                sprite.End();
            }

            edgeMap2 = (Texture2D)edgeMap2RenderTarget;

        }


        private void DrawComposite()
        {
            compositeEffect.CurrentTechnique = compositeEffect.Techniques["ComposeEdgeMaps"];

            compositeEffect.Parameters["normalEdgeTexture"].SetValue(edgeMap2);
            compositeEffect.Parameters["depthEdgeTexture"].SetValue(edgeMap);

            compositeEffect.Parameters["imageWidth"].SetValue((float)edgeMap2.Width);
            compositeEffect.Parameters["imageHeight"].SetValue((float)edgeMap2.Height);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, 800, 600, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            compositeEffect.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);

            compositeEffect.CurrentTechnique.Passes[0].Apply();

            //GraphicsDevice.Clear(Color.CornflowerBlue);

            //GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.SetRenderTarget(compositeRenderTarget);
            //GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            //compositeEdgeMap = edgeMap2;
            //edgeMap2 = testMap2;

            using (SpriteBatch sprite = new SpriteBatch(GraphicsDevice))
            {
                sprite.Begin(0, null, null, null, null, compositeEffect);
                //sprite.Draw(depthAndColorMap2, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 0.25f, SpriteEffects.None, 0);
                //sprite.Draw(testMap2, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.175f, SpriteEffects.None, 0);
                //sprite.Draw(compositeEdgeMap, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
                sprite.Draw(edgeMap2, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
                sprite.End();
            }
        }

        private void DrawBG()
        {
            compositeEffect.CurrentTechnique = compositeEffect.Techniques["DrawBG"];
            

            compositeEffect.Parameters["normalEdgeTexture"].SetValue(bgTexture);
            compositeEffect.Parameters["depthEdgeTexture"].SetValue(compositeEdgeMap);

            compositeEffect.Parameters["imageWidth"].SetValue((float)bgTexture.Width);
            compositeEffect.Parameters["imageHeight"].SetValue((float)bgTexture.Height);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, 800, 600, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            compositeEffect.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);

            compositeEffect.CurrentTechnique.Passes[0].Apply();

            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

            layerBlend = compositeEdgeMap;

            using (SpriteBatch sprite = new SpriteBatch(GraphicsDevice))
            {
                sprite.Begin(0, null, null, null, null, compositeEffect);
                //sprite.Draw(layerBlend, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.28f, SpriteEffects.None, 0);
                sprite.Draw(bgTexture, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
                sprite.End();
            }
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            //GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            //RasterizerState originalRasterizerState = GraphicsDevice.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            //GraphicsDevice.DepthStencilState = new DepthStencilState();
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            GraphicsDevice.SetRenderTarget(compositeRenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            // *** Lab 9 : Step2, Set the render target
            GraphicsDevice.SetRenderTarget(depthRenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);
            // *** Lab 9 : Step3, Render a shadow map    
            DrawDepthMap();

            GraphicsDevice.SetRenderTarget(null);

            depthAndColorMap = (Texture2D)depthRenderTarget;
            depthAndColorMap2 = (Texture2D)depthRenderTarget;

            //

            GraphicsDevice.SetRenderTarget(normalRenderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            DrawNormalMap();

            GraphicsDevice.SetRenderTarget(null);

            testMap = (Texture2D)normalRenderTarget;
            testMap2 = (Texture2D)normalRenderTarget;

            DrawEdgeMap();
            GraphicsDevice.SetRenderTarget(null);
            DrawComposite();


            for (int i = 1; i < depthLayerCount; i++)
            {
                //GraphicsDevice.SetRenderTarget(null);
                // *** Lab 9 : Step5: Clear the render target
                //depth2 = new RenderTarget2D(GraphicsDevice, 2048, 2048, false, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);
                if (i % 2 == 1)
                {
                    GraphicsDevice.SetRenderTarget(depth2);
                } 
                else
                {
                    GraphicsDevice.SetRenderTarget(depthRenderTarget);
                }

                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);
                // *** Lab 9 : Step6, Draw a scene
                DrawDepthLayer();

                GraphicsDevice.SetRenderTarget(null);

                //GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
                //depthAndColorMap = (Texture2D)renderTarget;
                // *** Lab 9 : Step7, clear the shadow map


                if (i % 2 == 1)
                {
                    GraphicsDevice.SetRenderTarget(normal2);
                }
                else
                {
                    GraphicsDevice.SetRenderTarget(normalRenderTarget);
                }

                GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

                DrawNormalLayer();

                GraphicsDevice.SetRenderTarget(null);

                if (i % 2 == 1)
                {
                    depthAndColorMap2 = (Texture2D)depth2;
                    testMap2 = (Texture2D)normal2;
                }
                else
                {
                    depthAndColorMap2 = (Texture2D)depthRenderTarget;
                    testMap2 = (Texture2D)normalRenderTarget;
                }

                DrawEdgeMap();

                GraphicsDevice.SetRenderTarget(null);

                DrawComposite();


                //depthAndColorMap2 = (Texture2D)depth2;
                //testMap2 = (Texture2D)normal2;




                //GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
                //testMap = (Texture2D)renderTarget;

                /*using (SpriteBatch sprite = new SpriteBatch(GraphicsDevice))
                {
                    sprite.Begin();
                    sprite.Draw(depthAndColorMap2, new Vector2(0, 0), null, Color.Black, 0,
                    new Vector2(0, 0), 0.25f, SpriteEffects.None, 0);
                    sprite.End();
                } */
            }


            //DrawEdgeMap();

            /*
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Red, 1.0f, 0);

            using (SpriteBatch sprite = new SpriteBatch(GraphicsDevice))
            {
                sprite.Begin();
                //sprite.Draw(depthAndColorMap2, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 0.25f, SpriteEffects.None, 1);
                //sprite.Draw(testMap2, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.175f, SpriteEffects.None, 0);
                sprite.Draw(edgeMap, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
                sprite.End();


                sprite.Begin();
                //sprite.Draw(depthAndColorMap2, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 0.25f, SpriteEffects.None, 1);
                //sprite.Draw(testMap2, new Vector2(400, 0), null, Color.White, 0, Vector2.Zero, 0.175f, SpriteEffects.None, 0);
                sprite.Draw(edgeMap2, new Vector2(500, 0), null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
                sprite.End();

            }

            */


            //DrawComposite();

            
            /*
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

            using (SpriteBatch sprite = new SpriteBatch(GraphicsDevice))
            {
                sprite.Begin();
                //sprite.Draw(depthAndColorMap2, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 0.25f, SpriteEffects.None, 1);
                sprite.Draw(testMap2, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 0.25f, SpriteEffects.None, 1);
                sprite.End();
            }
            */
            

            compositeEdgeMap = (Texture2D)compositeRenderTarget;
            DrawBG();

            /*
            GraphicsDevice.SetRenderTarget(layerBlendTarget);

            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            using (SpriteBatch sprite = new SpriteBatch(GraphicsDevice))
            {
                sprite.Begin();
                //sprite.Draw(depthAndColorMap2, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 0.25f, SpriteEffects.None, 1);
                sprite.Draw(compositeEdgeMap, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0);
                sprite.End();
            }

            layerBlend = (Texture2D)layerBlendTarget;
            


            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

            using (SpriteBatch sprite = new SpriteBatch(GraphicsDevice))
            {
                sprite.Begin();
                //sprite.Draw(depthAndColorMap2, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 0.25f, SpriteEffects.None, 1);
                sprite.Draw(compositeEdgeMap, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0);
                sprite.End();
            }
            */


            // *** Lab 9 : Step7, clear the shadow map
            depthAndColorMap = null;
            testMap = null;
            depthAndColorMap2 = null;
            testMap2 = null;
            edgeMap = null;
            edgeMap2 = null;
            compositeEdgeMap = null;
            layerBlend = null;

            //GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);



            _spriteBatch.Begin();

            _spriteBatch.DrawString(font, ("Depth Layer Count: " + depthLayerCount).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY * 12, Color.Black);
            //_spriteBatch.DrawString(font, ("SSAO Radius: " + SSAORad).ToString(), Vector2.UnitX * 4 +
            //    Vector2.UnitY + new Vector2(0, 3) * 12, Color.Black);
            //_spriteBatch.DrawString(font, ("Camera distance: " + distance).ToString(), Vector2.UnitX * 4 +
            //    Vector2.UnitY + new Vector2(0, 5) * 12, Color.Black);

            //_spriteBatch.DrawString(font, ("Change Offset: O/LShift+R").ToString(), Vector2.UnitX * 470 +
            //        Vector2.UnitY * 12, Color.Black);
            //_spriteBatch.DrawString(font, ("Change SSAO Radius: R/LShift+R").ToString(), Vector2.UnitX * 470 +
            //    Vector2.UnitY + new Vector2(0, 3) * 12, Color.Black);

            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}