/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2017
 *****************************************************/

/* AntInteraction represents the interaction with Ants */

using System;
using System.Collections.Generic;

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
    private List<City> cities;

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

    private Random random = new Random();

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
        Distances = new Distances(cities);

        // calculations for variable parameters
        if (tau0 == -1)
            this.tau0 = 1.0 / (cities.Count * Distances.CalculateNNHeuristic());
        if (pheromoneTrailInitialValue == -1)
            this.pheromoneTrailInitialValue = this.tau0;

        // init ants with random start point
        InitAnts(true);
        InitPheromones();

        choiceInfo = new ChoiceInfo(cities.Count);
        choiceInfo.UpdateChoiceInfo(Pheromones, Distances, alpha, beta);

        selectionProbability = new double[cities.Count];
        cumulativeProbs = new double[selectionProbability.Length + 1];
        probs = new double[cities.Count];
    }

    //update the tours of ants by considering the pheromones
    public void UpdateAnts()
    {
        InitAntUpdate();
        bool moveValid = true;

        for (int i = 1; i < cities.Count; i++)
        {
            switch (mode)
            {
                case AntAlgorithms.Mode.AntSystem:
                    moveValid = MoveAntsAs(i);
                    break;
                case AntAlgorithms.Mode.AntColonySystem:
                    moveValid = MoveAntsAcs(i);
                    break;
            }

            if (!moveValid)
                throw new Exception("No valid next city!");
        }
        CompleteTours();
    }

    // updates an ant stepwise every city
    public bool UpdateAntsStepwise(int citiesSoFar)
    {
        bool lastCity = false;

        if (!tourComplete)
        {
            if (citiesSoFar == 1)
                InitAntUpdate();
            else
            {
                switch (mode)
                {
                    case AntAlgorithms.Mode.AntSystem:
                        lastCity = !MoveAntsAs(citiesSoFar - 1);
                        break;
                    case AntAlgorithms.Mode.AntColonySystem:
                        lastCity = !MoveAntsAcs(citiesSoFar - 1);
                        break;
                }
                if (lastCity)
                    CompleteTours();
            }
        }
        return tourComplete;
    }

    /*#########################################################################
     *                    ANT SYSTEM (AS) INTERACTION
     *#########################################################################
     */

    // moves all ants one city ahead. returns false, if no city is available
    private bool MoveAntsAs(int currentCityPos)
    {
        for (int k = 0; k < Ants.Count; k++)
        {
            int nextCityIndex = AsDecisionRule(currentCityPos, k);
            if (nextCityIndex == noValidNextCity)
            {
                return false;
            }

            Ants[k].AddCityToTour(nextCityIndex);
            Ants[k].SetCityVisited(nextCityIndex);
        }
        return true;
    }

    // the core of the as algorithm: what city should the  ant select next
    private int AsDecisionRule(int currCityIndex, int antIndex)
    {
        CalculateProbs(currCityIndex, antIndex);
        return ExplorationDecision();
    }

    //calculation of the probabilities 
    private void CalculateProbs(int currCityIndex, int antIndex)
    {
        sumProbabilities = 0.0;

        int currentCity = Ants[antIndex].GetCityOfTour(currCityIndex - 1);

        for (int i = 0; i < selectionProbability.Length; i++)
        {
            if (Ants[antIndex].IsCityVisited(i))
            {
                selectionProbability[i] = 0.0;
            }
            else
            {
                selectionProbability[i] = choiceInfo.GetChoice(currentCity, i);
                sumProbabilities += selectionProbability[i];
            }
        }

        double probTemp = 0.0;

        bestProbIndex = -1;

        for (int i = 0; i < probs.Length; i++)
        {
            probs[i] = selectionProbability[i] / sumProbabilities;
            if (probs[i] > probTemp)
            {
                bestProbIndex = i;
                probTemp = probs[i];
            }
        }
    }

    // explore new edges by adding randomness
    private int ExplorationDecision()
    {
        // calculate the cumulative probabilities
        for (int i = 0; i < selectionProbability.Length; i++)
        {
            cumulativeProbs[i + 1] = cumulativeProbs[i] + probs[i];
        }
        cumulativeProbs[cumulativeProbs.Length - 1] = 1.0f;
        double p = random.NextDouble();

        for (int i = 0; i < cumulativeProbs.Length - 1; i++)
        {
            if (p >= cumulativeProbs[i] && p <= cumulativeProbs[i + 1])
            {
                return cities[i].Id;
            }
        }
        return noValidNextCity;
    }

    //updates the pheromones
    public void UpdatePheromonesAs()
    {
        EvaporatePheromones();

        for (int k = 0; k < Ants.Count; k++)
        {
            double increaseFactor = (1.0 / Ants[k].TourLength) * pheromoneIncreaseFactorParameter;
            DepositPheromones(k, increaseFactor);
        }

        FinishIteration();
    }

    /*#########################################################################
     *               ANT COLONY SYSTEM (ACS) INTERACTION
     *#########################################################################
     */

    // moves all ants one city ahead. returns false, if no city is available
    private bool MoveAntsAcs(int currentCityAmount)
    {
        for (int k = 0; k < Ants.Count; k++)
        {
            int nextCityIndex = AcsDecisionRule(currentCityAmount, k);
            if (nextCityIndex == noValidNextCity)
            {
                return false;
            }

            Ants[k].AddCityToTour(nextCityIndex);
            Ants[k].SetCityVisited(nextCityIndex);

            // local update for pheromones
            LocalPheromoneUpdateAcs(k, currentCityAmount);
        }
        return true;
    }

    // the core of the acs algorithm: what city should the  ant select next
    private int AcsDecisionRule(int currCityIndex, int antIndex)
    {
        double q = random.NextDouble();

        CalculateProbs(currCityIndex, antIndex);

        if (q <= acsQ0)
        {
            return cities[bestProbIndex].Id;
        }
        return ExplorationDecision();
    }

    //updates the pheromones for ACS
    public void GlobalPheromoneUpdateAcs()
    {
        int bestAntIndex = FindBestAnt().Id;

        EvaporatePheromones();

        double increaseFactor = (1.0 / Ants[bestAntIndex].TourLength) * q;

        DepositPheromones(bestAntIndex, increaseFactor);

        FinishIteration();
    }

    // the local pheromone update is done after each step (each ant one city step)
    private void LocalPheromoneUpdateAcs(int antIndex, int currentCityAmount)
    {
        int prevCity = Ants[antIndex].GetCityOfTour(currentCityAmount - 1);
        int currentCity = Ants[antIndex].GetCityOfTour(currentCityAmount);

        Pheromones.SetPheromone(prevCity, currentCity, ((1.0f - q) * Pheromones.GetPheromone(prevCity, currentCity)) + (q * tau0));
        Pheromones.SetPheromone(currentCity, prevCity, Pheromones.GetPheromone(prevCity, currentCity));
        choiceInfo.SetChoice(prevCity, currentCity, Math.Pow(Pheromones.GetPheromone(prevCity, currentCity), alpha) *
                                   Math.Pow((1.0 / Distances.GetDistance(prevCity, currentCity)), beta));
        choiceInfo.SetChoice(currentCity, prevCity, choiceInfo.GetChoice(prevCity, currentCity));
    }

    // finds the ant with the best tour
    public Ant FindBestAnt()
    {
        double minTourLength = Ants[0].TourLength;
        int minTourLengthIndex = 0;

        for (int i = 1; i < Ants.Count; i++)
        {
            if (Ants[i].TourLength < minTourLength)
            {
                minTourLength = Ants[i].TourLength;
                minTourLengthIndex = i;
            }
        }
        return Ants[minTourLengthIndex];
    }

    //the init step of the ant update
    private void InitAntUpdate()
    {
        bool[] placed = new bool[cities.Count];

        foreach (Ant ant in Ants)
        {
            ant.ClearTour();
            ant.UnvisitAllCities();
        }

        foreach (Ant ant in Ants)
        {
            int r = random.Next(0, cities.Count);
            while (placed[r] && !(numOfAnts > cities.Count))
                r = random.Next(0, cities.Count);
            placed[r] = true;
            ant.AddCityToTour(cities[r].Id);
            ant.SetCityVisited(cities[r].Id);
        }
    }

    // simply adds the first city to the end of a tour
    private void CompleteTours()
    {
        foreach (Ant ant in Ants)
        {
            ant.AddCityToTour(ant.GetCityOfTour(0));
            ant.CalculateTourLength();
        }
        tourComplete = true;
    }

    //init the ants with random tours
    private void InitAnts(bool randomPlacement)
    {
        Ants = new List<Ant>();
        bool[] placed = new bool[cities.Count];

        for (int i = 0; i < numOfAnts; i++)
        {
            if (!randomPlacement)
            {
                Ants.Add(new Ant(i, cities.Count, 0, Distances));
            }
            else
            {
                int r = random.Next(0, cities.Count);
                while (placed[r] && !(numOfAnts > cities.Count))
                {
                    r = random.Next(0, cities.Count);
                }
                Ants.Add(new Ant(i, cities.Count, r, Distances));
                placed[r] = true;
            }

        }
    }

    //init the pheromones
    private void InitPheromones()
    {
        Pheromones = new Pheromones(cities.Count, pheromoneTrailInitialValue);
        Pheromones.Init();
    }

    // evaporation of pheromones
    private void EvaporatePheromones()
    {
        double decreaseFactor = 1.0 - q;

        for (int i = 0; i < cities.Count; i++)
        {
            for (int j = i + 1; j < cities.Count; j++)
            {
                Pheromones.DecreasePheromoneAs(i, j, decreaseFactor);
                Pheromones.DecreasePheromoneAs(j, i, decreaseFactor);
            }
        }
    }

    // evaporation of pheromones
    private void EvaporatePheromones(int antIndex)
    {
        double decreaseFactor = 1.0 - q;

        for (int i = 0; i < cities.Count; i++)
        {
            int j = Ants[antIndex].GetCityOfTour(i);
            int l = Ants[antIndex].GetCityOfTour(i + 1);

            Pheromones.DecreasePheromoneAs(j, l, decreaseFactor);
            Pheromones.DecreasePheromoneAs(l, j, decreaseFactor);
        }
    }

    // deposit procedure of pheromones
    private void DepositPheromones(int antIndex, double increaseFactor)
    {
        for (int i = 0; i < cities.Count; i++)
        {
            int j = Ants[antIndex].GetCityOfTour(i);
            int l = Ants[antIndex].GetCityOfTour(i + 1);

            Pheromones.IncreasePheromoneAs(j, l, increaseFactor);
            Pheromones.IncreasePheromoneAs(l, j, increaseFactor);
        }
    }

    // deposit procedure of pheromones
    private void FinishIteration()
    {
        choiceInfo.UpdateChoiceInfo(Pheromones, Distances, alpha, beta);
        tourComplete = false;
    }

    public List<Ant> Ants { get; private set; }

    public Pheromones Pheromones { get; private set; }

    public Distances Distances { get; private set; }
}
