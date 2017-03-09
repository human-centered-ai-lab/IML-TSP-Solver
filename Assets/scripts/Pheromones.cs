/* Pheromones is a wrapper class for pheromones between cities in 2D space.
 * PROGRAMMED BY Andrej Müller (andrej.mueller@student.tugraz.at), student of TU Graz - University of Technology
 * */

using UnityEngine;

public class Pheromones 
{
    public static double initPheromoneValue = 0.01;
    // x is the city A and y is the city B
    private double[][] pheromones;
    private int count;

    public Pheromones(int numOfCities)
    {
        initializePheromones(numOfCities);
        count = numOfCities;
    }

    private void initializePheromones(int numOfCities)
    {
        pheromones = new double[numOfCities][];
        for (int i = 0; i < numOfCities; i++)
            pheromones[i] = new double[numOfCities];
        for (int i = 0; i < pheromones.Length; i++)
            for (int j = 0; j < pheromones[i].Length; j++)
                pheromones[i][j] = initPheromoneValue;

        Debug.Log("Pheromones: "+ this.ToString);
    }

    public double getPheromone(int cityAId, int cityBId)
    {
        return pheromones[cityAId][cityBId];
    }

    public int getCount()
    {
        return count;
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

    public void setPheromone(int cityAId, int cityBId, double amount)
    {
        pheromones[cityAId][cityBId] = amount;
    }
}
