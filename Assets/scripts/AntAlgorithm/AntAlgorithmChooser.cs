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
        private AntAlgorithm aa;

        public AntAlgorithmChooser(Mode mode, int alpha, int beta, double rho, double q, int numOfAnts, int firstCity, double acsQ0, double pheromoneTrailInitialValue, double xi, double tau0)
        {
            // TODO: intelligent switch due to parameters
            switch (mode)
            {
                case Mode.antSystem:
                    aa = new ASAlgorithm(alpha, beta, rho, q, numOfAnts, firstCity, pheromoneTrailInitialValue);
                    break;
                case Mode.antColonySystem:
                    aa = new ACSAlgorithm(alpha, beta, rho, q, numOfAnts, firstCity, acsQ0, pheromoneTrailInitialValue, xi, tau0);
                    break;
            }
        }

        public AntAlgorithm getAlgorithm()
        {
            return aa;
        }
    }
}
