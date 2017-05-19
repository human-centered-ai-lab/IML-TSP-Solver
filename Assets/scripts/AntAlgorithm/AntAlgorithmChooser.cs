/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2017
 *****************************************************/

/* AntAlgorithmController is the wrapper script. 
 * Drag it into your Unity Scene:
 * usage: AntAlgorithmChooser aac = new AntAlgorithmChooser(mode, alpha, beta, rho, q, numOfAnts, firstCity, acsQ0) - acsQ0 only matters if the algorithm is ACS
          AntAlgorithm aa = aac.getAlgorithm(); 
   "An  investigation  of  some  properties of an Ant algorithm" - 1992 
    */

using System.Reflection;
[assembly: AssemblyVersionAttribute("0.1")]

namespace AntAlgorithms
{
    public enum Mode
    {
        antSystem, antColonySystem
    }

    public class AntAlgorithmChooser
    {
        public static double PHERDEFAULTINITVALUE = 0.1;

        private AntAlgorithm aa;

        public AntAlgorithmChooser(Mode mode, int alpha, int beta, double q, int numOfAnts, double acsQ0, double pheromoneTrailInitialValue, double tau0)
        {
            // TODO: intelligent switch due to parameters
            switch (mode)
            {
                case Mode.antSystem:
                    aa = new ASAlgorithm(alpha, beta, q, numOfAnts, pheromoneTrailInitialValue);
                    break;
                case Mode.antColonySystem:
                    aa = new ACSAlgorithm(alpha, beta, q, numOfAnts, pheromoneTrailInitialValue, acsQ0,  tau0);
                    break;
            }
        }

        public AntAlgorithmChooser(int alpha, int beta, double q, int numOfAnts)
        {
            aa = new ASAlgorithm(alpha, beta, q, numOfAnts);
        }

        public AntAlgorithmChooser(int alpha, int beta, double q, int numOfAnts,  double acsQ0)
        {
            aa = new ACSAlgorithm(alpha, beta, q, numOfAnts, acsQ0);
        }

        public AntAlgorithm getAlgorithm()
        {
            return aa;
        }
    }
}
