using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doolhof.classes
{
    class Brain
    {
        //An array filled with directions 
        public Vector2[]    directions;
        public Random       random;

        //The current step and the stepsize in pixels
        public int          step            = 0;     
        public int          stepSize        = 15;
        float               mutationRate    = 0.01f;

        public Brain(int size, Random inputRandom, Vector2 position)
        {
            //We set the random and the random directions
            random      = inputRandom;
            directions  = new Vector2[size];
            randomize(position);
        }

        //This function randomizes initial generation's positions
        public void randomize(Vector2 position)
        {
            //We first grab the starting position (we use these to create posible position (so the mice won't teleport across the screen))
            float lastX = position.X;
            float lastY = position.Y;
            
            //we randomize a position for every index in the array
            for(int i = 0; i < directions.Length; i++)
            {
                //We generate a random chance to move in one of eight positions
                Vector2 randomvector;
                
                float randX = randomFloat();
                float randY = randomFloat();

                if (randX < 0.3f)
                {
                    randomvector.X = stepSize;
                }
                else if (randX < 0.6f)
                {
                    randomvector.X = -stepSize;
                }
                else
                {
                    randomvector.X = 0;
                }

                if (randY < 0.3f)
                {
                    randomvector.Y = stepSize;
                }
                else if (randY < 0.6f)
                {
                    randomvector.Y = -stepSize;
                }
                else
                {
                    randomvector.Y = 0;
                }

                directions[i]   = randomvector;
            }
        }

        //We clone the brain so it can be used by a new child
        public Brain clone()
        {
            //We create a genetically engineered brain to put into a mouse (with the same start conditions and directions)
            Brain clone = new Brain(directions.Length, random, directions[0]);
            for(int i = 0; i < directions.Length; i++)
            {
                clone.directions[i] = directions[i];
            }
            return clone;
        }

        //The is where the mutation happends
        public void mutate()
        {
            //We give every step a mouse takes a random chance to mutate
            for(int i = 0; i < directions.Length; i++)
            {
                // we have a really small chance to mutate a step
                if(randomFloat() < mutationRate)
                {
                    //We set two random floats to randomize rand x ~ y
                    float   randX           = randomFloat();
                    float   randY           = randomFloat();
                    Vector2 randomvector;

                    if (randX < 0.3f)
                    {
                        randomvector.X = stepSize;
                    }
                    else if(randX < 0.6f)
                    {
                        randomvector.X = -stepSize;
                    }
                    else
                    {
                        randomvector.X = 0;
                    }

                    if (randY < 0.3f)
                    {
                        randomvector.Y = stepSize;
                    }
                    else if(randY < 0.6f)
                    {
                        randomvector.Y = -stepSize;
                    }
                    else
                    {
                        randomvector.Y = 0;
                    }

                    directions[i]      = randomvector;
                }
            }
        }

        //Creates a randomFloat
        public float randomFloat()
        {
            float number = (float)random.NextDouble();
            return number;
        }
    }
}
