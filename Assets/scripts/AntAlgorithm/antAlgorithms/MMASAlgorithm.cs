/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2017
 *****************************************************/

/* ASAlgorithm represents the AS ant algorithm implementation:
   "An  investigation  of  some  properties of an Ant algorithm" - 1992*/

using AntAlgorithms.interaction;
using System.Collections.Generic;
using UnityEngine;

namespace AntAlgorithms
{
    public class MMASAlgorithm : AntAlgorithm
    {
        private double pBest;
        private double smoothingFactor;

        public MMASAlgorithm(int alpha, int beta, double q, int numOfAnts, double pBest, double smoothingFactor)
        {
            this.pBest = pBest;
            this.alpha = alpha;
            this.beta = beta;
            this.q = q;
            this.numOfAnts = numOfAnts;
            this.smoothingFactor = smoothingFactor;
            this.pheromoneTrailInitialValue = AntAlgorithmChooser.PHERDEFAULTINITVALUE;
        }

        public override void Init()
        {
            antin = new MMASAntInteraction(alpha, beta, q, numOfAnts, Cities, pBest);
            BestTour = new List<int>();
            TourLength = double.MaxValue;
            CheckBestTour();
            AlgStep = 1;
        }

        public override void Iteration()
        {
            AlgStep = 1;
            CurrentIteration++;
            antin.UpdateAnts();
            antin.UpdatePheromones();
            if (!CheckBestTour())
            {
                antin.reinitPheromones(smoothingFactor);
            }            
        }

        public override void Step()
        {
            if (antin.UpdateAntsStepwise(AlgStep))
            {
                CurrentIteration++;
                AlgStep = 1;
                antin.UpdatePheromones();
                CheckBestTour();
            }
            else
            {
                AlgStep++;
            }
        }
    }
}
