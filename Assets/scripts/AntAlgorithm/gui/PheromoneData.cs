/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2018
 *****************************************************/

/* PheromoneData is data for pheromones*/

using UnityEngine;

public class PheromoneData : MonoBehaviour {

    public string name;
    public int from;
    public int to;
    public float value;
}
