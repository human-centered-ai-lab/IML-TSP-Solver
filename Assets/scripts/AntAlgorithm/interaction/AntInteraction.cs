/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2017
 *****************************************************/

/* AntInteraction represents the interaction with Ants */

using System;
using System.Collections.Generic;
using UnityEngine;

public class AntInteraction
{
    private static int noValidNextCity = -1;

    private int alpha;
    private int beta;
    private double q;
    private double acsQ0;

    private double tau0;
    private double pheromoneTrailInitialValue;

    private int numOfAnts;
    private List<Ant> ants;
    private List<City> cities;

    private Pheromones pheromones;
    private Distances distances;
    private ChoiceInfo choiceInfo;
    private int startCity;
    private AntAlgorithms.Mode mode;

    //helper
    private double[] selectionProbability;
    private double sumProbabilities;
    private int bestProbIndex;
    private double[] cumulativeProbs;
    private double[] probs;
    private double bestGlobalTourLength;
    private int pheromoneIncreaseFactorParameter = 100;

    //helper flags
    private bool tourComplete = false;

    private System.Random random = new System.Random();

    public AntInteraction(AntAlgorithms.Mode mode, int alpha, int beta, double q, int numOfAnts, List<City> cities, double pheromoneTrailInitialValue, double acsQ0, double tau0)
    {
        this.cities = cities;
        this.alpha = alpha;
        this.beta = beta;
        this.q = q;
        this.numOfAnts = numOfAnts;
        this.mode = mode;
        this.pheromoneTrailInitialValue = pheromoneTrailInitialValue;
        this.tau0 = tau0;
        this.acsQ0 = acsQ0;

        //calculates distances
        distances = new Distances(cities);

        // calculations for variable parameters
        if (tau0 == -1)
            this.tau0 = 1.0 / (cities.Count * distances.calculateNNHeuristic());
        if (pheromoneTrailInitialValue == -1)
            this.pheromoneTrailInitialValue = this.tau0;

        // init ants with random start point
        initAnts(true);
        initPheromones();

        choiceInfo = new ChoiceInfo(cities.Count);
        choiceInfo.updateChoiceInfo(pheromones, distances, alpha, beta);

        selectionProbability = new double[cities.Count];
        cumulativeProbs = new double[selectionProbability.Length + 1];
        probs = new double[cities.Count];
    }

    //update the tours of ants by considering the pheromones
    public void updateAnts()
    {
        initAntUpdate();
        bool error = true;

        for (int i = 1; i < cities.Count; i++)
        {
            switch (mode)
            {
                case AntAlgorithms.Mode.antSystem:
                    error = moveAntsAS(i);
                    break;
                case AntAlgorithms.Mode.antColonySystem:
                    error = moveAntsACS(i);
                    break;
            }

            if (!error)
                throw new Exception("No valid next city!");
        }
        completeTours();
    }

    // updates an ant stepwise every city
    public bool updateAntsStepwise(int citiesSoFar)
    {
        bool lastCity = false;

        if (!tourComplete)
        {
            if (citiesSoFar == 1)
                initAntUpdate();
            else
            {
                switch (mode)
                {
                    case AntAlgorithms.Mode.antSystem:
                        lastCity = !moveAntsAS(citiesSoFar - 1);
                        break;
                    case AntAlgorithms.Mode.antColonySystem:
                        lastCity = !moveAntsACS(citiesSoFar - 1);
                        break;
                }
                if (lastCity)
                    completeTours();
            }
        }
        return tourComplete;
    }

    /*#########################################################################
     *                    ANT SYSTEM (AS) INTERACTION
     *#########################################################################
     */

    // moves all ants one city ahead. returns false, if no city is available
    private bool moveAntsAS(int currentCityPos)
    {
        for (int k = 0; k < ants.Count; k++)
        {
            int nextCityIndex = asDecisionRule(currentCityPos, k);
            if (nextCityIndex == noValidNextCity)
                return false;

            ants[k].addCityToTour(nextCityIndex);
            ants[k].setCityVisited(nextCityIndex);
        }
        return true;
    }

