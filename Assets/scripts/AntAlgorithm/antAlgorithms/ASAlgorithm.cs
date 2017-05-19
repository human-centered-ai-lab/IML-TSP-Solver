/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2017
 *****************************************************/

/* ASAlgorithm represents the AS ant algorithm implementation:
   "An  investigation  of  some  properties of an Ant algorithm" - 1992*/

using System.Collections.Generic;

namespace AntAlgorithms
{
    public class ASAlgorithm : AntAlgorithm
    {
        public ASAlgorithm(int alpha, int beta, double q, int numOfAnts, double pheromoneTrailInitialValue)
        {
            this.alpha = alpha;
            this.beta = beta;
            this.q = q;
            this.numOfAnts = numOfAnts;
            this.pheromoneTrailInitialValue = pheromoneTrailInitialValue;
        }

        public ASAlgorithm(int alpha, int beta, double q, int numOfAnts)     
        {
            this.alpha = alpha;
            this.beta = beta;
            this.q = q;
            this.numOfAnts = numOfAnts;
            this.pheromoneTrailInitialValue = AntAlgorithmChooser.PHERDEFAULTINITVALUE;
        }

        public override void init()
        {
            antin = new AntInteraction(Mode.antSystem, alpha, beta, q, numOfAnts, cities, pheromoneTrailInitialValue, 0, 0);
            bestTour = new List<int>();
            tourLength = double.MaxValue;
            checkBestTour();
            algStep = 1;
        }

        public override void iteration()
        {
            antin.updateAnts();
            antin.updatePheromonesAS();
            checkBestTour();
        }

        public override void step()
        {
            if (antin.updateAntsStepwise(algStep))
            {
                algStep = 1;
                antin.updatePheromonesAS();
                checkBestTour();
            }
            else
            {
                algStep++;
            }
        }
    }
}
