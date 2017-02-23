/* Tour is a wrapper class for a certain tour of points(cities)
 * PROGRAMMED BY Andrej Müller (andrej.mueller@student.tugraz.at), student of TU Graz - University of Technology
 * */
using System.Collections.Generic;
using UnityEngine;

public class Tour
{
    // holds a city list 
    private List<City2D> cities;
    // the length of a tour
    private double tourLength = 0; 

    public Tour(List<City2D> cities)
    {
        this.cities = cities;
        calculateTourLength();
    }

    public Tour(int[] cityInd, List<City2D> cities)
    {
        this.cities = new List<City2D>();
        for(int i = 0; i < cities.Count; i++)
        {
            this.cities.Add(cities[cityInd[i]]);
        }

        calculateTourLength();
    }

    public void addCityToTour(City2D city)
    {
        cities.Add(city);
        calculateTourLength();
    }

    public void calculateTourLength()
    {
        for (int i = 0; i < (cities.Count - 1); i++)
            tourLength = tourLength + Distances.calculateCityDistance(cities[i], cities[i + 1]);
        tourLength += Distances.calculateCityDistance(cities[cities.Count - 1], cities[0]);
    }

    public void printTour()
    {
        for(int i = 0; i < cities.Count; i++)
            Debug.Log("City with ID"+ cities[i].getId()+" and i = " + i +": " + cities[i].getXPosition() + " - " + cities[i].getYPosition());
    }

    public double getTourLength()
    {
        return tourLength;
    }

    public City2D getCity(int i)
    {
        return cities[i];
    }

    public int getAmountOfCities()
    {
        return cities.Count;
    }
}