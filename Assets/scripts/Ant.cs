/* Ant represents an ant of the ACO algorithm in 2D space.
 * PROGRAMMED BY Andrej Müller (andrej.mueller@student.tugraz.at), student of TU Graz - University of Technology
 * */
using System.Collections.Generic;
using UnityEngine;

public class Ant {

    // every ant has an own tour
    private Tour tour;

    public Ant(List<City2D> cities)
    {
        // generate a random tour after initialization
        tour = generateRandomTour(cities, 0);
    }

    // Generates a random tour by shuffeling the cities
    private Tour generateRandomTour(List<City2D> cities, int start)
    {

        List<int> cityIndices = new List<int>();

        for (int i = 0; i < cities.Count; i++)
            cityIndices.Add(i);

        //shuffle
        for (int i = 0; i < cities.Count; i++)
        {
            int random = Random.Range(i, cities.Count);
            int tmp = cityIndices[random];
            cityIndices[random] = cityIndices[i];
            cityIndices[i] = tmp;
        }

        int target = -1;

        // search for the index of the target city
        for (int i = 0; i < cityIndices.Count; i++)
        {
            if (cityIndices[i] == start)
            {
               target = i;
            }
        }

        int temp = cityIndices[0];
        cityIndices[0] = cityIndices[target];
        cityIndices[target] = temp;

        return new Tour(cityIndices, cities);
    }

    public void printCities()
    {
        Debug.Log("PrintC");
       /* Debug.Log("Ant " + this.GetHashCode() +": ");
        tour.printTour();
        Debug.Log("_______________________________");*/
    }

    public void setTour(Tour tour)
    {
        this.tour = tour;
    }

    public Tour getTour()
    {
        return tour;
    }
}
