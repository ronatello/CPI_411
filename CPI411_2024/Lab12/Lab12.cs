using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab12
{
    public class Lab12 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

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
        Model model;

        // Lab 12
        RenderTarget2D renderTarget;
        Texture2D randomNormalMap, depthAndNormalMap;
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

        public Lab12()
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

            font = Content.Load<SpriteFont>("Font");
            model = Content.Load<Model>("objects");
            randomNormalMap = Content.Load<Texture2D>("noise");

            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(GraphicsDevice, 2048, 2048, false, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);
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

            base.Update(gameTime);
        }

        private void DrawDepthAndNormalMap()
        {
            effect = Content.Load<Effect>("DepthAndNormalMap");
            effect.CurrentTechnique = effect.Techniques[0];
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

        private void DrawSSAO()
        {
            effect = Content.Load<Effect>("SSAO");
            effect.CurrentTechnique = effect.Techniques[0];
            effect.CurrentTechnique.Passes[0].Apply();
            effect.Parameters["RandomNormalTexture"].SetValue(randomNormalMap);
            effect.Parameters["DepthAndNormalTexture"].SetValue(depthAndNormalMap);
            effect.Parameters["offset"].SetValue(offset);
            effect.Parameters["rad"].SetValue(SSAORad);
            GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>
            (PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            // *** Lab 9 : Step2, Set the render target
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            // *** Lab 9 : Step3, Render a shadow map    
            DrawDepthAndNormalMap();
            // *** Lab 9 : Step4: Clear the render target
            GraphicsDevice.SetRenderTarget(null);
            depthAndNormalMap = (Texture2D)renderTarget;
            // *** Lab 9 : Step5: Clear the render target
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
            // *** Lab 9 : Step6, Draw a scene
            DrawSSAO();

            //GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
            /*
            using (SpriteBatch sprite = new SpriteBatch(GraphicsDevice))
            {
                sprite.Begin();
                sprite.Draw(depthAndNormalMap, new Vector2(0, 0), null, Color.White, 0,
                new Vector2(0, 0), 0.25f, SpriteEffects.None, 1);
                sprite.End();
            }
            */

            // *** Lab 9 : Step7, clear the shadow map
            depthAndNormalMap = null;

            _spriteBatch.Begin();

            _spriteBatch.DrawString(font, ("Offset: " + offset).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY * 12, Color.Black);
            _spriteBatch.DrawString(font, ("SSAO Radius: " + SSAORad).ToString(), Vector2.UnitX * 4 +
                Vector2.UnitY + new Vector2(0, 3) * 12, Color.Black);
            _spriteBatch.DrawString(font, ("Camera distance: " + distance).ToString(), Vector2.UnitX * 4 +
                Vector2.UnitY + new Vector2(0, 5) * 12, Color.Black);

            _spriteBatch.DrawString(font, ("Change Offset: O/LShift+R").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY * 12, Color.Black);
            _spriteBatch.DrawString(font, ("Change SSAO Radius: R/LShift+R").ToString(), Vector2.UnitX * 470 +
                Vector2.UnitY + new Vector2(0, 3) * 12, Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}