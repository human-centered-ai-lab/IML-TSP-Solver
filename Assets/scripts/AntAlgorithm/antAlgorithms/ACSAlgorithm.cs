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
        // parameter for the ACS implementation for the pseudorandom decision rule (balance between "best so far" and "explore" tour based decision)
        private double acsQ0;
        // parameter for the local phermone update
        private double xi;
        private double tau0;

        public ACSAlgorithm(int alpha, int beta, double rho, double q, int numOfAnts, int firstCity, double pheromoneTrailInitialValue, double acsQ0, double xi, double tau0)
        {
            this.alpha = alpha;
            this.beta = beta;
            this.rho = rho;
            this.q = q;
            this.numOfAnts = numOfAnts;
            this.firstCity = firstCity;
            this.acsQ0 = acsQ0;
            this.tau0 = tau0;
            this.xi = xi;
        }

        public override void init()
        {
            antin = new AntInteraction(Mode.antColonySystem, alpha, beta, rho, q, numOfAnts, cities, firstCity, pheromoneTrailInitialValue, acsQ0, xi, tau0);
            bestTour = new List<int>();
            tourLength = double.MaxValue;
            checkBestTour();
            algStep = 1;
        }

        public override void iteration()
        {
            antin.updateAnts();
            antin.globalPheromoneUpdateACS();
            checkBestTour();
        }

        public override void step()
        {
        }
    }
}

