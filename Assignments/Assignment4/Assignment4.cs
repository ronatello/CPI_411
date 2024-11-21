using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI411.SimpleEngine;
using System;

namespace Assignment4
{
    public class Assignment4 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Assignment 4
        string currentEmitter = "square";
        string currentFountain = "basic";

        int curveCount = 0;
        Vector3 fountainVelocity = new Vector3(0.0f, 0.0f, 0.0f);
        float gravity = 0.0f;
        bool bounceFlag = false;
        bool basicFlag = true;
        Vector3 windForce = new Vector3(0.0f, 0.0f, 0.0f);
        float windStrength = 0.0f;
        float userXVel = 0.0f;
        float userYVel = 0.0f;
        float userZVel = 0.0f;
        bool userVelocityToggle = false;
        float resilience = 0.8f;
        float friction = 0.5f;
        float lifespan = 5.0f;
        float userGravity = -9.8f;
        string directionToggle = "Up";
        float randomScale = 3.0f;

        string particleType = "fire";

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
        KeyboardState previousKeyboardState;
        Model model;
        bool info = true;
        bool help = true;

        Texture2D texture;
        Texture2D texture_fire;
        Texture2D texture_water;
        Texture2D texture_smoke;

        /* Color Data Set*/
        Vector4 ambient = new Vector4(0, 0, 0, 0);
        float ambientIntensity = 0.1f;
        Vector4 diffuseColor = new Vector4(1, 1, 1, 1);
        float diffuseIntensity = 0.6f;
        Vector3 lightPos = new Vector3(0, 0, 10);

        float shininess = 25.0f;
        Vector4 specularColor = new Vector4(1, 1, 1, 1);
        float specularIntensity = 1.0f;

        public Assignment4()
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
            font = Content.Load<SpriteFont>("Font");
            texture_fire = Content.Load<Texture2D>("fire");
            texture_smoke = Content.Load<Texture2D>("smoke");
            texture_water = Content.Load<Texture2D>("water");
            model = Content.Load<Model>("Plane");
            random = new System.Random();
            particleManager = new ParticleManager(GraphicsDevice, 10000);
            particlePosition = new Vector3(0, 0, 0);

