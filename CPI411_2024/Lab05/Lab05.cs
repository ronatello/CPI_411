using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI411.SimpleEngine;

namespace Lab05
{
    public class Lab05 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Lab02
        float angleXZ;
        float angleYZ;
        float distance = 13;
        Vector3 cameraPos = new Vector3(0, 0, 2);
        Matrix world, view, projection;

        // Lab03
        MouseState previousMouseState;

        // Lab05
        Skybox skybox;
        string[] skyboxTextures; //names of images

        public Lab05()
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

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.Opaque;

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            skybox.Draw(view, projection, cameraPos);

            base.Draw(gameTime);
        }
    }
}