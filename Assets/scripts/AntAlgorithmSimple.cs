/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2017
 *****************************************************/

/* AntAlgorithmSimple is a wrapper class for the Ant Algorithm 
   -> use this script in your Unity scene */

using System.Collections.Generic;
using UnityEngine;

namespace AntAlgorithms
{
    public enum Mode
    {
        normal
    }

    public class AntAlgorithmSimple : MonoBehaviour
    {
        // influence of pheromone for decision
        public int alpha = 3;
        // influence of distance for decision
        public int beta = 2;
        // pheromone decrease factor
        public double rho = 0.01;
        // pheromone increase factor
        public double q = 2.0;

        private List<City> cities;
        // Ant interactions
        private AntInteraction antin;

        public int numOfAnts;
        public int firstCity;

        //placeholder
        public Mode mode;

        // output - updateing after every algorithm iteration
        private double tourLength;
        private List<int> bestTour;

        //helper
        public int algStep = 1;

        // inits step of the algorithm
        public void init()
        {
            Debug.Log("######################### Begin Ant Colony Optimization: INIT ########################");
            antin = new AntInteraction(alpha, beta, rho, q, numOfAnts, cities);

            Ant bestAnt = antin.findBestAnt();
            tourLength = bestAnt.getTourLength();
            Debug.Log("Best Dist: " + tourLength);

            Debug.Log("######################################################################################");

            algStep = 1;

        }

        // iteration step of the algorithm - calculates a complete tour for every ant
        public void iteration()
        {

            Debug.Log("######################### Begin Ant Colony Optimization: ITERATION ########################");

            antin.updateAnts();
            antin.updatePheromones();

            Ant bestAntTemp = antin.findBestAnt();
            double tourLengthTemp = bestAntTemp.getTourLength();

            if (tourLengthTemp < tourLength)
            {
                tourLength = tourLengthTemp;
            }

            Debug.Log("::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
            Debug.Log("Best Dist: " + tourLength);

        }

        // all ants are moving one city ahead. If the routes are completed, a new iteration is starting.
        public void step()
        {

            Debug.Log("######################### Begin Ant Colony Optimization: STEP " + algStep + "########################");

            if (antin.updateAntsStepwise(algStep))
            {
                algStep = 1;
                antin.updatePheromones();

                Ant bestAntTemp = antin.findBestAnt();
                double tourLengthTemp = bestAntTemp.getTourLength();

                if (tourLengthTemp < tourLength)
                {
                    tourLength = tourLengthTemp;
                }

                Debug.Log("::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
                Debug.Log("Best Dist: " + tourLength);
            }else
            {
                algStep++;
            }
        }

        // set cities before the initialization
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

    }
}