            texture = texture_fire;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.H) && !previousKeyboardState.IsKeyDown(Keys.H)) info = !info;
            if (Keyboard.GetState().IsKeyDown(Keys.OemQuestion) && !previousKeyboardState.IsKeyDown(Keys.OemQuestion)) help = !help;

            if (Keyboard.GetState().IsKeyDown(Keys.D1))
            {
                effect.CurrentTechnique = effect.Techniques["Phong"];
                particleType = "Phong";
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D2))
            {
                effect.CurrentTechnique = effect.Techniques["Particle"];
                texture = texture_smoke;
                particleType = "Smoke";
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D3))
            {
                effect.CurrentTechnique = effect.Techniques["Particle"];
                texture = texture_water;
                particleType = "Water";
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D4))
            {
                effect.CurrentTechnique = effect.Techniques["Particle"];
                texture = texture_fire;
                particleType = "Fire";
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                currentFountain = "basic";
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F2))
            {
                currentFountain = "medium";
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F3))
            {
                currentFountain = "advanced";
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F4) && !previousKeyboardState.IsKeyDown(Keys.F4))
            {
                if (currentEmitter == "square")
                {
                    currentEmitter = "circle";
                }
                else if (currentEmitter == "circle")
                {
                    currentEmitter = "sineCurve";
                } else
                {
                    currentEmitter = "square";
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                if (currentFountain == "basic")
                {
                    bounceFlag = false;
                    basicFlag = true;

                    if (directionToggle == "Up")
                    {
                        fountainVelocity = new Vector3(0.0f, 0.0f, 10.0f);
                    }
                    else
                    {
                        fountainVelocity = new Vector3(0.0f, 0.0f, -10.0f);
                    }

                    gravity = 0.0f;
                    windForce = Vector3.Zero;
                }

                else if (currentFountain == "medium")
                {
                    bounceFlag = false;
                    basicFlag = false;

                    float constVelocity = 10.0f;
                    float squareVelocity = constVelocity * constVelocity;

                    float sqzVel = 25.0f + (float)random.NextDouble() * 25.0f;
                    squareVelocity -= sqzVel;

                    float sqxVel = ((float)random.NextDouble() * squareVelocity * 2) - squareVelocity;
                    squareVelocity -= Math.Abs(sqxVel);

                    float sqyVel;

                    if ((float)random.NextDouble() - 0.5f < 0.0f)
                    {
                        sqyVel = -squareVelocity;
                    }
                    else
                    {
                        sqyVel = squareVelocity;
                    }

                    float zVel = (float)Math.Sqrt(sqzVel);
                    float xVel;
                    float yVel;

                    if (sqxVel < 0.0f)
                    {
                        xVel = -(float)Math.Sqrt(-sqxVel);
                    } else
                    {
                        xVel = (float)Math.Sqrt(sqxVel);
                    }

                    if (sqyVel < 0.0f)
                    {
                        yVel = -(float)Math.Sqrt(-sqyVel);
                    }
                    else
                    {
                        yVel = (float)Math.Sqrt(sqyVel);
                    }

                    fountainVelocity = new Vector3(xVel, yVel, zVel);

                    //fountainVelocity = new Vector3(random.Next(-5, 5), random.Next(-5, 5), random.Next(5, 10));
                    gravity = userGravity;
                    windForce = Vector3.Zero;
                }

                else if (currentFountain == "advanced")
                {
                    bounceFlag = true;
                    basicFlag = false;

                    if (userVelocityToggle)
                    {
                        fountainVelocity = new Vector3(userXVel, userYVel, userZVel);
                    }
                    else
                    {
                        float constVelocity = 2.0f;
                        float squareVelocity = constVelocity * constVelocity;

                        float sqzVel = 25.0f + (float)random.NextDouble() * 25.0f;
                        squareVelocity -= sqzVel;

                        float sqxVel = ((float)random.NextDouble() * squareVelocity * 2) - squareVelocity;
                        squareVelocity -= Math.Abs(sqxVel);

                        float sqyVel;

                        if ((float)random.NextDouble() - 0.5f < 0.0f)
                        {
                            sqyVel = -squareVelocity;
                        }
                        else
                        {
                            sqyVel = squareVelocity;
                        }

                        float zVel = (float)Math.Sqrt(sqzVel);
                        float xVel;
                        float yVel;

                        if (sqxVel < 0.0f)
                        {
                            xVel = -(float)Math.Sqrt(-sqxVel);
                        }
                        else
                        {
                            xVel = (float)Math.Sqrt(sqxVel);
                        }

                        if (sqyVel < 0.0f)
                        {
                            yVel = -(float)Math.Sqrt(-sqyVel);
                        }
                        else
                        {
                            yVel = (float)Math.Sqrt(sqyVel);
                        }

                        fountainVelocity = new Vector3(xVel, yVel, zVel);

                        //fountainVelocity = new Vector3(random.Next(-5, 5), random.Next(-5, 5), random.Next(5, 10));
                    }

                    gravity = userGravity;
                    windForce += new Vector3(((float)random.NextDouble() - 0.5f) * 2.0f * windStrength * 0.05f, ((float)random.NextDouble() - 0.5f) * 2.0f * windStrength * 0.05f, ((float)random.NextDouble() - 0.5f) * 2.0f * windStrength * 0.05f);
                }

                if (currentEmitter == "square")
                {
                    for (int i = 0; i < 26; i++)
                    {
                        Particle particle = particleManager.getNext();
                        particle.Position = particlePosition;
                        //particle.Velocity = Vector3.Transform(new Vector3(random.Next(-2, 2), random.Next(-2, 2), random.Next(-2, 2)), Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                        if (currentFountain != "basic")
                        {
                            particle.Velocity = new Vector3(-5.0f + (i * 2 * 5 / 25) + (((float)random.NextDouble() - 0.5f) * randomScale), 5.0f + (((float)random.NextDouble() - 0.5f) * randomScale), 5.0f + (((float)random.NextDouble() - 0.5f) * randomScale)) + windForce;
                        } 
                        else
                        {
                            particle.Velocity = new Vector3(-5.0f + (i * 2 * 5 / 25), 5.0f, 0.0f) + fountainVelocity + windForce;
                        }
                        particle.Acceleration = new Vector3(0, 0, gravity);
                        //particle.Acceleration = Vector3.Transform(new Vector3(1, 2, 0), Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                        particle.MaxAge = lifespan;
                        particle.bounceFlag = bounceFlag;
                        particle.basicFlag = basicFlag;
                        particle.gravity = gravity;
                        particle.resilience = resilience;
                        particle.friction = friction;
                        particle.Init();
                    }

                    for (int i = 0; i < 26; i++)
                    {
                        Particle particle = particleManager.getNext();
                        particle.Position = particlePosition;
                        
                        if (currentFountain != "basic")
                        {
                            particle.Velocity = new Vector3(-5.0f + (i * 2 * 5 / 25) + (((float)random.NextDouble() - 0.5f) * randomScale), -5.0f + (((float)random.NextDouble() - 0.5f) * randomScale), 5.0f + (((float)random.NextDouble() - 0.5f) * randomScale)) + windForce;
                        } 
                        else
                        {
                            particle.Velocity = new Vector3(-5.0f + (i * 2 * 5 / 25), -5.0f, 0.0f) + fountainVelocity + windForce;
                        }
                        particle.Acceleration = new Vector3(0, 0, gravity);
                        particle.MaxAge = lifespan;
                        particle.bounceFlag = bounceFlag;
                        particle.basicFlag = basicFlag;
                        particle.gravity = gravity;
                        particle.resilience = resilience;
                        particle.friction = friction;
                        particle.Init();
                    }

                    for (int i = 0; i < 24; i++)
                    {
                        Particle particle = particleManager.getNext();
                        particle.Position = particlePosition;
                        if (currentFountain != "basic")
                        {
                            particle.Velocity = new Vector3(-5.0f + (((float)random.NextDouble() - 0.5f) * randomScale), -5.0f + (2 * 5 / 25) + (i * 2 * 5 / 25) + (((float)random.NextDouble() - 0.5f) * randomScale), 5.0f + (((float)random.NextDouble() - 0.5f) * randomScale)) + windForce;
                        }
                        else
                        {
                            particle.Velocity = new Vector3(-5.0f, -5.0f + (2 * 5 / 25) + (i * 2 * 5 / 25), 0.0f) + fountainVelocity + windForce;
                        }
                        particle.Acceleration = new Vector3(0, 0, gravity);
                        particle.MaxAge = lifespan;
                        particle.bounceFlag = bounceFlag;
                        particle.basicFlag = basicFlag;
                        particle.gravity = gravity;
                        particle.resilience = resilience;
                        particle.friction = friction;
                        particle.Init();
                    }

                    for (int i = 0; i < 24; i++)
                    {
                        Particle particle = particleManager.getNext();
                        particle.Position = particlePosition;
                        if (currentFountain != "basic")
                        {
                            particle.Velocity = new Vector3(5.0f + (((float)random.NextDouble() - 0.5f) * randomScale), -5.0f + (2 * 5 / 25) + (i * 2 * 5 / 25) + (((float)random.NextDouble() - 0.5f) * randomScale), 5.0f + (((float)random.NextDouble() - 0.5f) * randomScale)) + windForce;
                        }
                        else
                        {
                            particle.Velocity = new Vector3(5.0f, -5.0f + (2 * 5 / 25) + (i * 2 * 5 / 25), 0.0f) + fountainVelocity + windForce;
                        }
                        particle.Acceleration = new Vector3(0, 0, gravity);
                        particle.MaxAge = lifespan;
                        particle.bounceFlag = bounceFlag;
                        particle.basicFlag = basicFlag;
                        particle.gravity = gravity;
                        particle.resilience = resilience;
                        particle.friction = friction;
                        particle.Init();
                    }
                }

                else if (currentEmitter == "circle")
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Particle particle = particleManager.getNext();
                        particle.Position = particlePosition;
                        if (currentFountain != "basic")
                        {
                            particle.Velocity = new Vector3((float)Math.Sin(i) * 5.0f + (((float)random.NextDouble() - 0.5f) * randomScale), (float)Math.Cos(i) * 5.0f + (((float)random.NextDouble() - 0.5f) * randomScale), 5.0f + (((float)random.NextDouble() - 0.5f) * randomScale)) + windForce;
                        } else
                        {
                            particle.Velocity = new Vector3((float)Math.Sin(i) * 5.0f, (float)Math.Cos(i) * 5.0f, 0.0f) + fountainVelocity + windForce;
                        }
                        particle.Acceleration = new Vector3(0, 0, gravity);
                        particle.MaxAge = lifespan;
                        particle.bounceFlag = bounceFlag;
                        particle.basicFlag = basicFlag;
                        particle.gravity = gravity;
                        particle.resilience = resilience;
                        particle.friction = friction;
                        particle.Init();
                    }
                }

                else if (currentEmitter == "sineCurve")
                {
                    Particle particle = particleManager.getNext();
                    particle.Position = particlePosition + new Vector3((float)Math.Sin(curveCount * 2.0f * Math.PI / 180.0f) * 2.0f, 0.0f, 0.0f);
                    particle.Velocity = new Vector3(0.0f, 0.0f, 0.0f) + fountainVelocity + windForce;
                    particle.Acceleration = new Vector3(0, 0, gravity);
                    particle.MaxAge = lifespan;
                    particle.bounceFlag = bounceFlag;
                    particle.basicFlag = basicFlag;
                    particle.gravity = gravity;
                    particle.resilience = resilience;
                    particle.friction = friction;
                    particle.Init();

                    curveCount = (curveCount + 1) % 180;
                }
            }

            particleManager.Update(gameTime.ElapsedGameTime.Milliseconds * 0.001f);


            if (Keyboard.GetState().IsKeyDown(Keys.R) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                resilience -= 0.05f;
            } 
            else if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                resilience += 0.05f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                friction -= 0.05f;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                friction += 0.05f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.L) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                if (lifespan <= 0.1f)
                {
                    lifespan = 0.1f;
                } else
                {
                    lifespan -= 0.05f;
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.L))
            {
                lifespan += 0.05f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                windStrength -= 0.05f;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                windStrength += 0.05f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.G) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                userGravity -= 0.05f;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.G))
            {
                userGravity += 0.05f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.K) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                randomScale -= 0.05f;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.K))
            {
                randomScale += 0.05f;
            }



            if (Keyboard.GetState().IsKeyDown(Keys.V) && !previousKeyboardState.IsKeyDown(Keys.V))
            {
                userVelocityToggle = !userVelocityToggle;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D) && !previousKeyboardState.IsKeyDown(Keys.D) && directionToggle == "Up")
            {
                directionToggle = "Down";
            } 
            else if (Keyboard.GetState().IsKeyDown(Keys.D) && !previousKeyboardState.IsKeyDown(Keys.D) && directionToggle == "Down")
            {
                directionToggle = "Up";
            }



            if (Keyboard.GetState().IsKeyDown(Keys.X) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                userXVel -= 0.05f;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.X))
            {
                userXVel += 0.05f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Y) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                userYVel -= 0.05f;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Y))
            {
                userYVel += 0.05f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Z) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                userZVel -= 0.05f;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Z))
            {
                userZVel += 0.05f;
            }


            if (Keyboard.GetState().IsKeyDown(Keys.Left)) angleL -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) angleL += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) angleL2 -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) angleL2 += 0.02f;

            if (Keyboard.GetState().IsKeyDown(Keys.S)) 
            { 
                angle = angle2 = angleL = angleL2 = 0;
                distance = 20;
                cameraTarget = Vector3.Zero;

                currentEmitter = "square";
                currentFountain = "basic";

                curveCount = 0;
                fountainVelocity = new Vector3(0.0f, 0.0f, 0.0f);
                gravity = 0.0f;
                bounceFlag = false;
                basicFlag = true;
                windForce = new Vector3(0.0f, 0.0f, 0.0f);
                windStrength = 0.0f;
                userXVel = 0.0f;
                userYVel = 0.0f;
                userZVel = 0.0f;
                userVelocityToggle = false;
                resilience = 0.8f;
                friction = 0.5f;
                lifespan = 5.0f;
                userGravity = -9.8f;
                directionToggle = "Up";
            }

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
            previousKeyboardState = Keyboard.GetState();
            cameraPosition = Vector3.Transform(new Vector3(0, 0, distance),
                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle) * Matrix.CreateTranslation(cameraTarget));
            view = Matrix.CreateLookAt(
                cameraPosition,
                cameraTarget,
                Vector3.Transform(Vector3.UnitY, Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle)));
            lightPosition = Vector3.Transform(
                lightPos,
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
            
            effect.CurrentTechnique.Passes[0].Apply();
            effect.Parameters["World"].SetValue(Matrix.Identity);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
            //effect.Parameters["InverseCamera"].SetValue(Matrix.Identity);
            effect.Parameters["InverseCamera"].SetValue(Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
            effect.Parameters["Texture"].SetValue(texture);

            effect.Parameters["CameraPosition"].SetValue(cameraPosition);
            effect.Parameters["AmbientColor"].SetValue(ambient);
            effect.Parameters["AmbientIntensity"].SetValue(ambientIntensity);
            effect.Parameters["DiffuseLightDirection"].SetValue(lightPosition);
            effect.Parameters["DiffuseColor"].SetValue(diffuseColor);
            effect.Parameters["DiffuseIntensity"].SetValue(diffuseIntensity);
            effect.Parameters["SpecularColor"].SetValue(diffuseColor);
            effect.Parameters["SpecularIntensity"].SetValue(specularIntensity);
            effect.Parameters["Shininess"].SetValue(shininess);

            particleManager.Draw(GraphicsDevice);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            // 2D Drawing
            _spriteBatch.Begin();

            if (info)
            {
                _spriteBatch.DrawString(font, ("angleXZ: " + angle).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY * 12, Color.White);
                _spriteBatch.DrawString(font, ("angleYZ: " + angle).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 3) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Light angleXZ: " + angleL).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 5) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Light angleYZ: " + angleL2).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 7) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Particle: " + particleType), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 9) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Emitter: " + currentEmitter), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 11) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Fountain: " + currentFountain), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 13) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Resilience: " + resilience).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 15) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Friction: " + friction).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 17) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Lifespan: " + lifespan).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 19) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Wind Strength: " + windStrength).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 21) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Gravity: " + userGravity).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 23) * 12, Color.White);

                _spriteBatch.DrawString(font, ("User Velocity Toggle: " + userVelocityToggle.ToString()).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 27) * 12, Color.White);
                _spriteBatch.DrawString(font, ("User Velocity: " + new Vector3(userXVel, userYVel, userZVel).ToString()).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 29) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Basic Fountain Direction Toggle: " + directionToggle), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 31) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Randomness Scale: " + randomScale).ToString(), Vector2.UnitX * 4 +
                    Vector2.UnitY + new Vector2(0, 33) * 12, Color.White);
            }

            if (help)
            {
                _spriteBatch.DrawString(font, ("Rotate Camera: Mouse Left Drag").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Camera Distance: Mouse Right Drag").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 3) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Translate Camera: Mouse Middle Drag").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 5) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Reset Camera and Values: S").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 7) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Particle Texture: Num Keys 1-4").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 9) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Fountain: Function Keys F1-F3").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 11) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Emitter: Function Key F4").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 13) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Change Resilience: R/LShift+R").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 15) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Friction: F/LShift+F").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 17) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Lifespan: L/LShift+L").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 19) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Wind Strength: W/LShift+W").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 21) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Gravity: G/LShift+G").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 23) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Toggle User Velocity Control: V").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 25) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change User X Velocity: X/LShift+X").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 27) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change User Y Velocity: Y/LShift+Y").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 29) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change User Z Velocity: Z/LShift+Z").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 31) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Toggle Basic Fountain Direction: D").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 33) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Change Randomness Scale: K/LShift+K").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 35) * 12, Color.White);

                _spriteBatch.DrawString(font, ("Show/Hide Shader Values: H").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 37) * 12, Color.White);
                _spriteBatch.DrawString(font, ("Show/Hide Help: ?").ToString(), Vector2.UnitX * 470 +
                    Vector2.UnitY + new Vector2(0, 38.5f) * 12, Color.White);

            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}