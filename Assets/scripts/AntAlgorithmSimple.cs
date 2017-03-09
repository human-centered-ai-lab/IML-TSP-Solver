/* THE SIMPLE ACO algorithm by Dorigo et. al. solving the TSP Problem 
 * PROGRAMMED BY Andrej Müller (andrej.mueller@student.tugraz.at), student of TU Graz - University of Technology
 * */

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
        //variables for the ACO basic algorithm

        // influence of pheromone on direction
        public int alpha = 3;
        // influence of distance
        public int beta = 2;

        // pheromone decrease factor
        public double rho = 0.01;
        // pheromone increase factor
        public double q = 2.0;

        private List<City> cities;
        private AntInteraction antin;
        public int numOfAnts;
        public int firstCity;
        public Mode mode;

        private Ant bestAnt;

        public void setCities(List<City> cities)
        {
            this.cities = cities;
        }

        public List<City> getCities()
        {
            return cities;
        }

        public void init()
        {
            Debug.Log("######################### Begin Ant Colony Optimization: INIT ########################");
            antin = new AntInteraction(alpha, beta, rho, q, numOfAnts, cities);

            bestAnt = antin.findBestAnt();
            Debug.Log("Best Ant: " + bestAnt.ToString);

            Debug.Log("######################################################################################");

        }

        public void step()
        {

            Debug.Log("######################### Begin Ant Colony Optimization: STEP ########################");

            antin.updateAnts();
            antin.updatePheromones();

            Ant bestAntTemp = antin.findBestAnt();


            if (bestAntTemp.getTourLength() < bestAnt.getTourLength())
                bestAnt = bestAntTemp;

            Debug.Log("Length of best trail found!");
            Debug.Log("::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
            Debug.Log("Best Ant: " + bestAnt.ToString);
            Debug.Log("::::::::::::::::::::::::::::::::::::::::::::::::::::::::");

            Debug.Log("######################################################################################");

        }

    }
}
