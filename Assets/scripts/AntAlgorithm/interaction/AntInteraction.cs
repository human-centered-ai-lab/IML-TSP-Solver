/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2017
 *****************************************************/

/* AntInteraction represents the interaction with Ants */

using System;
using System.Collections.Generic;
using System.Diagnostics;

public class AntInteraction
{
    private static int noValidNextCity = -1;
    private string errorMessage;

    private int alpha;
    private int beta;
    private double rho;
    private double q;
    private double acsQ0;

    private int numOfAnts;
    private List<Ant> ants;
    private List<City> cities;

    private Pheromones pheromones;
    private Distances distances;
    private ChoiceInfo choiceInfo;
    private int startCity;

    string str = "";

    //helper
    private double[] selectionProbability;
    private double sumProbabilities;
    private int bestProbIndex;
    private double[] cumulativeProbs;
    double[] probs;

    //helper flags
    private bool tourComplete = false;

    private Random random = new Random();


    public AntInteraction(int alpha, int beta, double rho, double q, int numOfAnts, List<City> cities, int firstCity, double acsQ0)
    {
        this.cities = cities;
        this.alpha = alpha;
        this.beta = beta;
        this.rho = rho;
        this.q = q;
        this.numOfAnts = numOfAnts;
        this.startCity = firstCity;
        this.acsQ0 = acsQ0;

        distances = new Distances(cities);
        initAnts();
        pheromones = new Pheromones(cities.Count);
        choiceInfo = new ChoiceInfo(cities.Count);
        choiceInfo.updateChoiceInfo(pheromones, distances, alpha, beta);

        selectionProbability = new double[cities.Count];
        cumulativeProbs = new double[selectionProbability.Length + 1];
        probs = new double[cities.Count];

        Debug.WriteLine("Choices: " + choiceInfo.ToString);
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
        initAntUpdate();

        for (int i = 1; i < cities.Count; i++)
        {
            if (!moveAnts(i))
            {
                Debug.WriteLine("No valid next city!" + errorMessage);
            }
        }

        completeTours();
    }

    //the init step of the ant update
    private void initAntUpdate()
    {
        for (int k = 0; k < ants.Count; k++)
        {
            ants[k].clearTour();
            ants[k].unvisitAllCities();
        }

        for (int k = 0; k < ants.Count; k++)
        {
            //int start = startCity;//Random.Range(0, cities.Count);
            ants[k].addCityToTour(cities[startCity].getId());
            ants[k].setCityVisited(cities[startCity].getId());
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

    // updates an ant stepwise every city
    public bool updateAntsStepwise(int citiesSoFar)
    {
        if (!tourComplete)
        {
            if (citiesSoFar == 1)
                initAntUpdate();
            else
            {
                if (!moveAnts(citiesSoFar - 1))
                {
                    completeTours();
                }
            }
        }
        return tourComplete;
    }

    // moves all ants one city ahead. returns false, if no city is available
    private bool moveAnts(int currentCityPos)
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
        //errorMessage = "Error: p=" + p + " ant=" + antIndex + " city=" + currCityIndex + " probabilities:" + str;
        return noValidNextCity;
    }

    private int acsDecisionRule(int currCityIndex, int antIndex)
    {
        calculateProbs(currCityIndex, antIndex);
        double q = random.NextDouble();
        // TODO: always with alpha = 1;
        if (q <= acsQ0)
        {
            return cities[bestProbIndex].getId();
        }
        else
        {
            return explorationDecision();
        }
    }

    // the core of the ant algorithm: what city should the  ant select next
    private int asDecisionRule(int currCityIndex, int antIndex)
    {
        calculateProbs(currCityIndex, antIndex);
        return explorationDecision();
    }

    //updates the pheromones for ACS
    public void globalPheromoneUpdate()
    {
        double decreaseFactor = 1.0 - rho;
        double increaseFactor = 0;
        int bestAntIndex = findBestAntIndex();

        for (int i = 0; i < cities.Count; i++)
        {
            for (int j = i + 1; j < cities.Count; j++)
            {
                pheromones.decreasePheromone(i, j, decreaseFactor);
                pheromones.decreasePheromone(j, i, decreaseFactor);
            }
        }

        increaseFactor = (1.0 / ants[bestAntIndex].getTourLength()) * q;

        for (int i = 0; i < cities.Count; i++)
        {
            int j = ants[bestAntIndex].getCityOfTour(i);
            int l = ants[bestAntIndex].getCityOfTour(i + 1);

            pheromones.increasePheromone(j, l, increaseFactor);
            pheromones.increasePheromone(l, j, increaseFactor);
        }

        choiceInfo.updateChoiceInfo(pheromones, distances, alpha, beta);
        tourComplete = false;

    }
    //updates the pheromones
    public void updatePheromones()
    {
        double decreaseFactor = 1.0 - rho;
        double increaseFactor = 0;

        for (int i = 0; i < cities.Count; i++)
        {
            for (int j = i + 1; j < cities.Count; j++)
            {
                pheromones.decreasePheromone(i, j, decreaseFactor);
                pheromones.decreasePheromone(j, i, decreaseFactor);
            }
        }
        for (int k = 0; k < ants.Count; k++)
        {
            increaseFactor = (1.0 / ants[k].getTourLength()) * q;

            for (int i = 0; i < cities.Count; i++)
            {
                int j = ants[k].getCityOfTour(i);
                int l = ants[k].getCityOfTour(i + 1);

                pheromones.increasePheromone(j, l, increaseFactor);
                pheromones.increasePheromone(l, j, increaseFactor);
            }
        }

        choiceInfo.updateChoiceInfo(pheromones, distances, alpha, beta);
        tourComplete = false;

        //Debug.Log("Choices: " + choiceInfo.ToString);
        //Debug.Log("UPDATE PHEROMONE" + pheromones.ToString);
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

    // finds the ant index with the best tour
    public int findBestAntIndex()
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
        return minTourLengthIndex;
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

