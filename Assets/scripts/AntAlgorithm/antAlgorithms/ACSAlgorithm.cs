/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2017
 *****************************************************/

/* ACSAlgorithm represents the ACS ant algorithm implementation:
   "Ant colony system: a cooperative learning approach to the traveling salesman problem" - 1996*/

using System.Collections.Generic;

namespace AntAlgorithms
{
    public class ACSAlgorithm : AntAlgorithm
    {
        public ACSAlgorithm(int alpha, int beta, double rho, double q, int numOfAnts, int firstCity, double acsQ0)
        {
            this.alpha = alpha;
            this.beta = beta;
            this.rho = rho;
            this.q = q;
            this.numOfAnts = numOfAnts;
            this.firstCity = firstCity;
            this.acsQ0 = acsQ0;
        }
        public override void init()
        {
            antin = new AntInteraction(alpha, beta, rho, q, numOfAnts, cities, firstCity, acsQ0);
            bestTour = new List<int>();
            tourLength = double.MaxValue;
            checkBestTour();
            algStep = 1;
        }

        public override void iteration()
        {
            antin.updateAnts();
            antin.updatePheromones();
            checkBestTour();
        }

        public override void step()
        {
        }
    }
}

