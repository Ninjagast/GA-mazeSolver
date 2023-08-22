using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Doolhof.classes;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.CodeDom;
using System.Runtime.InteropServices;

namespace MazeSolverAI
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager   graphics;
        SpriteBatch             spriteBatch;
        Population              mice;
        Random                  random;
        SpriteFont              font;

        //Positions of text
        Vector2                 genNumberPosition   = new Vector2(10, 75);
        Vector2                 stepNumberPosition  = new Vector2(10, 50);
        Vector2                 wonPosition         = new Vector2(10, 25);

        //Mouse textures
        Texture2D               mouseTexture;
        Texture2D               alphaMouseTexture;

        //The goal (aka exit of the maze)
        Rectangle               goalHitbox;
        Vector2                 goalPosition;
        Texture2D               goalTexture;

        //The walls
        Rectangle               wallHitBox;
        Vector2                 wallPosition;
        Texture2D               wallTexture;
        Rectangle               wallHitBox2;
        Vector2                 wallPosition2;
        Texture2D               wallTexture2;

        //The tutor help box
        Vector2                 tutorBoxPosition;
        Texture2D               tutorBoxTexture;

        //Customization vars
        double                  updateHrtz          = 60; //60 updates per second
        int                     step                = 0;
        int                     numberOfMice        = 4000;

        Vector2                 pausedPosition;
        bool                    paused              = true;

        public Game1()
        {
            //We set a new graphicsDeviceManager
            graphics = new GraphicsDeviceManager(this);

            //Set the content directory
            Content.RootDirectory = "Content";

            //We set a custom hertz for the update function
            this.IsFixedTimeStep = true;

            //we set it at 60 updates per second as standard
            this.TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / updateHrtz);

            //We make the mouse vivible on screen
            this.IsMouseVisible = true;
        }

        //This function gets called after this class has been set
        protected override void Initialize()
        {
            //We set all textures
            mouseTexture            = Content.Load<Texture2D>("mouse");
            alphaMouseTexture       = Content.Load<Texture2D>("aMouse");
            goalTexture             = Content.Load<Texture2D>("goal");
            wallTexture             = Content.Load<Texture2D>("wall");
            wallTexture2            = Content.Load<Texture2D>("wallH");
            tutorBoxTexture         = Content.Load<Texture2D>("tutorBox");

            //We set the position of the wall and goal
            goalPosition            = new Vector2(GraphicsDevice.Viewport.Width  * 0.9f,                                     GraphicsDevice.Viewport.Height * 0.2f);
            wallPosition            = new Vector2(GraphicsDevice.Viewport.Width  * 0.7f,                                     GraphicsDevice.Viewport.Height * 0f);
            wallPosition2           = new Vector2(GraphicsDevice.Viewport.Width  * 0.7f,                                     GraphicsDevice.Viewport.Height * 0.5f);
            tutorBoxPosition        = new Vector2((GraphicsDevice.Viewport.Width * 0.5f) - (tutorBoxTexture.Width * 0.5f),   (GraphicsDevice.Viewport.Height * 0.5f) - (tutorBoxTexture.Height * 0.5f));
            pausedPosition          = new Vector2((GraphicsDevice.Viewport.Width * 0.38f),                                   (GraphicsDevice.Viewport.Height * 0.2f));

            //We set a new random
            random                  = new Random();

            //We create hitboxes for the walls and goal
            wallHitBox  = new Rectangle
            (
                (int)(wallPosition.X)   ,   //x position of the rectangle
                (int)(wallPosition.Y)   ,   //Y position of the rectangle
                wallTexture.Width       ,   //Width of the rectangle
                wallTexture.Height          //Height of the rectangle
            );

            wallHitBox2 = new Rectangle
            (
                (int)(wallPosition2.X),      
                (int)(wallPosition2.Y),      
                wallTexture2.Width,          
                wallTexture2.Height          
            );

            goalHitbox  = new Rectangle
            (
                (int)(goalPosition.X)   ,
                (int)(goalPosition.Y)   ,
                goalTexture.Width       ,
                goalTexture.Height
            );


            //We create a new population based on the graphics and random we set
            mice = new Population(numberOfMice, graphics, mouseTexture, alphaMouseTexture, random);

            base.Initialize();
        }

        //After initializing we load the content
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //We load our standard font
            font        = Content.Load<SpriteFont>("gen");
        }

        protected override void UnloadContent()
        {

        }

        //This function is called x amount of times per second to update the game
        protected override void Update(GameTime gameTime)
        {
            //we check if the player has pressed the exit button
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

            if(Keyboard.GetState().IsKeyDown(Keys.NumPad1))
            {
                updateHrtz = 30;
                this.TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / updateHrtz);

            }
            if(Keyboard.GetState().IsKeyDown(Keys.NumPad2))
            {
                updateHrtz = 60;
                this.TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / updateHrtz);

            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad3))
            {
                updateHrtz = 120;
                this.TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / updateHrtz);
            }            

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                paused = true;
            }

            if(!paused)
            {
                // We create a new generation and mutate it when all current mice are dead or reached the goal
                if (mice.allMiceDead())
                {

                    step = 0;
                    //We calculate the fitness of the last generation
                    mice.calculateFitness(goalPosition);
                    //We select parents and make babies we also copy the best mouse into the new generation so the new generation will never be worse than the last
                    mice.naturalSelection(graphics, mouseTexture, alphaMouseTexture, random);
                    //This is where we force feed toxic substances to the babies so they'll mutate and be different than their parents
                    mice.mutateBabies();
                }
            }
            else
            {
                if(Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    paused = false;
                }
            }

            base.Update(gameTime);
        }

        //This function gets called after each update function and is mainly used to draw in the game window
        protected override void Draw(GameTime gameTime)
        {
            //gives the background of the screen a color
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Begins a sprite batch (which is used to draw sprites on the screen)
            spriteBatch.Begin();

            if (!paused)
            {
                //We draw the goal and walls
                spriteBatch.Draw(goalTexture,  goalPosition,  Color.White);
                spriteBatch.Draw(wallTexture,  wallPosition,  Color.White);
                spriteBatch.Draw(wallTexture2, wallPosition2, Color.White);

                //We update the mice (we let them take a step and show it on the screen)
                mice.update(graphics, GraphicsDevice, goalHitbox, wallHitBox, wallHitBox2);
                mice.show(spriteBatch);

                //We draw the generation number, whether a mouse has won yet and step number
                spriteBatch.DrawString(font, "Generation number = " + mice.gen, genNumberPosition, Color.Black);
                spriteBatch.DrawString(font, "step number = " + step, stepNumberPosition, Color.Black);
                if (!mice.won)
                {
                    spriteBatch.DrawString(font, "No mouse has won yet", wonPosition, Color.Black);
                }
                else
                {
                    spriteBatch.DrawString(font, "sweet sweet victory", wonPosition, Color.Black);
                }

                //Another step bites the dust
                step++;
            }
            else
            {
                spriteBatch.DrawString(font, "The game is currently paused",                                    pausedPosition, Color.Black);
                spriteBatch.Draw(tutorBoxTexture, tutorBoxPosition, Color.White);
                spriteBatch.DrawString(font, "Unpause the game with the Enter Button",                          tutorBoxPosition + new Vector2(8,4), Color.White);
                spriteBatch.DrawString(font, "Pause the game with the space Button",                            tutorBoxPosition + new Vector2(8,24), Color.White);
                spriteBatch.DrawString(font, "Speed and slow down the game with the numbpad 1~3",               tutorBoxPosition + new Vector2(8,44), Color.White);
                spriteBatch.DrawString(font, "Please read the README for a full description of this project",   tutorBoxPosition + new Vector2(8,64), Color.White);
            }

            //We close this batch since everything is nice and cooked and ready for sale
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
