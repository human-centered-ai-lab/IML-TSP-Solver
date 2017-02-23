/* THE SIMPLE ACO algorithm by Dorigo et. al. solving the TSP Problem 
 * The code base is inspired by https://msdn.microsoft.com/en-us/magazine/hh781027.aspx (visited on 20.02.2017)
 * PROGRAMMED BY Andrej Müller (andrej.mueller@student.tugraz.at), student of TU Graz - University of Technology
 * */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntAlgortitmSimple : MonoBehaviour {

    //variables for the ACO basic algorithm
    static int alpha = 3;
    static int beta = 2;
    static double rho = 0.01;
    static double Q = 2.0;

    private List<Ant> ants;
    private List<City2D> cities;

    public int numOfAnts;
    public int numOfLoops;
    public GameObject cityGameObject;

    // helper
    private static int cityId = 0;

    // initialize ants, routes
    public void initializationPhase()
    {
        
    }
    // Use this for initialization
    void Start () {

        cities = new List<City2D>();

        cities.Add(new City2D(2, 4, cityId++, "Vienna", cityGameObject));
        cities.Add(new City2D(1, 9, cityId++, "Graz", cityGameObject));
        cities.Add(new City2D(3, 8, cityId++, "Klagenfurt", cityGameObject));
        cities.Add(new City2D(9, 1, cityId++, "Innsbruck", cityGameObject));
        /*cities.Add(new City2D(5, 2, cityId++));
        cities.Add(new City2D(2, 5, cityId++));
        cities.Add(new City2D(9, 6, cityId++));
        cities.Add(new City2D(8, 1, cityId++));
        cities.Add(new City2D(7, 2, cityId++));
        cities.Add(new City2D(6, 1, cityId++));
        cities.Add(new City2D(4, 0, cityId++));
        */

        initAnts();

        try
        {
            Debug.Log("#########################Begin Ant Colony Optimization########################");

            Debug.Log("Calculate distances...");
            Distances distances = new Distances(cities);

            Debug.Log("Calculate initial best tour...");
            Tour bestTour = calculateBestTour();
            double bestLength = bestTour.getTourLength();

            Debug.Log("Init pheromones...");
            Pheromones pheromones = new Pheromones(cities.Count);

            int time = 0;
            while (time < numOfLoops)
            {
                Debug.Log("_________________LOOP"+time+"____________________");
                Debug.Log("Update ANTS...");
                updateAnts(ants, pheromones, distances);
                Debug.Log("Update PHEROMONES...");
                UpdatePheromones(pheromones, distances);

                Debug.Log("Calculate best tour...");
                Tour currBestTour = calculateBestTour();
                double currBestLength = currBestTour.getTourLength();
                if (currBestLength < bestLength)
                {
                    Debug.Log("BEST TOUR SO FAR!!!");
                    bestLength = currBestLength;
                    bestTour = currBestTour;
                }
                time ++;
            }

            Debug.Log("Best trail found:");
            Display(bestTour);
            Debug.Log("Length of best trail found: " +
            bestLength.ToString("F1"));
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private void Display(Tour tour)
    {
        for (int i = 0; i < cities.Count; i++)
        {
            Debug.Log(i + ": ID = " + tour.getCity(i).getId() + " \n");
        }
    }

    private Tour calculateBestTour()
    {
        double bestLength = ants[0].getTour().getTourLength();
        int bestLengthIndex = 0;

        for (int i = 1; i < ants.Count; i++)
        {
            double len = ants[i].getTour().getTourLength(); ;
            if (len < bestLength)
            {
                bestLength = len;
                bestLengthIndex = i;
            }
        }

        Debug.Log("BEST TOUR: " + "ANT "+ bestLengthIndex + " LENGTH: " + bestLength);

        return ants[bestLengthIndex].getTour();
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void initAnts()
    {
        ants = new List<Ant>();

        for (int i = 0; i < numOfAnts; i++)
        {
            ants.Add(new Ant(cities));
           // ants[i].printCities();
        }
    }

    private void updateAnts(List<Ant> ants, Pheromones pheromones, Distances distances)
    {
        for (int i = 0; i < ants.Count; i++)
        {
            int start = UnityEngine.Random.Range(0, cities.Count);
            Debug.Log("::::::::::: ANT " + i + " starts at " + start + " ::::::::::::::::::::");

            Tour newTour = BuildTrail(i, start, pheromones, distances);
            ants[i].setTour(newTour);
            Debug.Log("The tour length of ANT " + i + " is " + newTour.getTourLength() + "!");

        }
    }

    private Tour BuildTrail(int k, int start, Pheromones pheromones, Distances distances)
    {
        List<City2D> tempCityList = new List<City2D>();
        bool[] visited = new bool[cities.Count];
        tempCityList.Add(cities[start]);
        visited[start] = true;

        for (int i = 0; i < cities.Count - 1; i++)
        {
            City2D cityA = tempCityList[i];
            City2D next = NextCity(k, cityA, visited, pheromones, distances);

            Debug.Log("City "+ cityA.getId() + " selected. NEXT city is: " + next.getId());

            tempCityList.Add(next);
            visited[next.getId()] = true;
        }
        return new Tour(tempCityList);
    }

    private City2D NextCity(int k, City2D cityA, bool[] visited, Pheromones pheromones, Distances distances)
    {
        double[] probabilities = MoveProbabilities(k, cityA, visited, pheromones, distances);
        double[] cumulativeProbs = new double[probabilities.Length + 1];

        // calculate the comulative probabilities
        for (int i = 0; i < probabilities.Length; i++)
            cumulativeProbs[i + 1] = cumulativeProbs[i] + probabilities[i];

        // calculate a random threshold
        double p = UnityEngine.Random.Range(0, 1);

        for (int i = 0; i < cumulativeProbs.Length - 1; i++)
            if (p >= cumulativeProbs[i] && p < cumulativeProbs[i + 1])
                return cities[i];
        throw new Exception("Failure to return valid city in NextCity");
    }

    //calculate the probabilities for moving to a certain city
    private double[] MoveProbabilities(int k, City2D cityA, bool[] visited, Pheromones pheromones, Distances distances)
    {
        double[] taueta = new double[cities.Count];
        double sum = 0.0;
        for (int i = 0; i < taueta.Length; i++)
        {
            if (cities[i].getId() == cityA.getId())
            {
                taueta[i] = 0.0;
            }
            else if (visited[i] == true)
                taueta[i] = 0.0; 
            else
            {
                taueta[i] = Math.Pow(pheromones.getPheromone(cityA.getId(), i), alpha) *
                Math.Pow((1.0 / distances.getDistance(cityA.getId(), i)), beta);

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
          //  Debug.Log("MOVE PROB: From city "+ cityA.getId() +" to city" + i + " is " + probs[i] );

        }
        return probs;
    }

    private void UpdatePheromones(Pheromones pheromones, Distances dists)
    {
        for (int i = 0; i < pheromones.getCount(); i++)
        {
            for (int j = i + 1; j < pheromones.getCount(); j++)
            {
                for (int k = 0; k < ants.Count; k++)
                {
                    double length = ants[k].getTour().getTourLength(); 
                    // length of ant k trail
                    double decrease = (1.0 - rho) * pheromones.getPheromone(i, j);
                    double increase = 0.0;
                    if (EdgeInTrail(ants[k].getTour().getCity(i), ants[k].getTour().getCity(j), ants[k]) == true)
                    {
                        increase = (Q / length);
                    }

                    pheromones.setPheromone(i, j, decrease + increase);

                    if (pheromones.getPheromone(i, j) < 0.0001)
                    {
                        pheromones.setPheromone(i, j, 0.0001);
                    }
                    else if (pheromones.getPheromone(i, j) > 100000.0)
                    {
                        pheromones.setPheromone(i, j, 100000.0);
                    }

                    pheromones.setPheromone(j, i,  pheromones.getPheromone(i, j));
                }
            }
        }
    }
    private bool EdgeInTrail(City2D cityX, City2D cityY, Ant ant)
    {
        // are cityX and cityY adjacent to each other in trail[]?
        int lastIndex = cities.Count - 1;
        int idx = IndexOfTarget(ant, cityX);

        if (idx == 0 && ant.getTour().getCity(1) == cityY)
        {
            return true;
        }
        else if (idx == 0 && ant.getTour().getCity(lastIndex) == cityY)
        {
            return true;
        }
        else if (idx == 0)
        {
            return false;
        }
        else if (idx == lastIndex && ant.getTour().getCity(lastIndex - 1) == cityY)
        {
            return true;
        }
        else if (idx == lastIndex && ant.getTour().getCity(0) == cityY)
        {
            return true;
        }
        else if (idx == lastIndex)
        {
            return false;
        }
        else if (ant.getTour().getCity(idx - 1) == cityY)
        {
            return true;
        }
        else if (ant.getTour().getCity(idx + 1) == cityY)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private int IndexOfTarget(Ant ant, City2D target)
    {
        // helper for RandomTrail
        for (int i = 0; i < cities.Count; i++)
        {
            if (ant.getTour().getCity(i).getId() == target.getId())
            {
                return i;
            }
        }
        throw new Exception("Target not found in IndexOfTarget");
    }

}
