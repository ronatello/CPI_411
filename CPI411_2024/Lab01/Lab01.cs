﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab01
{
    public class Lab01 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Lab 01
        Effect effect;
        VertexPositionColor[] vertices =
        {
            new VertexPositionColor(new Vector3(0, 1, 0), Color.White),
            new VertexPositionColor(new Vector3(1, 0, 0), Color.Blue),
            new VertexPositionColor(new Vector3(-1, 0, 0), Color.Red)
        };
        /*VertexPositionTexture[] vertices =
        {
            new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(0.5f, 0)),
            new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(1, 1)),
            new VertexPositionTexture(new Vector3(-1, 0, 0), new Vector2(0, 1))
        };*/
        //

        public Lab01()
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

            // TODO: use this.Content to load your game content here
            effect = Content.Load<Effect>("SimpleTexture");
            //effect.Parameters["MyTexture"].SetValue(Content.Load<Texture2D>("logo_mg"));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            //GraphicsDevice.BlendState = BlendState.AlphaBlend;

            // TODO: Add your drawing code here
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3);
            }

            base.Draw(gameTime);
        }
    }
}