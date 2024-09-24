using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    void Update()
    {
        transform.Translate(Vector3.left * GameManager.BackgrounfSpeed * Time.deltaTime);
        if (transform.position.x < -125f)
        {
            Destroy(gameObject);
        }
    }
}