    // the core of the as algorithm: what city should the  ant select next
    private int asDecisionRule(int currCityIndex, int antIndex)
    {
        calculateProbs(currCityIndex, antIndex);
        return explorationDecision();
    }

    //calculation of the probabilities 
    private void calculateProbs(int currCityIndex, int antIndex)
    {
        sumProbabilities = 0.0;

        int currentCity = ants[antIndex].getCityOfTour(currCityIndex - 1);

        for (int i = 0; i < selectionProbability.Length; i++)
        {
            if (ants[antIndex].isCityVisited(i))
            {
                selectionProbability[i] = 0.0;
            }
            else
            {
                selectionProbability[i] = choiceInfo.getChoice(currentCity, i);
                sumProbabilities += selectionProbability[i];
            }
        }

        double prob_temp = 0.0;

        bestProbIndex = -1;

        for (int i = 0; i < probs.Length; i++)
        {
            probs[i] = selectionProbability[i] / sumProbabilities;
            if (probs[i] > prob_temp)
            {
                bestProbIndex = i;
                prob_temp = probs[i];
            }
        }
    }

    // explore new edges by adding randomness
    private int explorationDecision()
    {
        // calculate the comulative probabilities
        for (int i = 0; i < selectionProbability.Length; i++)
            cumulativeProbs[i + 1] = cumulativeProbs[i] + probs[i];
        cumulativeProbs[cumulativeProbs.Length - 1] = 1.0f;
        double p = random.NextDouble();

        for (int i = 0; i < cumulativeProbs.Length - 1; i++)
            if (p >= cumulativeProbs[i] && p <= cumulativeProbs[i + 1])
                return cities[i].getId();
        return noValidNextCity;
    }

    //updates the pheromones
    public void updatePheromonesAS()
    {
        double increaseFactor = 0;

        evaporatePheromones();

        for (int k = 0; k < ants.Count; k++)
        {
            increaseFactor = (1.0 / ants[k].getTourLength()) * pheromoneIncreaseFactorParameter;
            depositPheromones(k, increaseFactor);
        }

        finishIteration();
    }

    /*#########################################################################
     *               ANT COLONY SYSTEM (ACS) INTERACTION
     *#########################################################################
     */

    // moves all ants one city ahead. returns false, if no city is available
    private bool moveAntsACS(int currentCityAmount)
    {
        for (int k = 0; k < ants.Count; k++)
        {
            int nextCityIndex = acsDecisionRule(currentCityAmount, k);
            if (nextCityIndex == noValidNextCity)
                return false;

            ants[k].addCityToTour(nextCityIndex);
            ants[k].setCityVisited(nextCityIndex);

            // local update for pheromones
            localPheromoneUpdateACS(k, currentCityAmount);
        }

        return true;
    }

    // the core of the acs algorithm: what city should the  ant select next
    private int acsDecisionRule(int currCityIndex, int antIndex)
    {
        double q = random.NextDouble();

        calculateProbs(currCityIndex, antIndex);

        if (q <= acsQ0)
        {
            return cities[bestProbIndex].getId();
        }
        else
        {
            return explorationDecision();
        }
    }

    //updates the pheromones for ACS
    public void globalPheromoneUpdateACS()
    {
        int bestAntIndex = findBestAnt().getID();

        evaporatePheromones();

        double increaseFactor = (1.0 / ants[bestAntIndex].getTourLength()) * q;

        depositPheromones(bestAntIndex, increaseFactor);

        finishIteration();
    }

