/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2017
 *****************************************************/

/* AntAlgorithmController is the wrapper script. 
 * Drag it into your Unity Scene:
 * usage: AntAlgorithmController aac = new AntAlgortihmController(mode, alpha, beta, rho, q, numOfAnts, firstCity, acsQ0) - acsQ0 only matters if the algorithm is ACS
          aac.initAlgorithm();
          AntAlgorithm aa = aac.getAlgorithm(); 
   "An  investigation  of  some  properties of an Ant algorithm" - 1992*/
using UnityEngine;

namespace AntAlgorithms
{
    public enum Mode
    {
        antSystem, antColonySystem
    }

    public class AntAlgorithmController
    {
        private AntAlgorithm aa;
        private Mode mode;

        // influence of pheromone for decision
        private int alpha;
        // influence of distance for decision
        private int beta;
        // pheromone decrease factor
        private double rho;
        // pheromone increase factor
        private double q;
        // parameter for the ACS implementation for the pseudorandom decision rule (balance between "best so far" and "explore" tour based decision)
        private double acsQ0;
        private int numOfAnts;
        private int firstCity;

        public AntAlgorithmController(Mode mode, int alpha, int beta, double rho, double q, int numOfAnts, int firstCity, double acsQ0)
        {
            this.mode = mode;
            this.alpha = alpha;
            this.beta = beta;
            this.rho = rho;
            this.q = q;
            this.numOfAnts = numOfAnts;
            this.firstCity = firstCity;
            this.acsQ0 = acsQ0;
        }

        // inits the Algorithm. Depends on the mode you have chosen in your Unity scene.
        public void initAlgorithm()
        {
            mode = new Mode();
            switch (mode)
            {
                case Mode.antSystem:
                    aa = new ASAlgorithm(alpha, beta, rho, q, numOfAnts, firstCity);
                    break;
                case Mode.antColonySystem:
                    aa = new ACSAlgorithm(alpha, beta, rho, q, numOfAnts, firstCity, acsQ0);
                    break;
            }
        }

        public AntAlgorithm getAlgorithm()
        {
            return aa;
        }
    }
}
