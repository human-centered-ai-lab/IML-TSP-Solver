﻿/****************************************************
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
        public ASAlgorithm(int alpha, int beta, double rho, double q, int numOfAnts, int firstCity, double pheromoneTrailInitialValue)
        {
            this.alpha = alpha;
            this.beta = beta;
            this.rho = rho;
            this.q = q;
            this.numOfAnts = numOfAnts;
            this.firstCity = firstCity;
            this.pheromoneTrailInitialValue = pheromoneTrailInitialValue;
        }

        public override void init()
        {
            antin = new AntInteraction(Mode.antSystem, alpha, beta, rho, q, numOfAnts, cities, firstCity, pheromoneTrailInitialValue);
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
