
/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2017
 *****************************************************/

/* AntAlgorithSimpleTest */

using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;

[TestFixture]
public class AntAlgorithmSimpleTest
{
    private List<City> cities;
    GameObject cityGameObject;
    AntAlgorithms.AntAlgorithmSimple aas;

    [TestFixtureSetUp]
    public void Init()
    {
        //Init runs once before running test cases.
    }

    [TestFixtureTearDown]
    public void CleanUp()
    {
        //CleanUp runs once after all test cases are finished.
    }

    [SetUp]
    public void SetUp()
    {
        cityGameObject = new GameObject();
        cities = new List<City>();
        aas = Camera.main.GetComponent<AntAlgorithms.AntAlgorithmSimple>();
    }

    [TearDown]
    public void TearDown()
    {
        //SetUp runs after all test cases
    }

    [Test]
    public void Simple()
    {       
        cities.Add(new City(2, 4, 0, "Vienna", cityGameObject));
        cities.Add(new City(1, 9, 1, "Graz", cityGameObject));
        cities.Add(new City(3, 8, 2, "Klagenfurt", cityGameObject));
        cities.Add(new City(9, 1, 3, "Innsbruck", cityGameObject));

        aas.setCities(cities);
        aas.init();
        for (int i = 0; i < 20; i++)
            aas.step();
        Assert.True(true);
    }

    [Test]
    public void test10()
    {
        cities.Add(new City(2, 4, 0, "Vienna", cityGameObject));
        cities.Add(new City(1, 9, 1, "Graz", cityGameObject));
        cities.Add(new City(3, 8, 2, "Klagenfurt", cityGameObject));
        cities.Add(new City(9, 1, 3, "Innsbruck", cityGameObject));
        cities.Add(new City(10, 1, 4, "Innsbruck", cityGameObject));

        cities.Add(new City(5, 4, 5, "Vienna", cityGameObject));
        cities.Add(new City(1, 11, 6, "Graz", cityGameObject));
        cities.Add(new City(3, 4, 7, "Klagenfurt", cityGameObject));

        aas.setCities(cities);
        aas.init();
        for (int i = 0; i < 400; i++)
            aas.iteration();
        Assert.True(true);
    }

    [Test]
    public void test50()
    {
        cities.Add(new City(2, 4, 0, "Vienna", cityGameObject));
        cities.Add(new City(1, 9, 1, "Graz", cityGameObject));
        cities.Add(new City(3, 8, 2, "Klagenfurt", cityGameObject));
        cities.Add(new City(9, 1, 3, "Innsbruck", cityGameObject));
        cities.Add(new City(9, 2, 4, "Innsbruck", cityGameObject));

        cities.Add(new City(22, 4, 5, "Vienna", cityGameObject));
        cities.Add(new City(1, 11, 6, "Graz", cityGameObject));
        cities.Add(new City(3, 4, 7, "Klagenfurt", cityGameObject));
        cities.Add(new City(55, 1, 8, "Innsbruck", cityGameObject));
        cities.Add(new City(9, 38, 9, "Innsbruck", cityGameObject));

        cities.Add(new City(12, 14, 10, "Vienna", cityGameObject));
        cities.Add(new City(1, 33, 11, "Graz", cityGameObject));
        cities.Add(new City(3, 53, 12, "Klagenfurt", cityGameObject));
        cities.Add(new City(80, 1, 13, "Innsbruck", cityGameObject));
        cities.Add(new City(53, 11, 14, "Innsbruck", cityGameObject));

        cities.Add(new City(22, 42, 15, "Vienna", cityGameObject));
        cities.Add(new City(11, 92, 16, "Graz", cityGameObject));
        cities.Add(new City(32, 82, 17, "Klagenfurt", cityGameObject));
        cities.Add(new City(91, 12, 18, "Innsbruck", cityGameObject));
        cities.Add(new City(92, 12, 19, "Innsbruck", cityGameObject));

        cities.Add(new City(23, 44, 20, "Vienna", cityGameObject));
        cities.Add(new City(14, 94, 21, "Graz", cityGameObject));
        cities.Add(new City(33, 84, 22, "Klagenfurt", cityGameObject));
        cities.Add(new City(94, 14, 23, "Innsbruck", cityGameObject));
        cities.Add(new City(93, 14, 24, "Innsbruck", cityGameObject));

        cities.Add(new City(25, 46, 25, "Vienna", cityGameObject));
        cities.Add(new City(16, 96, 26, "Graz", cityGameObject));
        cities.Add(new City(35, 86, 27, "Klagenfurt", cityGameObject));
        cities.Add(new City(96, 16, 28, "Innsbruck", cityGameObject));
        cities.Add(new City(95, 16, 29, "Innsbruck", cityGameObject));

        cities.Add(new City(26, 47, 30, "Vienna", cityGameObject));
        cities.Add(new City(17, 97, 31, "Graz", cityGameObject));
        cities.Add(new City(36, 87, 32, "Klagenfurt", cityGameObject));
        cities.Add(new City(97, 17, 33, "Innsbruck", cityGameObject));
        cities.Add(new City(96, 17, 34, "Innsbruck", cityGameObject));

        cities.Add(new City(27, 48, 35, "Vienna", cityGameObject));
        cities.Add(new City(18, 98, 36, "Graz", cityGameObject));
        cities.Add(new City(37, 88, 37, "Klagenfurt", cityGameObject));
        cities.Add(new City(98, 18, 38, "Innsbruck", cityGameObject));
        cities.Add(new City(97, 18, 39, "Innsbruck", cityGameObject));

        cities.Add(new City(29, 40, 40, "Vienna", cityGameObject));
        cities.Add(new City(10, 90, 41, "Graz", cityGameObject));
        cities.Add(new City(39, 80, 42, "Klagenfurt", cityGameObject));
        cities.Add(new City(90, 10, 43, "Innsbruck", cityGameObject));
        cities.Add(new City(99, 10, 44, "Innsbruck", cityGameObject));

        cities.Add(new City(82, 44, 45, "Vienna", cityGameObject));
        cities.Add(new City(61, 49, 46, "Graz", cityGameObject));
        cities.Add(new City(73, 48, 47, "Klagenfurt", cityGameObject));
        cities.Add(new City(49, 41, 48, "Innsbruck", cityGameObject));
        cities.Add(new City(39, 41, 49, "Innsbruck", cityGameObject));

        aas.setCities(cities);
        aas.init();
        for(int i = 0; i < 120; i++)
            aas.iteration();
        Assert.True(true);
    }
}
