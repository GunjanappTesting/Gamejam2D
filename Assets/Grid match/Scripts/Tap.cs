using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tap : MonoBehaviour
{
    private void OnMouseDown()
    {
        GridManager.Instance.OnObjectClicked(gameObject);
    }
}
