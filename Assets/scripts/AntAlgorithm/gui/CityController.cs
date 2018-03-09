/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2018
 *****************************************************/

/* CityController is dragged in to the Scene and is used for city ui controlling*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityController : MonoBehaviour
{
    private static GameObject _cityPrefab;
    public static List<GameObject> cityObjects;
    private static int numOfCities = 0;
    public int Id { get; private set; }

    // Use this for initialization
    public static void Init()
    {
        cityObjects = new List<GameObject>();
        int minX, maxX, minY, maxY;

        var cities = AntAlgorithmManager.Instance.Cities;
        numOfCities = cities.Count;
        GetExtremeCityCoordinates(cities, out minX, out maxX, out minY, out maxY);

        int absoluteMaxX = Mathf.Max(Mathf.Abs(minX), Mathf.Abs(maxX));
        int absoluteMaxY = Mathf.Max(Mathf.Abs(minY), Mathf.Abs(maxY));

        foreach (var city in cities)
        {
            float xPos = (city.XPosition / (float)absoluteMaxX) * Screen.width * 5;
            float yPos = (city.YPosition / (float)absoluteMaxY) * Screen.height * 5;
            var pos = new Vector3(xPos, yPos, 0);

            cityObjects.Add(Create(pos, city.Id));
        }
    }
    public static GameObject Create(Vector3 position, int id)
    {
        if (_cityPrefab == null)
        {
            _cityPrefab = Resources.Load("Prefabs/City") as GameObject;
        }

        var cityGameObject = Instantiate(_cityPrefab, position, Quaternion.identity);
        cityGameObject.name = "CityGameObject_" + id;
        Text text = cityGameObject.GetComponentInChildren<Image>().GetComponentInChildren<Text>();
        text.text = "" + id;

        return cityGameObject;
    }

    public static void DestroyAll()
    {
        if (cityObjects != null)
        {
            for (int i = 0; i < cityObjects.Count; i++)
                Destroy(cityObjects[i]);
            cityObjects.Clear();
        }
    }

    #region Helper Methods

    private static void GetExtremeCityCoordinates(IEnumerable<City> cities, out int minX, out int maxX, out int minY, out int maxY)
    {
        minX = minY = int.MaxValue;
        maxX = maxY = int.MinValue;

        foreach (var city in cities)
        {
            var x = city.XPosition;
            var y = city.YPosition;

            if (x < minX)
                minX = x;
            if (x > maxX)
                maxX = x;
            if (y < minY)
                minY = y;
            if (y > maxY)
                maxY = y;
        }
    }
    #endregion
}
