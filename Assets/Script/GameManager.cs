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
    public static float BackgrounfSpeed = 4;
}
