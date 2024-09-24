using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperatorOverloding : MonoBehaviour
{
    Vector3 v1, v2, v3;

    void Start()
    {
        Point p1 = new Point(5, 7);
        Point p2 = new Point(3, 4);

        Point resultAdd = p1 + p2; // Using overloaded + operator
        Point resultSub = p1 - p2; // Using overloaded - operator
        Debug.Log("1");
        Debug.LogError(resultAdd);
        Debug.Log("3");
        //Debug.LogError(resultSub);

       
        v3 = v1 + v2;
    }
}

class Point
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    // Overloading the + operator
    public static Point operator +(Point p1, Point p2)
    {
        return new Point(p1.X + p2.X, p1.Y + p2.Y);
    }

    // Overloading the - operator
    public static Point operator -(Point p1, Point p2)
    {
        return new Point(p1.X - p2.X, p1.Y - p2.Y);
    }

    public override string ToString()
    {
        Debug.Log("2");
        return $"({X}, {Y})";
    }
}


