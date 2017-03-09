using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntInteraction {

    public int alpha;
    public int beta;
    public double rho;
    public double q;

    private List<Ant> ants;
    private List<City> cities;
    private List<int> bestTour;

    private double bestTourLengthHelper;
    private double bestTourLength;

    private Pheromones pheromones;
    private Distances distances;

    public int numOfAnts;
    public int numOfLoops;
    public int firstCity;

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

    }

    //init the ants with random tours
    private void initAnts()
    {
        Debug.Log("------------Init ANTS with random tour each-----------");
        ants = new List<Ant>();

        for (int i = 0; i < numOfAnts; i++)
        {
            ants.Add(new Ant(i, cities.Count, firstCity, distances));
        }
    }

    //update the tours of ants by considering the pheromones
    public void updateAnts()
    {
        for (int i = 0; i < ants.Count; i++)
        {
            int start = UnityEngine.Random.Range(0, cities.Count);
            ants[i].clearTour();
            ants[i].unvisitAllCities();
            buildTrail(i, start);

            ants[i].calculateTourLength();
            //Debug.Log("The tour length of ANT " + i + " is " + ants[i].getTourLength());

        }
    }

    private void buildTrail(int antIndex, int startingCity)
    {
        ants[antIndex].addCityToTour(cities[startingCity].getId());
        ants[antIndex].setCityVisited(startingCity);

        for (int i = 0; i < cities.Count - 1; i++)
        {

            int currCityIndex = ants[antIndex].getCityOfTour(i);
            int nextCityIndex = calculateNextCity(currCityIndex, antIndex);
          //  Debug.Log("City " + currCityIndex + " selected. NEXT city is: " + nextCityIndex);

            ants[antIndex].addCityToTour(nextCityIndex);
            ants[antIndex].setCityVisited(nextCityIndex);
        }
    }

    private int calculateNextCity(int currCityIndex, int antIndex)
    {
        double[] probabilities = calculateMoveProbabilities(currCityIndex, antIndex);
        double[] cumulativeProbs = new double[probabilities.Length + 1];

        // calculate the comulative probabilities
        for (int i = 0; i < probabilities.Length; i++)
            cumulativeProbs[i + 1] = cumulativeProbs[i] + probabilities[i];

        // calculate a random threshold
        double p = UnityEngine.Random.Range(0, 1);

        for (int i = 0; i < cumulativeProbs.Length - 1; i++)
            if (p >= cumulativeProbs[i] && p < cumulativeProbs[i + 1])
                return cities[i].getId();
        throw new Exception("No valid next city!");
    }

    //calculate the probabilities for moving to a certain city
    private double[] calculateMoveProbabilities(int currCityIndex, int antIndex)
    {
        double[] taueta = new double[cities.Count];
        double sum = 0.0;

        for (int i = 0; i < taueta.Length; i++)
        {
            if (cities[i].getId() == currCityIndex)            
                taueta[i] = 0.0;         
            else if (ants[antIndex].isCityVisited(i))
                taueta[i] = 0.0;
            else
            {
                // core function for calculating probabilities // From DORIGO97
                taueta[i] = Math.Pow(pheromones.getPheromone(currCityIndex, i), alpha) *
                Math.Pow((1.0 / distances.getDistance(currCityIndex, i)), beta);

                if (taueta[i] < 0.0001)
                    taueta[i] = 0.0001;
                else if (taueta[i] > (double.MaxValue / (cities.Count * 100)))
                    taueta[i] = double.MaxValue / (cities.Count * 100);
            }
            sum += taueta[i];
        }
        double[] probs = new double[cities.Count];
        for (int i = 0; i < probs.Length; ++i)
        {
            probs[i] = taueta[i] / sum;

        }
        return probs;
    }

    public void updatePheromones()
    {
        for (int i = 0; i < pheromones.getCount(); i++)
        {
            for (int j = i + 1; j < pheromones.getCount(); j++)
            {
                for (int k = 0; k < ants.Count; k++)
                {
                    double length = ants[k].getTourLength();
                    double decrease = (1.0 - rho) * pheromones.getPheromone(i, j);
                    double increase = 0.0;

                    // if the current edge is visited by the current ant
                    if (ants[k].isEdge(i, j))
                    {
                        // Debug.Log("Ant [" + k + "] There is an edge between City " + i + " and City " + j + "!");
                        increase = (q / length);
                    }

                    pheromones.setPheromone(i, j, decrease + increase);

                    // 0.0001 to 10000.0
                    if (pheromones.getPheromone(i, j) < 0.0001)
                    {
                        pheromones.setPheromone(i, j, 0.0001);
                    }
                    else if (pheromones.getPheromone(i, j) > 100000.0)
                    {
                        pheromones.setPheromone(i, j, 100000.0);
                    }

                    pheromones.setPheromone(j, i, pheromones.getPheromone(i, j));
                }
            }
        }
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

