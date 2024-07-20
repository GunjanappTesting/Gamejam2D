using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemScript : MonoBehaviour
{
    public AbilityObjects ability;
    public bool isDestroybal;
    public enum AbilityObjects
    {
        NoDeathGem = 0,
        SlowSpeedGem = 1,
        ResizeGem = 2,
        ExtraLifeGem = 3,
        Obstracles=4
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDestroybal) return;
        Destroy(gameObject);
    }
}
