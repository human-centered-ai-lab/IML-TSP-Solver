/* City2D represents a point in 2D space.
 * PROGRAMMED BY Andrej Müller (andrej.mueller@student.tugraz.at), student of TU Graz - University of Technology
 * */
using UnityEngine;

public class City{

    private int id;
    private int xPosition;
    private int yPosition;
    private GameObject city;

    public City(int xPosition, int yPosition, int id, string name, GameObject gameObject)
    {
        this.id = id;
        this.xPosition = xPosition;
        this.yPosition = yPosition;
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