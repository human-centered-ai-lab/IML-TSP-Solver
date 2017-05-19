/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2017
 *****************************************************/

/* Pheromone represents the pheromones between cities */

public class Pheromones
{
    // initialization factor for pheromones
    private double initPheromoneValue;
    // Matrix of pheromones between city x and city y
    private double[][] pheromones;
    private int numOfCities;

    public Pheromones(int numOfCities, double initPheromoneValue)
    {
        this.numOfCities = numOfCities;
        this.initPheromoneValue = initPheromoneValue;
    }

    // init of pheromones
    public void init()
    {
        pheromones = new double[numOfCities][];
        for (int i = 0; i < numOfCities; i++)
            pheromones[i] = new double[numOfCities];
        for (int i = 0; i < pheromones.Length; i++)
            for (int j = 0; j < pheromones[i].Length; j++)
                pheromones[i][j] = initPheromoneValue;
    }

    public new string ToString
    {
        get
        {
            string str = "";
            for (int i = 0; i < pheromones.Length; i++)
            {
                str += "\n";
                for (int j = 0; j < pheromones[i].Length; j++)
                    str += pheromones[i][j] + " ";
            }
            return str;
        }
    }

    // decrease the pheromone value between 2 particular cities by one ant 
    public void decreasePheromoneAS(int cityAId, int cityBId, double decreaseValue)
    {
        pheromones[cityAId][cityBId] = decreaseValue * pheromones[cityAId][cityBId];
    }

    // decrease the pheromone value between 2 particular cities by one ant 
    public void increasePheromoneAS(int cityAId, int cityBId, double increaseValue)
    {
        pheromones[cityAId][cityBId] = pheromones[cityAId][cityBId] + increaseValue;
    }

    public void setPheromone(int cityAId, int cityBId, double value)
    {
        pheromones[cityAId][cityBId] = value;
    }

    public double getPheromone(int cityAId, int cityBId)
    {
        return pheromones[cityAId][cityBId];
    }
}
