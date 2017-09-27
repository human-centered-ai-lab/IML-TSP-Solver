/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2017
 *****************************************************/

/* AntAlgorithm is an abstract class for all antAlgorithms 
*/

using System.Collections.Generic;
using UnityEngine;

namespace AntAlgorithms
{
    public abstract class AntAlgorithm
    {
        public static int REINITIALIZATIONAFTERXITERATIONS = 400;

        // influence of pheromone for decision
        protected int alpha;
        // influence of distance for decision
        protected int beta;
        // pheromone increase/decrease factor
        protected double q;

        // Ant interactions
        protected AntInteraction antin;

        protected int numOfAnts;
        protected double pheromoneTrailInitialValue = -1;

        // output - updateing after every algorithm iteration

        //helper
        protected int algStep;
        private int reinitializationThreshhold = REINITIALIZATIONAFTERXITERATIONS;
        private int iteration = 0;


        // inits step of the algorithm
        // usage: use it once for initialization
        public abstract void Init();

        // iteration step of the algorithm - calculates a complete tour for every ant
        // usage: you can use it several times
        public abstract void Iteration();

        // all ants are moving one city ahead. If the routes are completed, a new iteration is starting.
        // usage: small part of the iteration. you should use it several times to complete an iteration
        public abstract void Step();

        // debug output for best tour
        public void PrintBestTour(string context, int offset)
        {
            string str = "";
            foreach (int cityIndex in BestTour)
            {
                str += (cityIndex + offset) + " ";
            }
            Debug.Log("[" + context + "] Best Dist: " + TourLength + " Tour: " + str);
        }

        //returns false if reinitialization is needed.
        protected bool CheckBestTour()
        {
            iteration++;
            Ant bestAnt = antin.FindBestAnt();
            double tourLengthTemp = bestAnt.TourLength;
            reinitializationThreshhold--;

            if (tourLengthTemp < TourLength)
            {
                reinitializationThreshhold = REINITIALIZATIONAFTERXITERATIONS;
                TourLength = tourLengthTemp;
                BestTour.Clear();
                for (int i = 0; i < bestAnt.Tour.Count; i++)
                {
                    BestTour.Add(bestAnt.Tour[i]);
                }
            }
            if (reinitializationThreshhold <= 0)
            {
                reinitializationThreshhold = REINITIALIZATIONAFTERXITERATIONS;
                return false;
            }

            return true;
        }

        // usage: set cities before the initialization
        public List<City> Cities { get; set; }

        // after the initialization you can modify each ant
        public Ant GetAnt(int antIndex)
        {
            return antin.Ants[antIndex];
        }

        // after the initialization you can modify the pheromones
        public Pheromones Pheromones
        {
            get { return antin.Pheromones; }
        }

        public double TourLength { get; protected set; }

        public List<int> BestTour { get; protected set; }
    }
}
