using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace CPI411.SimpleEngine
{
    public class Particle
    {
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 Acceleration { get; set; }
        public float Age { get; set; }
        public float MaxAge { get; set; }
        public Vector3 Color { get; set; }
        public float Size { get; set; }
        public float SizeVelocity { get; set; }
        public float SizeAcceleration { get; set; }
        public bool bounceFlag { get; set; }
        public bool basicFlag { get; set; }
        public float friction { get; set; }
        public float resilience { get; set; }
        public float gravity { get; set; }
        private int bounceCount;
        public Particle() { Age = -1; }
        public bool Update(float ElapsedGameTime)
        {
            if (Age < 0) return false;
            Velocity += Acceleration * ElapsedGameTime;
            Position += Velocity * ElapsedGameTime;
            SizeVelocity += SizeAcceleration * ElapsedGameTime;
            Size += SizeVelocity * ElapsedGameTime;
            Age += ElapsedGameTime;
            if (Age > MaxAge)
            {
                Age = -1;
                return false;
            }

            if (Position.Z <= 0.0f && Age > 0)
            {
                if (!bounceFlag && !basicFlag) 
                {
                    Velocity = new Vector3(Velocity.X, Velocity.Y, 0.0f);
                    Acceleration = new Vector3(Acceleration.X * friction, Acceleration.Y * friction, 0.0f);
                } else if (bounceFlag && bounceCount < 5)
                {
                    bounceCount++;
                    Velocity = new Vector3(Velocity.X, Velocity.Y, -Velocity.Z * resilience);
                    Acceleration = new Vector3(Acceleration.X * friction, Acceleration.Y * friction, gravity);
                }
                else if (bounceFlag && bounceCount >= 5)
                {
                    Velocity = new Vector3(Velocity.X, Velocity.Y, 0.0f);
                    Acceleration = new Vector3(Acceleration.X * friction, Acceleration.Y * friction, 0.0f);
                }
            }
            return true;
        }
        public bool IsActive() { return Age < 0 ? false : true; }
        public void Activate() { Age = 0; }
        public void Init()
        {
            Age = 0; Size = 1; SizeVelocity = SizeAcceleration = 0;
        }
    }
}