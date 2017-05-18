
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
[assembly: AssemblyVersionAttribute("0.1")]

[TestFixture]
public class ASTest
{
    private List<City> cities;
    private AntAlgorithms.AntAlgorithmChooser aac;
    private AntAlgorithms.AntAlgorithm aa;


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
        aac = new AntAlgorithms.AntAlgorithmChooser(AntAlgorithms.Mode.antColonySystem, 1, 2, 0.1, 10, 0.9, 0.000088, 0.000088);
        aa = aac.getAlgorithm();
    }

    [TearDown]
    public void TearDown()
    {
        //SetUp runs after all test cases
    }

    [Test]
    public void Eil51()
    {
        aa.setCities(TSPImporter.importTsp("eil51.tsp"));
        aa.init();
        for (int i = 0; i < 3000; i++)
        {
            aa.iteration();
        }
        aa.printBestTour("Eil51");
        Assert.True(true);
    }

    [Test]
    public void Oliver30()
    {
        aa.setCities(TSPImporter.importTsp("oliver30.tsp"));
        aa.init();
        for (int i = 0; i < 2500; i++)
        {
            aa.iteration();
        }
        aa.printBestTour("oliver30");
        Assert.True(true);
    }

    [Test]
    public void Berlin52()
    {
        aa.setCities(TSPImporter.importTsp("berlin52.tsp"));
        aa.init();
        for (int i = 0; i < 1; i++)
            aa.iteration();
        aa.printBestTour("berlin52");
        Assert.True(true);
    }

    [Test]
    public void test8()
    {
        cities.Add(new City(2, 4, 0));
        cities.Add(new City(1, 9, 1));
        cities.Add(new City(3, 8, 2));
        cities.Add(new City(9, 1, 3));
        cities.Add(new City(10, 1, 4));

        cities.Add(new City(5, 4, 5));
        cities.Add(new City(1, 11, 6));
        cities.Add(new City(3, 4, 7));

        aa.setCities(cities);
        aa.init();
        for (int i = 0; i < 400; i++)
            aa.iteration();
        aa.printBestTour("test8");
        Assert.True(true);
    }

}
