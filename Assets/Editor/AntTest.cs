
/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2017
 *****************************************************/

/* AntAlgorithSimpleTest */

using System.Collections.Generic;
using NUnit.Framework;
using System.Reflection;
using AntAlgorithm.tools;
using UnityEngine;

[assembly: AssemblyVersion("0.1")]

[TestFixture]
public class AntTest
{
    private List<City> cities;
    private AntAlgorithms.AntAlgorithmChooser antAlgorithmChooser;
    private AntAlgorithms.AntAlgorithm antAlgorithm;
    private TSPImporter tspImporter;


    [OneTimeSetUp]
    public void Init()
    {
        //Init runs once before running test cases.
    }

    [OneTimeTearDown]
    public void CleanUp()
    {
        //CleanUp runs once after all test cases are finished.
    }

    [SetUp]
    public void SetUp()
    {
        cities = new List<City>();
        tspImporter = new TSPImporter();
    }

    [TearDown]
    public void TearDown()
    {
        //SetUp runs after all test cases
    }

    [Test]
    public void Eil51ACS()
    {
        antAlgorithmChooser = new AntAlgorithms.AntAlgorithmChooser(AntAlgorithms.Mode.AntColonySystem, 1, 2, 0.1, 10, 0.9, -1);
        antAlgorithm = antAlgorithmChooser.Algorithm;
        antAlgorithm.Cities = TSPImporter.ImportTsp("eil51.tsp");
        antAlgorithm.Init();
        for (int i = 0; i < 10000; i++)
        {
            antAlgorithm.Iteration();
        }
        antAlgorithm.PrintBestTour("Eil51ACS", 1);
        Assert.True(true);
    }

    [Test]
    public void Eil51MM()
    {
        for (int x = 0; x < 1; x++)
        {
            antAlgorithmChooser = new AntAlgorithms.AntAlgorithmChooser(AntAlgorithms.Mode.MinMaxAntSystem, 1, 2, 0.02, 51, -1, 0.05);
            antAlgorithm = antAlgorithmChooser.Algorithm;
            antAlgorithm.Cities = TSPImporter.ImportTsp("eil51.tsp");
            antAlgorithm.Init();
            for (int i = 0; i < 4000; i++)
            {
                antAlgorithm.Iteration();
            }

            antAlgorithm.PrintBestTour("Eil51MM-"+ x, 1);

        }
        Assert.True(true);
    }

    [Test]
    public void Eil51AS()
    {
        antAlgorithmChooser = new AntAlgorithms.AntAlgorithmChooser(1, 2, 0.7, 100);
        antAlgorithm = antAlgorithmChooser.Algorithm;
        antAlgorithm.Cities = TSPImporter.ImportTsp("eil51.tsp");
        antAlgorithm.Init();
        for (int i = 0; i < 3000; i++)
        {
            antAlgorithm.Iteration();
        }
        antAlgorithm.PrintBestTour("Eil51AS", 1);
        Assert.True(true);
    }

    [Test]
    public void Oliver30ACS()
    {
        antAlgorithmChooser = new AntAlgorithms.AntAlgorithmChooser(1, 2, 0.1, 10, 0.9);
        antAlgorithm = antAlgorithmChooser.Algorithm;
        antAlgorithm.Cities = TSPImporter.ImportTsp("oliver30.tsp");
        antAlgorithm.Init();
        for (int i = 0; i < 5000; i++)
        {
            antAlgorithm.Iteration();
        }
        antAlgorithm.PrintBestTour("Oliver30ACS", 1);
        Assert.True(true);
    }

    [Test]
    public void Oliver30AS()
    {
        antAlgorithmChooser = new AntAlgorithms.AntAlgorithmChooser(1, 2, 0.7, 10);
        antAlgorithm = antAlgorithmChooser.Algorithm;
        antAlgorithm.Cities = TSPImporter.ImportTsp("oliver30.tsp");
        antAlgorithm.Init();
        for (int i = 0; i < 2500; i++)
        {
            antAlgorithm.Iteration();
        }
        antAlgorithm.PrintBestTour("Oliver30AS", 1);
        Assert.True(true);
    }

    [Test]
    public void Oliver30ACSWithCustomPara()
    {
        antAlgorithmChooser = new AntAlgorithms.AntAlgorithmChooser(AntAlgorithms.Mode.AntColonySystem, 1, 2, 0.1, 10, 0.9, -1);
        antAlgorithm = antAlgorithmChooser.Algorithm;
        antAlgorithm.Cities = TSPImporter.ImportTsp("oliver30.tsp");
        antAlgorithm.Init();
        for (int i = 0; i < 1000; i++)
        {
            antAlgorithm.Iteration();
        }
        antAlgorithm.PrintBestTour("Oliver30ACSWithCustomPara", 1);
        Assert.True(true);
    }

    [Test]
    public void Berlin52ACS()
    {
        antAlgorithmChooser = new AntAlgorithms.AntAlgorithmChooser(1, 2, 0.1, 10, 0.9);
        antAlgorithm = antAlgorithmChooser.Algorithm;
        antAlgorithm.Cities = TSPImporter.ImportTsp("berlin52.tsp");
        antAlgorithm.Init();
        for (int i = 0; i < 200; i++)
        {
            antAlgorithm.Iteration();
        }
        antAlgorithm.PrintBestTour("berlin52", 1);
        Assert.True(true);
    }

    [Test]
    public void test8()
    {
        antAlgorithmChooser = new AntAlgorithms.AntAlgorithmChooser(AntAlgorithms.Mode.MinMaxAntSystem, 1, 2, 0.02, 8, -1, 0.05);
        antAlgorithm = antAlgorithmChooser.Algorithm;

        cities.Add(new City(10, 10, 0));
        cities.Add(new City(35, 20, 1));
        cities.Add(new City(20, 40, 2));
        cities.Add(new City(45, 65, 3));
        cities.Add(new City(55, 15, 4));
        cities.Add(new City(65, 40, 5));
        cities.Add(new City(75, 60, 6));
        cities.Add(new City(75, 30, 7));

        antAlgorithm.Cities = cities;
        antAlgorithm.Init();
        for (int i = 0; i < 4; i++)
            antAlgorithm.Iteration();
        antAlgorithm.PrintBestTour("test8", 1);
        Assert.True(true);
    }

}
