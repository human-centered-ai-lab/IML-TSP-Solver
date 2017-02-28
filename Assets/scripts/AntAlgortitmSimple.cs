/* THE SIMPLE ACO algorithm by Dorigo et. al. solving the TSP Problem 
 * The code base is inspired by https://msdn.microsoft.com/en-us/magazine/hh781027.aspx (visited on 20.02.2017)
 * PROGRAMMED BY Andrej Müller (andrej.mueller@student.tugraz.at), student of TU Graz - University of Technology
 * */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AntAlgortitmSimple : MonoBehaviour
{

    //variables for the ACO basic algorithm

    // influence of pheromone on direction
    private static int alpha = 3;
    // influence of adjacent node distance
    private static int beta = 2;

    // pheromone decrease factor
    private static double rho = 0.01;
    // pheromone increase factor
    private static double Q = 2.0;

    private List<Ant> ants;
    private List<City2D> cities;

    public int numOfAnts;
    public int numOfLoops;
    public GameObject cityGameObject;

    // helper
    private static int cityId = 0;
    private Tour bestTour;
    private Pheromones pheromones;
    private Distances distances;
    private double bestLength;

    // testing
    public Button button;
    private bool clicked = false;
    int counter = 0;

    // Use this for initialization
    void Start()
    {

        //testing
        button.onClick.AddListener(buttonClickRoutine);


        cities = new List<City2D>();

        cities.Add(new City2D(2, 4, cityId++, "Vienna", cityGameObject));
        cities.Add(new City2D(1, 9, cityId++, "Graz", cityGameObject));
        cities.Add(new City2D(3, 8, cityId++, "Klagenfurt", cityGameObject));
        cities.Add(new City2D(9, 1, cityId++, "Innsbruck", cityGameObject));

        cities.Add(new City2D(5, 2, cityId++, "Innsbruck", cityGameObject));
        cities.Add(new City2D(2, 5, cityId++, "Innsbruck", cityGameObject));
        cities.Add(new City2D(9, 6, cityId++, "Innsbruck", cityGameObject));
        cities.Add(new City2D(8, 1, cityId++, "Innsbruck", cityGameObject));
        cities.Add(new City2D(7, 2, cityId++, "Innsbruck", cityGameObject));
        cities.Add(new City2D(6, 1, cityId++, "Innsbruck", cityGameObject));
        cities.Add(new City2D(4, 0, cityId++, "Innsbruck", cityGameObject));
        
        //start
        initAnts();

        try
        {
            Debug.Log("#########################Begin Ant Colony Optimization########################");

            Debug.Log("Calculate distances...");
            distances = new Distances(cities);

            Debug.Log("Calculate initial best tour...");
            bestTour = calculateBestTour();
            bestLength = bestTour.getLength();

            Debug.Log("Init pheromones...");
            pheromones = new Pheromones(cities.Count);

        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private void buttonClickRoutine()
    {
            clicked = true;
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
        double bestLength = ants[0].getTour().getLength();
        int bestLengthIndex = 0;

        for (int i = 1; i < ants.Count; i++)
        {
            double len = ants[i].getTour().getLength(); ;
            if (len < bestLength)
            {
                bestLength = len;
                bestLengthIndex = i;
            }
        }

        Debug.Log("BEST TOUR: " + "ANT " + bestLengthIndex + " LENGTH: " + bestLength);

        return ants[bestLengthIndex].getTour();
    }

    // Update is called once per frame
    void Update()
    {
        if (clicked == true)
        {

            try
            {

                Debug.Log("_________________LOOP" + counter + "____________________");
                Debug.Log("Update ANTS...");
                updateAnts(ants);
                Debug.Log("Update PHEROMONES...");
                UpdatePheromones();

                Debug.Log("Calculate best tour...");
                Tour currBestTour = calculateBestTour();
                double currBestLength = currBestTour.getLength();
                if (currBestLength < bestLength)
                {
                    Debug.Log("BEST TOUR SO FAR!!!");
                    bestLength = currBestLength;
                    bestTour = currBestTour;
                }


                Debug.Log("Best trail found:");
                Display(bestTour);
                Debug.Log("Length of best trail found: " +
                bestLength.ToString("F1"));
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                Debug.Log(ex.StackTrace);
                Debug.Log(ex.HelpLink);
            }
            counter++;
            clicked = false;
        }

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

    private void updateAnts(List<Ant> ants)
    {
        for (int i = 0; i < ants.Count; i++)
        {
            int start = UnityEngine.Random.Range(0, cities.Count);
            Debug.Log("::::::::::: ANT " + i + " starts at " + start + " ::::::::::::::::::::");

            Tour newTour = BuildTrail(i, start);
            ants[i].setTour(newTour);
            Debug.Log("The tour length of ANT " + i + " is " + newTour.getLength() + "!");

        }
    }

    private Tour BuildTrail(int k, int start)
    {
        List<int> tempCityIndexList = new List<int>();
        bool[] visited = new bool[cities.Count];
        tempCityIndexList.Add(cities[start].getId());
        visited[start] = true;

        for (int i = 0; i < cities.Count - 1; i++)
        {
            int cityAIndex = tempCityIndexList[i];
            int nextCityIndex = NextCity(k, cityAIndex, visited);

            Debug.Log("City " + cityAIndex + " selected. NEXT city is: " + nextCityIndex);

            tempCityIndexList.Add(nextCityIndex);
            visited[nextCityIndex] = true;
        }
        return new Tour(tempCityIndexList, cities);
    }

    private int NextCity(int k, int cityAIndex, bool[] visited)
    {
        double[] probabilities = MoveProbabilities(k, cityAIndex, visited);
        double[] cumulativeProbs = new double[probabilities.Length + 1];

        // calculate the comulative probabilities
        for (int i = 0; i < probabilities.Length; i++)
            cumulativeProbs[i + 1] = cumulativeProbs[i] + probabilities[i];

        // calculate a random threshold
        double p = UnityEngine.Random.Range(0, 1);

        for (int i = 0; i < cumulativeProbs.Length - 1; i++)
            if (p >= cumulativeProbs[i] && p < cumulativeProbs[i + 1])
                return cities[i].getId();
        throw new Exception("Failure to return valid city in NextCity");
    }

    //calculate the probabilities for moving to a certain city
    private double[] MoveProbabilities(int k, int cityAIndex, bool[] visited)
    {
        double[] taueta = new double[cities.Count];
        double sum = 0.0;
        for (int i = 0; i < taueta.Length; i++)
        {
            if (cities[i].getId() == cityAIndex)
            {
                taueta[i] = 0.0;
            }
            else if (visited[i] == true)
                taueta[i] = 0.0;
            else
            {
                taueta[i] = Math.Pow(pheromones.getPheromone(cityAIndex, i), alpha) *
                Math.Pow((1.0 / distances.getDistance(cityAIndex, i)), beta);

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

    private void UpdatePheromones()
    {
        for (int i = 0; i < pheromones.getCount(); i++)
        {
            for (int j = i + 1; j < pheromones.getCount(); j++)
            {
                for (int k = 0; k < ants.Count; k++)
                {
                    double length = ants[k].getTour().getLength();
                    double decrease = (1.0 - rho) * pheromones.getPheromone(i, j);
                    double increase = 0.0;

                    // if the current edge is visited by the current ant
                    if (ants[k].getTour().isEdge(i,j))
                    {
                        Debug.Log("Ant [" + k + "] There is an edge between City " + i + " and City " + j + "!");
                        increase = (Q / length);
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
   
    



}
