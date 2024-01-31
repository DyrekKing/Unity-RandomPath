using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int X;
    public int Y;

    public void set(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Vector2 get()
    {
        return new Vector2(X, Y);
    }
}
