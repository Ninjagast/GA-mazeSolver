using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Doolhof.classes
{
    class Mouse
    {
        public Texture2D    texture;
        public Texture2D    alphaMouse;
        public Vector2      position;

        public string       name         = "duco";
        public float        fitness      = 0f;

        public Brain        brain;

        public Boolean      dead         = false;
        public Boolean      reachedGoal  = false;
        public Boolean      isBest       = false;
        public Random       random;

        public Mouse(GraphicsDeviceManager graphics, Texture2D mouse, Texture2D inputAlphaMouse, Random inputrandom)
        {
            random      = inputrandom;
            position    = new Vector2(graphics.PreferredBackBufferWidth * 0.3f, graphics.PreferredBackBufferHeight / 2);
            brain       = new Brain(400, random, position);
            texture     = mouse;
            alphaMouse  = inputAlphaMouse;
        }
        
        public void show(SpriteBatch spriteBatch)
        {
            if(isBest)
            {
                spriteBatch.Draw(
                    alphaMouse,
                    position,
                    null,
                    Microsoft.Xna.Framework.Color.White,
                    0f,
                    new Vector2(texture.Width / 2, texture.Height / 2),
                    Vector2.One,
                    SpriteEffects.None,
                    0f
                );
            }
            else
            {
                spriteBatch.Draw(
                    texture,
                    position,
                    null,
                    Microsoft.Xna.Framework.Color.White,
                    0f,
                    new Vector2(texture.Width / 2, texture.Height / 2),
                    Vector2.One,
                    SpriteEffects.None,
                    0f
                );
            }
        }

        public void move()
        {
            if(brain.directions.Length > brain.step)
            {
                position += brain.directions[brain.step];
                brain.step++;
            }
            else
            {
                dead = true;
            }
        }
        public void update(GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice, Rectangle goalHitBox, Rectangle wallHitBox, Rectangle wallHitBox2)
        {
            if(!dead && !reachedGoal)
            {
                move();
                Microsoft.Xna.Framework.Rectangle mouseHitBox = new Microsoft.Xna.Framework.Rectangle
                (
                    (int)(position.X - texture.Width / 2),
                    (int)(position.Y - texture.Height / 2), texture.Width, texture.Height
                );

                //makes a border around the screen so It can't move past it without dying
                if (position.X > graphics.PreferredBackBufferWidth - texture.Width / 2 || position.Y > graphics.PreferredBackBufferHeight - texture.Height / 2 || position.X < (0 + (texture.Width / 2)) || position.Y < (0 + (texture.Height / 2)))
                {
                    dead = true;
                }
                else if (goalHitBox.Intersects(mouseHitBox)) //reached goal
                {
                    reachedGoal = true;
                }
                else if (wallHitBox.Intersects(mouseHitBox) || wallHitBox2.Intersects(mouseHitBox)) // hit obstacle
                {
                    dead = true;
                }
            }
        }
        public void calculateFitness(Vector2 goal, Vector2 mouse)
        {
            
            if (reachedGoal)
            {
                fitness = ((1.0f / ((float)brain.step * 4)));
            }
            else
            {
                float distance = Vector2.Distance(mouse, goal); // get distance from goal
                fitness = 0.5f / ((distance * distance) + (brain.step * 4));
            }
        }
        public Mouse gimmeBaby(GraphicsDeviceManager graphics, Texture2D mouseT, Texture2D alphaMouseT)
        {
            Mouse mouse = new Mouse(graphics, mouseT, alphaMouseT, random);
            mouse.brain = brain.clone();
            return mouse;
        }
    }
}
