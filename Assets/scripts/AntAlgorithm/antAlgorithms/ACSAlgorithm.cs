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
        //increasing parameter for the local pheromone update
        private double tau0 = -1;

        public ACSAlgorithm(int alpha, int beta, double q, int numOfAnts, double pheromoneTrailInitialValue, double acsQ0, double tau0)
        {
            this.alpha = alpha;
            this.beta = beta;
            this.q = q;
            this.numOfAnts = numOfAnts;
            this.acsQ0 = acsQ0;
            this.tau0 = tau0;
            this.pheromoneTrailInitialValue = pheromoneTrailInitialValue;
        }

        public ACSAlgorithm(int alpha, int beta, double q, int numOfAnts, double acsQ0)
        {
            this.alpha = alpha;
            this.beta = beta;
            this.q = q;
            this.numOfAnts = numOfAnts;
            this.acsQ0 = acsQ0;
        }

        public override void init()
        {
            antin = new AntInteraction(Mode.antColonySystem, alpha, beta, q, numOfAnts, cities, pheromoneTrailInitialValue, acsQ0, tau0);
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
            if (antin.updateAntsStepwise(algStep))
            {
                algStep = 1;
                antin.globalPheromoneUpdateACS();
                checkBestTour();
            }
            else
            {
                algStep++;
            }
        }
    }
}

