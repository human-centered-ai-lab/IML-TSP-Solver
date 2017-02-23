/* Distances is a wrapper class for distances bewtween 2 2d points(cities)
 * PROGRAMMED BY Andrej Müller (andrej.mueller@student.tugraz.at), student of TU Graz - University of Technology
 * */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distances
{ 
    // default value used for initialization
    public static double initPheromoneValue = 0.01;

    // The distance matrix: x is the city A and y is the city B
    private double[][] distances;

    public Distances(List<City2D> cities)
    {
        //build the distance matix
        MakeGraphDistances(cities);
    }

    // Calculate the initial distances between cities in array order and return it as an array
    private void MakeGraphDistances(List<City2D> cities)
    {
        distances = new double[cities.Count][];

        for (int i = 0; i < cities.Count; i++)
            distances[i] = new double[cities.Count];

        for (int i = 0; i < cities.Count; i++)
            for (int j = 0; j < cities.Count; j++)
            {
                // distance matrix from cityA to cityB
                double distance = calculateCityDistance(cities[i], cities[j]);
                distances[i][j] = distance;
                distances[j][i] = distance;
            }
    }

    // Calculate the the vpoint distance between 2 cities with 2D coordinates
    public static double calculateCityDistance(City2D cityA, City2D cityB)
    {
        return (Math.Pow(cityA.getXPosition() - cityB.getXPosition(), 2) + Math.Pow(cityA.getYPosition() - cityB.getYPosition(), 2));
    }

    // returns the distance between two cities
    public double getDistance(int cityAId, int cityBId)
    {
        return distances[cityAId][cityBId];
    }

}