    // the local pheromone update is done after each step (each ant one city step)
    private void localPheromoneUpdateACS(int antIndex, int currentCityAmount)
    {
        int prevCity = ants[antIndex].getCityOfTour(currentCityAmount - 1);
        int currentCity = ants[antIndex].getCityOfTour(currentCityAmount);

        pheromones.setPheromone(prevCity, currentCity, ((1.0f - q) * pheromones.getPheromone(prevCity, currentCity)) + (q * tau0));
        pheromones.setPheromone(currentCity, prevCity, pheromones.getPheromone(prevCity, currentCity));
        choiceInfo.setChoice(prevCity, currentCity, Math.Pow(pheromones.getPheromone(prevCity, currentCity), alpha) *
                                   Math.Pow((1.0 / distances.getDistance(prevCity, currentCity)), beta));
        choiceInfo.setChoice(currentCity, prevCity, choiceInfo.getChoice(prevCity, currentCity));
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

    //the init step of the ant update
    private void initAntUpdate()
    {
        bool[] placed = new bool[cities.Count];

        for (int k = 0; k < ants.Count; k++)
        {
            ants[k].clearTour();
            ants[k].unvisitAllCities();
        }

        for (int k = 0; k < ants.Count; k++)
        {
            int r = random.Next(0, cities.Count);
            while (placed[r] && !(numOfAnts > cities.Count))
                r = random.Next(0, cities.Count);
            placed[r] = true;
            ants[k].addCityToTour(cities[r].getId());
            ants[k].setCityVisited(cities[r].getId());
        }
    }

    // simply adds the first city to the end of a tour
    private void completeTours()
    {
        for (int k = 0; k < ants.Count; k++)
        {
            ants[k].addCityToTour(ants[k].getCityOfTour(0));
            ants[k].calculateTourLength();
        }
        tourComplete = true;
    }

    //init the ants with random tours
    private void initAnts(bool randomPlacement)
    {
        ants = new List<Ant>();
        bool[] placed = new bool[cities.Count];

        for (int i = 0; i < numOfAnts; i++)
        {
            if (!randomPlacement)
            {
                ants.Add(new Ant(i, cities.Count, 0, distances));
            }
            else
            {
                int r = random.Next(0, cities.Count);
                while (placed[r] && !(numOfAnts > cities.Count))
                    r = random.Next(0, cities.Count);
                ants.Add(new Ant(i, cities.Count, r, distances));
                placed[r] = true;
            }

        }
    }

    //init the pheromones
    private void initPheromones()
    {
        pheromones = new Pheromones(cities.Count, pheromoneTrailInitialValue);
        pheromones.init();
    }

    // evaporation of pheromones
    private void evaporatePheromones()
    {
        double decreaseFactor = 1.0 - q;

        for (int i = 0; i < cities.Count; i++)
        {
            for (int j = i + 1; j < cities.Count; j++)
            {
                pheromones.decreasePheromoneAS(i, j, decreaseFactor);
                pheromones.decreasePheromoneAS(j, i, decreaseFactor);
            }
        }
    }

    // evaporation of pheromones
    private void evaporatePheromones(int antIndex)
    {
        double decreaseFactor = 1.0 - q;

        for (int i = 0; i < cities.Count; i++)
        {
            int j = ants[antIndex].getCityOfTour(i);
            int l = ants[antIndex].getCityOfTour(i + 1);

            pheromones.decreasePheromoneAS(j, l, decreaseFactor);
            pheromones.decreasePheromoneAS(l, j, decreaseFactor);
        }
    }

    // deposit procedure of pheromones
    private void depositPheromones(int antIndex, double increaseFactor)
    {
        for (int i = 0; i < cities.Count; i++)
        {
            int j = ants[antIndex].getCityOfTour(i);
            int l = ants[antIndex].getCityOfTour(i + 1);

            pheromones.increasePheromoneAS(j, l, increaseFactor);
            pheromones.increasePheromoneAS(l, j, increaseFactor);
        }
    }

    // deposit procedure of pheromones
    private void finishIteration()
    {
        choiceInfo.updateChoiceInfo(pheromones, distances, alpha, beta);
        tourComplete = false;
    }

    public List<Ant> getAnts()
    {
        return ants;
    }

    public Pheromones getPheromones()
    {
        return pheromones;
    }

    public Distances getDistances()
    {
        return distances;
    }
}
