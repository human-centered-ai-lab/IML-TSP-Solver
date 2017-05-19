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
        // influence of pheromone for decision
        protected int alpha;
        // influence of distance for decision
        protected int beta;
        // pheromone increase/decrease factor
        protected double q;

        protected List<City> cities;
        // Ant interactions
        protected AntInteraction antin;

        protected int numOfAnts;
        protected double pheromoneTrailInitialValue = -1;

        // output - updateing after every algorithm iteration
        protected double tourLength;
        protected List<int> bestTour;

        //helper
        protected int algStep;

        // inits step of the algorithm
        // usage: use it once for initialization
        public abstract void init();

        // iteration step of the algorithm - calculates a complete tour for every ant
        // usage: you can use it several times
        public abstract void iteration();

        // all ants are moving one city ahead. If the routes are completed, a new iteration is starting.
        // usage: small part of the iteration. you should use it several times to complete an iteration
        public abstract void step();

        // debug output for best tour
        public void printBestTour(string context)
        {
            string str = "";
            for (int i = 0; i < bestTour.Count; i++)
                str += bestTour[i] + " ";
            Debug.Log("[" + context + "] Best Dist: " + tourLength + " Tour: " + str);
        }

        protected bool checkBestTour()
        {
            Ant bestAnt = antin.findBestAnt();
            double tourLengthTemp = bestAnt.getTourLength();

            if (tourLengthTemp < tourLength)
            {
                tourLength = tourLengthTemp;
                bestTour.Clear();
                for (int i = 0; i < bestAnt.getTour().Count; i++)
                    bestTour.Add(bestAnt.getTour()[i]);
                return true;
            }
            return false;
        }

        // usage: set cities before the initialization
        public void setCities(List<City> cities)
        {
            this.cities = cities;
        }

        // after the initialization you can modify each ant
        public Ant getAnt(int i)
        {
            return antin.getAnts()[i];
        }

        // after the initialization you can modify the pheromones
        public Pheromones getPheromones()
        {
            return antin.getPheromones();
        }

        public double getTourLength()
        {
            return tourLength;
        }

        public List<int> getTour()
        {
            return bestTour;
        }
    }
}
