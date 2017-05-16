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

    private double xi;
    private double tau0;

    private int numOfAnts;
    private List<Ant> ants;
    private List<City> cities;

    private Pheromones pheromones;
    private Distances distances;
    private ChoiceInfo choiceInfo;
    private int startCity;
    private AntAlgorithms.Mode mode;
    private double pheromoneTrailInitialValue;

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

    public AntInteraction(AntAlgorithms.Mode mode, int alpha, int beta, double rho, double q, int numOfAnts, List<City> cities, int firstCity, double pheromoneTrailInitialValue, double acsQ0, double xi, double tau0)
    {
        initAntInteractionForAS(mode, alpha, beta, rho, q, numOfAnts, cities, firstCity, pheromoneTrailInitialValue);
        this.acsQ0 = acsQ0;
        this.xi = xi;
        this.tau0 = tau0;
    }

    public AntInteraction(AntAlgorithms.Mode mode, int alpha, int beta, double rho, double q, int numOfAnts, List<City> cities, int firstCity, double pheromoneTrailInitialValue)
    {
        initAntInteractionForAS(mode, alpha, beta, rho, q, numOfAnts, cities, firstCity, pheromoneTrailInitialValue);
    }

    //update the tours of ants by considering the pheromones
    public void updateAnts()
    {
        initAntUpdate();

        for (int i = 1; i < cities.Count; i++)
        {
            switch (mode)
            {
                case AntAlgorithms.Mode.antSystem:
                    if (!moveAntsAS(i))
                    {
                        Debug.WriteLine("No valid next city!" + errorMessage);
                    }
                    break;
                case AntAlgorithms.Mode.antColonySystem:
                    if (!moveAntsACS(i))
                    {
                        Debug.WriteLine("No valid next city!" + errorMessage);
                    }
                    break;
            }

            completeTours();
        }
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
                switch (mode)
                {
                    case AntAlgorithms.Mode.antSystem:
                        if (!moveAntsAS(citiesSoFar - 1))
                        {
                            completeTours();
                        }
                        break;
                    case AntAlgorithms.Mode.antColonySystem:
                        if (!moveAntsACS(citiesSoFar - 1))
                        {
                            completeTours();
                        }
                        break;
                }
            }
        }
        return tourComplete;
    }

    /*###################################
     *   ANT SYSTEM(AS) INTERACTION
     *###################################
     **/

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

    //updates the pheromones
    public void updatePheromonesAS()
    {
        double decreaseFactor = 1.0 - rho;
        double increaseFactor = 0;

        for (int i = 0; i < cities.Count; i++)
        {
            for (int j = i + 1; j < cities.Count; j++)
            {
                pheromones.decreasePheromoneAS(i, j, decreaseFactor);
                pheromones.decreasePheromoneAS(j, i, decreaseFactor);
            }
        }
        for (int k = 0; k < ants.Count; k++)
        {
            increaseFactor = (1.0 / ants[k].getTourLength()) * q;

            for (int i = 0; i < cities.Count; i++)
            {
                int j = ants[k].getCityOfTour(i);
                int l = ants[k].getCityOfTour(i + 1);

                pheromones.increasePheromoneAS(j, l, increaseFactor);
                pheromones.increasePheromoneAS(l, j, increaseFactor);
            }
        }

        choiceInfo.updateChoiceInfo(pheromones, distances, alpha, beta);
        tourComplete = false;

        //Debug.Log("Choices: " + choiceInfo.ToString);
        //Debug.Log("UPDATE PHEROMONE" + pheromones.ToString);
    }

    /*########################################
    *   ANT COLONY SYSTEM (ACS) INTERACTION
    *#########################################
     **/

    // moves all ants one city ahead. returns false, if no city is available
    private bool moveAntsACS(int currentCityPos)
    {
        for (int k = 0; k < ants.Count; k++)
        {
            int nextCityIndex = acsDecisionRule(currentCityPos, k);
            if (nextCityIndex == noValidNextCity)
                return false;

            ants[k].addCityToTour(nextCityIndex);
            ants[k].setCityVisited(nextCityIndex);

            localPheromoneUpdateACS(k, currentCityPos);
        }
        return true;
    }

    // the core of the acs algorithm: what city should the  ant select next
    private int acsDecisionRule(int currCityIndex, int antIndex)
    {
        calculateProbs(currCityIndex, antIndex);
        double q = random.NextDouble();
        // TODO: calculate prob always with alpha = 1;
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
        double decreaseFactor = 1.0 - rho;
        double increaseFactor = 0;
        int bestAntIndex = findBestAntIndex();

        for (int i = 0; i < cities.Count; i++)
        {
            for (int j = i + 1; j < cities.Count; j++)
            {
                pheromones.decreasePheromoneAS(i, j, decreaseFactor);
                pheromones.decreasePheromoneAS(j, i, decreaseFactor);
            }
        }

        increaseFactor = (1.0 / ants[bestAntIndex].getTourLength()) * q;

        for (int i = 0; i < cities.Count; i++)
        {
            int j = ants[bestAntIndex].getCityOfTour(i);
            int l = ants[bestAntIndex].getCityOfTour(i + 1);

            pheromones.increasePheromoneAS(j, l, increaseFactor);
            pheromones.increasePheromoneAS(l, j, increaseFactor);
        }

        choiceInfo.updateChoiceInfo(pheromones, distances, alpha, beta);
        tourComplete = false;

    }
    
    private void localPheromoneUpdateACS(int antIndex, int currentCityPos)
    {
        int prevCity = ants[antIndex].getCityOfTour(currentCityPos - 1);
        int currentCity = ants[antIndex].getCityOfTour(currentCityPos);
        pheromones.setPheromone(prevCity, currentCity, ((1.0 - xi) * pheromones.getPheromone(prevCity, currentCity)) + (xi * tau0));
        pheromones.setPheromone(currentCity, prevCity, pheromones.getPheromone(prevCity, currentCity));
        choiceInfo.setChoice(prevCity, currentCity, pheromones.getPheromone(prevCity, currentCity) * Math.Pow(1.0 / distances.getDistance(prevCity, currentCity), beta));
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

    //init the ants with random tours
    private void initAnts()
    {
        ants = new List<Ant>();
        for (int i = 0; i < numOfAnts; i++)
        {
            ants.Add(new Ant(i, cities.Count, 0, distances));
        }
    }

    //init the pheromones
    private void initPheromones()
    {
        pheromones = new Pheromones(cities.Count, 10.0);
        pheromones.init();
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

    private void initAntInteractionForAS(AntAlgorithms.Mode mode, int alpha, int beta, double rho, double q, int numOfAnts, List<City> cities, int firstCity, double pheromoneTrailInitialValue)
    {
        this.cities = cities;
        this.alpha = alpha;
        this.beta = beta;
        this.rho = rho;
        this.q = q;
        this.numOfAnts = numOfAnts;
        this.startCity = firstCity;
        this.mode = mode;
        this.pheromoneTrailInitialValue = pheromoneTrailInitialValue;
        //calculates distances
        distances = new Distances(cities);

        initAnts();
        initPheromones();

        choiceInfo = new ChoiceInfo(cities.Count);
        choiceInfo.updateChoiceInfo(pheromones, distances, alpha, beta);

        selectionProbability = new double[cities.Count];
        cumulativeProbs = new double[selectionProbability.Length + 1];
        probs = new double[cities.Count];
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

