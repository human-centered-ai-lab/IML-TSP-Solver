
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
public class ASTest
{
    private List<City> cities;
    GameObject cityGameObject;
    AntAlgorithms.ASAlgorithm asa;

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
        asa = Camera.main.GetComponent<AntAlgorithms.ASAlgorithm>();
    }

    [TearDown]
    public void TearDown()
    {
        //SetUp runs after all test cases
    }

    [Test]
    public void Eil51()
    {
        asa.setCities(TSPImporter.importTsp("eil51.tsp"));
        asa.init();
        for (int i = 0; i < 3000; i++)
            asa.iteration();
        asa.printBestTour("SimpleTest");
        Assert.True(true);
    }

    [Test]
    public void Berlin52()
    {
        asa.setCities(TSPImporter.importTsp("berlin52.tsp"));
        asa.init();
        for (int i = 0; i < 3000; i++)
            asa.iteration();
        asa.printBestTour("berlin52");
        Assert.True(true);
    }

    [Test]
    public void test8()
    {
        cities.Add(new City(2, 4, 0, "Vienna", cityGameObject));
        cities.Add(new City(1, 9, 1, "Graz", cityGameObject));
        cities.Add(new City(3, 8, 2, "Klagenfurt", cityGameObject));
        cities.Add(new City(9, 1, 3, "Innsbruck", cityGameObject));
        cities.Add(new City(10, 1, 4, "Innsbruck", cityGameObject));

        cities.Add(new City(5, 4, 5, "Vienna", cityGameObject));
        cities.Add(new City(1, 11, 6, "Graz", cityGameObject));
        cities.Add(new City(3, 4, 7, "Klagenfurt", cityGameObject));

        asa.setCities(cities);
        asa.init();
        for (int i = 0; i < 400; i++)
            asa.iteration();
        asa.printBestTour("test8");
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

        asa.setCities(cities);
        asa.init();
        for (int i = 0; i < 120; i++)
            asa.iteration();
        asa.printBestTour("test50");
        Assert.True(true);
    }

    [Test]
    public void dantzig42()
    {

        cities.Add(new City(170, 85, 0, "D1", cityGameObject));
        cities.Add(new City(166, 88, 1, "D2", cityGameObject));
        cities.Add(new City(133, 73, 2, "D3", cityGameObject));
        cities.Add(new City(140, 70, 3, "D4", cityGameObject));
        cities.Add(new City(142, 55, 4, "D5", cityGameObject));

        cities.Add(new City(126, 53, 5, "D6", cityGameObject));
        cities.Add(new City(125, 60, 6, "D7", cityGameObject));
        cities.Add(new City(119, 68, 7, "D8", cityGameObject));
        cities.Add(new City(117, 74, 8, "D9", cityGameObject));
        cities.Add(new City(99,  83, 9, "D10", cityGameObject));

        cities.Add(new City(73, 79, 10, "D11", cityGameObject));
        cities.Add(new City(72, 91, 11, "D12", cityGameObject));
        cities.Add(new City(37, 94, 12, "D13", cityGameObject));
        cities.Add(new City(6, 106, 13, "D14", cityGameObject));
        cities.Add(new City(3, 97, 14, "D15", cityGameObject));

        cities.Add(new City(21, 82, 15, "D16", cityGameObject));
        cities.Add(new City(33, 67, 16, "D17", cityGameObject));
        cities.Add(new City(4, 66, 17, "D18", cityGameObject));
        cities.Add(new City(3,  42, 18, "D19", cityGameObject));
        cities.Add(new City(27,  33, 19, "D20", cityGameObject));

        cities.Add(new City(52,  41, 20, "D21", cityGameObject));
        cities.Add(new City(57,  59, 21, "D22", cityGameObject));
        cities.Add(new City(58,  66, 22, "D23", cityGameObject));
        cities.Add(new City(88,  65, 23, "D24", cityGameObject));
        cities.Add(new City(99,  67, 24, "D25", cityGameObject));

        cities.Add(new City(95,  55, 25, "D26", cityGameObject));
        cities.Add(new City(89,  55, 26, "D27", cityGameObject));
        cities.Add(new City(83,  38, 27, "D28", cityGameObject));
        cities.Add(new City(85,  25, 28, "D29", cityGameObject));
        cities.Add(new City(104,  35, 29, "D30", cityGameObject));

        cities.Add(new City(112,  37, 30, "D31", cityGameObject));
        cities.Add(new City(112,  24, 31, "D32", cityGameObject));
        cities.Add(new City(113,  13, 32, "D33", cityGameObject));
        cities.Add(new City(125,  30, 33, "D34", cityGameObject));
        cities.Add(new City(135,  32, 34, "D35", cityGameObject));

        cities.Add(new City(147,  18, 35, "D36", cityGameObject));
        cities.Add(new City(147,  36, 36, "D37", cityGameObject));
        cities.Add(new City(154,  45, 37, "D38", cityGameObject));
        cities.Add(new City(157,  54, 38, "D39", cityGameObject));
        cities.Add(new City(158,  61, 39, "D40", cityGameObject));

        cities.Add(new City(172,  82, 40, "D41", cityGameObject));
        cities.Add(new City(174,  87, 41, "D42", cityGameObject));

        asa.setCities(cities);
        asa.init();
        for (int i = 0; i < 120; i++)
            asa.iteration();
        asa.printBestTour("dantzig42");
        Assert.True(true);
    }
}
