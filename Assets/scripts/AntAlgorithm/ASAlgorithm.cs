/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2017
 *****************************************************/

/* AntAlgorithmSimple is a wrapper class for the Ant Algorithm 
   -> use this script in your Unity scene */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace AntAlgorithms
{
    public class ASAlgorithm : AntAlgorithm
    {
        public override void init()
        {
            alpha = 1;
            beta = 2;
            rho = 0.07;
            q = 100;

            antin = new AntInteraction(alpha, beta, rho, q, numOfAnts, cities, firstCity);
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
            if (antin.updateAntsStepwise(algStep))
            {
                algStep = 1;
                antin.updatePheromones();
                checkBestTour();
            }
            else
            {
                algStep++;
            }
        }
    }
}
