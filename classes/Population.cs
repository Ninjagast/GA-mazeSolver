using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Doolhof.classes
{
    class Population
    {
        //All the mice within the current population
        Mouse[] mice;

        private float   fitnessSum;

        //We give standard values to these vars
        public  int     gen         = 1;
        private int     bestMouse   = 0;
        private int     minStep     = 1000;
        public  bool    won         = false;

        //Constructor of this class
        public Population(int size, GraphicsDeviceManager graphics, Texture2D texture, Texture2D alphaMouse, Random inputRandom)
        {
            //Sets size of population
            mice = new Mouse[size];
            for (int i = 0; i < size; i++)
            {
                //Fills every slot with a new mouse
                mice[i] = new Mouse(graphics, texture, alphaMouse, inputRandom);
            }
        }

        //Show function for updating the screen to show the new mouse positions
        public void show(SpriteBatch spriteBatch)
        {
            //we show every mouse
            for (int i = 1; i < mice.Length; i++)
            {
                mice[i].show(spriteBatch);
            }
            mice[0].show(spriteBatch);
        }

        //the update function updates the mice to take the next step in their directions array
        public void update(GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice, Rectangle goalHitBox, Rectangle wallHitBox, Rectangle wallHitBox2)
        {
            for (int i = 0; i < mice.Length; i++)
            {
                if(mice[i].reachedGoal)
                {
                    //yay someone got the cheese
                    won = true;
                }

                //we kill if a mouse has set more steps then the min
                if (mice[i].brain.step > minStep)
                {
                    mice[i].dead = true;
                }
                else
                {
                    //we update the mouse
                    mice[i].update(graphics, graphicsDevice, goalHitBox, wallHitBox, wallHitBox2);
                }
            }
        }

        //This function calculates the fitness of everymouse
        public void calculateFitness(Vector2 goal)
        {
            for (int i = 0; i < mice.Length; i++)
            {
                mice[i].calculateFitness(goal, mice[i].position);
            }
        }

        //checks whether or not all mice are dead true if dead
        public Boolean allMiceDead()
        {
            for (int i = 0; i < mice.Length; i++)
            {
                if (!mice[i].dead && !mice[i].reachedGoal)
                {
                    return false;
                }
            }
            return true;
        }

        //Does all the naturalSelection and generates the new generation
        public void naturalSelection(GraphicsDeviceManager graphics, Texture2D mouse, Texture2D alphaMouse, Random random)
        {
            //We generate the array for the new mice
            Mouse[] newMice = new Mouse[mice.Length];

            //We get the best mouse and calculate combine all fitness scores
            setBestMouse();
            calculateFitnessSum();

            //We set the best mouse
            newMice[0]          = mice[bestMouse].gimmeBaby(graphics, mouse, alphaMouse);
            newMice[0].isBest   = true;

            //we get a parent for every new mouse and make a baby
            for(int i = 1; i < newMice.Length; i++)
            {
                Mouse parent = selectParent(random);
                newMice[i]   = parent.gimmeBaby(graphics, mouse, alphaMouse);
            }

            //Current mice are overwritten by the new generation and we increment the gen count
            mice = newMice;
            Console.WriteLine("new gen");
            gen++;
        }

        //This is where we mutatebabies
        public void mutateBabies()
        {
            //We mutate the brain of every mouse (except the best one)
            for (int i = 1; i < mice.Length; i++)
            {
                mice[i].brain.mutate();
            }
        }

        //This function selects a parent from all mice
        public Mouse selectParent(Random random)
        {
            //generates a random number float times the fitnessSum so it has a random point in the row of possible numbers 
            float   rand        = (float)random.NextDouble() * (fitnessSum);

            //The amount of fitness we have passed
            float   runningSum  = 0;

            for(int i = 0; i < mice.Length; i++)
            {
                //we increment the runningSum with the fitness of the current mouse
                runningSum += mice[i].fitness;

                //we select a parent if the current amount of fitness is bigger than the random pointer
                if(runningSum > rand)
                {
                    return mice[i];
                }
            }
            // should never ever get here and scream bloody murderer if it does
            return null;
        }

        //Calculates the total fitness of the mice
        public void calculateFitnessSum()
        {
            fitnessSum = 0;
            for(int i = 0; i < mice.Length; i++)
            {
                Console.WriteLine("fitness = " + mice[i].fitness);
                fitnessSum += mice[i].fitness;
            }
        }

        //Sets the best mouse and the new min amount of steps so the new generation is as fast or faster then the last
        public void setBestMouse()
        {
            float   max         = 0f;
            int     maxIndex    = 0;

            for(int i = 0; i < mice.Length; i++)
            {
                //we set the current mouse as best mouse if the current fitness is the biggest one
                if(mice[i].fitness > max)
                {
                    max         = mice[i].fitness;
                    maxIndex    = i;
                }
            }

            //we set the best mouse
            bestMouse = maxIndex;

            Console.WriteLine("Best mouses fitness = " + mice[bestMouse].fitness);

            //set new min amount of steps
            if(mice[bestMouse].reachedGoal)
            {
                minStep = mice[bestMouse].brain.step;
            }
        }
    }
}
