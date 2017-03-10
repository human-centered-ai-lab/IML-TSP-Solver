using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceInfo {

    private double[][] choiceInfo;
    private int size;

    public ChoiceInfo(int numOfCities)
    {
        this.size = numOfCities;
        choiceInfo = new double[numOfCities][];
        for (int i = 0; i < numOfCities; i++)
            choiceInfo[i] = new double[numOfCities];
    }

    public void updateChoiceInfo(Pheromones pheromones, Distances distances, int alpha, int beta)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = i + 1; j < size; j++)
            {
                choiceInfo[i][j] = Math.Pow(pheromones.getPheromone(i, j), alpha) *
                                   Math.Pow((1.0 / distances.getDistance(i, j)), beta);
               /* if (choiceInfo[i][j] < 0.0001)
                    choiceInfo[i][j] = 0.0001;
                else if (choiceInfo[i][j] > (double.MaxValue / (size * 100)))
                    choiceInfo[i][j] = double.MaxValue / (size * 100);*/

                choiceInfo[j][i] = choiceInfo[i][j];
            }
        }
    }

    public double getChoice(int cityA, int cityB)
    {
        return choiceInfo[cityA][cityB];
    }

    public new string ToString
    {
        get
        {
            string str = "";
            for (int i = 0; i < choiceInfo.Length; i++)
            {
                str += "\n";
                for (int j = 0; j < choiceInfo[i].Length; j++)
                    str += choiceInfo[i][j] + " ";
            }
            return str;
        }
    }
}
