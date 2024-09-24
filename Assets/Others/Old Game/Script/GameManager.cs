using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static Action PlayerHitFired;
    public static Action GameEnd;
    public static Action ReSetGame;
    public static bool isAlive = true;
    public static float BackgrounfSpeed = 3;

    private void FixedUpdate()
    {
        if (BackgrounfSpeed<=7 && isAlive)
        {
            BackgrounfSpeed += (Time.deltaTime / 50);
        }
    }
}
