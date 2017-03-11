using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntInteraction
{

    public int alpha;
    public int beta;
    public double rho;
    public double q;

    private List<Ant> ants;
    private List<City> cities;

    private Pheromones pheromones;
    private Distances distances;
    private ChoiceInfo choiceInfo;

    public int numOfAnts;

    public AntInteraction(int alpha, int beta, double rho, double q, int numOfAnts, List<City> cities)
    {
        this.cities = cities;
        this.alpha = alpha;
        this.beta = beta;
        this.rho = rho;
        this.q = q;
        this.numOfAnts = numOfAnts;

        distances = new Distances(cities);
        initAnts();
        pheromones = new Pheromones(cities.Count);
        choiceInfo = new ChoiceInfo(cities.Count);
        choiceInfo.updateChoiceInfo(pheromones, distances, alpha, beta);
        Debug.Log("Choices: " + choiceInfo.ToString);
    }

    //init the ants with random tours
    private void initAnts()
    {
        ants = new List<Ant>();
        for (int i = 0; i < numOfAnts; i++)
        {
            ants.Add(new Ant(i, cities.Count, 0, distances));
        }
    }

    //update the tours of ants by considering the pheromones
    public void updateAnts()
    {
        for (int k = 0; k < ants.Count; k++)
        {
            ants[k].clearTour();
            ants[k].unvisitAllCities();
        }
        for (int k = 0; k < ants.Count; k++)
        {
            int start = UnityEngine.Random.Range(0, cities.Count);
            ants[k].addCityToTour(cities[start].getId());
            ants[k].setCityVisited(cities[start].getId());
        }
        for (int i = 1; i < cities.Count; i++)
        {
            for (int k = 0; k < ants.Count; k++)
            {
                int nextCityIndex = asDecisionRule(i, k);
                //Debug.Log("Ant " + k + " Tour: [" + ants[k].ToString + "] NextCity:"+ nextCityIndex);

                ants[k].addCityToTour(nextCityIndex);
                ants[k].setCityVisited(nextCityIndex);
            }
        }
        for (int k = 0; k < ants.Count; k++)
        {
            ants[k].addCityToTour(ants[k].getCityOfTour(0));
            ants[k].calculateTourLength();
        }
    }

    private int asDecisionRule(int currCityIndex, int antIndex)
    {
        double[] selectionProbability = new double[cities.Count];
        double sumProbabilities = 0.0;
        int lastCity = ants[antIndex].getCityOfTour(currCityIndex - 1);
        String str = "";
        String str2 = "";

        for (int i = 0; i < selectionProbability.Length; i++)
        {
            if (ants[antIndex].isCityVisited(i)) { 
                selectionProbability[i] = 0.0;
            }
            else
            {
                selectionProbability[i] = choiceInfo.getChoice(lastCity, i);
                sumProbabilities += selectionProbability[i];
            }
        }

        double[] probs = new double[cities.Count];
        for (int i = 0; i < probs.Length; ++i)
        {
            probs[i] = selectionProbability[i] / sumProbabilities;
            str += probs[i] + " ";
        }

        double[] cumulativeProbs = new double[selectionProbability.Length + 1];

        // calculate the comulative probabilities
        for (int i = 0; i < selectionProbability.Length; i++)
        {
            cumulativeProbs[i + 1] = cumulativeProbs[i] + probs[i];
            str2 += cumulativeProbs[i] + " ";
        }

        double p = UnityEngine.Random.Range(0.0f,1.0f);

        for (int i = 0; i < cumulativeProbs.Length - 1; i++)
            if (p >= cumulativeProbs[i] && p <= cumulativeProbs[i + 1])
                return cities[i].getId();
        throw new Exception("No valid next city!");
    }

    public void updatePheromones()
    {

        for (int i = 0; i < cities.Count; i++)
        {
            for (int j = i + 1; j < cities.Count; j++)
            {
                pheromones.setPheromone(i, j, (1.0 - rho) * pheromones.getPheromone(i, j));
                pheromones.setPheromone(j, i, pheromones.getPheromone(i, j));
            }
        }
        for (int k = 0; k < ants.Count; k++)
        {
            double deltaTau = 1.0 / ants[k].getTourLength();

            for (int i = 0; i < cities.Count; i++)
            {
                int j = ants[k].getCityOfTour(i);
                int l = ants[k].getCityOfTour(i + 1);
                pheromones.setPheromone(j, l, pheromones.getPheromone(j, l) + (deltaTau * q));
                pheromones.setPheromone(l, j, pheromones.getPheromone(j, l));

            }
        }
        choiceInfo.updateChoiceInfo(pheromones, distances, alpha, beta);

        Debug.Log("Choices: " + choiceInfo.ToString);
        Debug.Log("UPDATE PHEROMONE" + pheromones.ToString);
    }

    // finds the ant with the best tour
    public Ant findBestAnt()
    {
        double minTourLength;
        int minTourLengthIndex;

        minTourLength = ants[0].getTourLength();
        minTourLengthIndex = 0;
        for (int i = 1; i < ants.Count; i++)
        {
            if (ants[i].getTourLength() < minTourLength)
            {
                minTourLength = ants[i].getTourLength();
                minTourLengthIndex = i;
            }
        }
        return ants[minTourLengthIndex];
    }

}

