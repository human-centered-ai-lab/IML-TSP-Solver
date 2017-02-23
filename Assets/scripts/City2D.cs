/* City2D represents a point in 2D space.
 * PROGRAMMED BY Andrej Müller (andrej.mueller@student.tugraz.at), student of TU Graz - University of Technology
 * */
using UnityEngine;

public class City2D: MonoBehaviour{

    private int id;
    private int xPosition;
    private int yPosition;
    private GameObject city;

    public City2D(int xPosition, int yPosition, int id, string name, GameObject cityGA)
    {
        this.id = id;
        this.xPosition = xPosition;
        this.yPosition = yPosition;

        if (cityGA != null)
        {
            Debug.Log(cityGA.name);
            this.city = (GameObject)Instantiate(cityGA) as GameObject;
            city.transform.position = new Vector3(xPosition, yPosition, 0);
            //city.name = "City:" + name;
        }
    }

    public int getId()
    {
        return id;
    }

    public int getXPosition()
    {
        return xPosition;
    }

    public void setXPosition(int xPosition)
    {
        this.xPosition = xPosition;
    }

    public int getYPosition()
    {
        return yPosition;
    }

    public void setYPosition(int yPosition)
    {
        this.yPosition = yPosition;
    }
}