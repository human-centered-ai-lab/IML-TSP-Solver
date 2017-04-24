/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2017
 *****************************************************/

/* AntAlgorithmController is the wrapper script. 
 * Drag it into your Unity Scene:
 * usage: AntAlgorithmController aac = GetComponent<AntAlgorithms.AntAlgorithmController>();
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

    public class AntAlgorithmController : MonoBehaviour
    {
        private AntAlgorithm aa;
        // influence of pheromone for decision
        public int alpha;
        // influence of distance for decision
        public int beta;
        // pheromone decrease factor
        public double rho;
        // pheromone increase factor
        public double q;
        // parameter for the ACS implementation for the pseudorandom decision rule (balance between "best so far" and "explore" tour based decision)
        public double acsQ0;

        public int numOfAnts;
        public int firstCity;

        // inits the Algorithm. Depends on the mode you have chosen in your Unity scene.
        public void initAlgorithm()
        {
            Mode mode = new Mode();
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
