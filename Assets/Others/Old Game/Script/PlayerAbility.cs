using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerAbility : MonoBehaviour
{
    // For No Death Gem Ability
    private bool isNoDeathActive = false;
    private float noDeathDuration = 5f;
    private float noDeathTimer = 0f;
    public TMP_Text noDeathTimerCounter;
    
    // For BackGround Slow Ability
    private bool isSlowSpeedActive = false;
    private float slowSpeedDuration = 5f;
    private float slowSpeedTimer = 0f;
    private float originalBackgroundSpeed;
    public float slowBackgroundSpeed = 0.5f;
    public float backgroundSpeed = 2f;
    public TMP_Text SlowTimeCounter;

    // For Resize Player
    private bool isResizeActive = false;
    private float resizeDuration = 5f;
    private float resizeTimer = 0f;
    private Vector3 originalSize;
    private Rigidbody2D rb;
    public Vector3 resizedSize = new Vector3(0.5f, 0.5f, 0.5f);
    public TMP_Text resizeTimeCounter;
    //for Extra Life
    private int lives = 1;
    private int maxLives = 2;
    public TMP_Text lifeCounter;


    private Collider2D playerCollider;
    public ObstacleGenerator obstaclGen;

    void Start()
    {
        playerCollider = GetComponent<Collider2D>();
      /*  originalBackgroundSpeed = backgroundSpeed;*/
        originalSize = transform.localScale;
    }

    void Update()
    {
        lifeCounter.text = "Life Count:" + Mathf.Ceil(lives).ToString();
        if (lives>0)
        {
            //MoveBackground();
            if (isNoDeathActive)
            {
                noDeathTimer -= Time.deltaTime;
                noDeathTimerCounter.text = "No Death: " + Mathf.Ceil(noDeathTimer).ToString();
                if (noDeathTimer <= 0)
                {
                    DeactivateNoDeath();
                }
            }
            if (isSlowSpeedActive)
            {
                slowSpeedTimer -= Time.deltaTime;
                SlowTimeCounter.text = "Slow Speed Timer:" + Mathf.Ceil(slowSpeedTimer).ToString();
                if (slowSpeedTimer <= 0)
                {
                    DeactivateSlowSpeed();
                }
            }
            if (isResizeActive)
            {
                resizeTimer -= Time.deltaTime;
                resizeTimeCounter.text = "Player Resize in:" + Mathf.Ceil(resizeTimer).ToString();
                if (resizeTimer <= 0)
                {
                    DeactivateResize();
                }
            }
        }
        
    }
    public void AddLife()
    {
        if (lives < maxLives)
        {
            lives++;
            Debug.Log("Extra life gained! Lives: " + lives);
        }
    }
    void ActivateResize()
    {
        isResizeActive = true;
        resizeTimer = resizeDuration;
        transform.localScale = resizedSize;
    }

    void DeactivateResize()
    {
        isResizeActive = false;
        transform.localScale = originalSize;
    }

    public void TriggerResizeAbility()
    {
        ActivateResize();
    }
    void MoveBackground()
    {
        obstaclGen.distance = 500;
    }
    void ActivateSlowSpeed()
    {
        //GameManager.BackgrounfSpeed = 
        isSlowSpeedActive = true;
        slowSpeedTimer = slowSpeedDuration;
        GameManager.BackgrounfSpeed = slowBackgroundSpeed;
    }

    void DeactivateSlowSpeed()
    {
        isSlowSpeedActive = false;
        backgroundSpeed = originalBackgroundSpeed;
    }

    public void TriggerSlowSpeedAbility()
    {
        ActivateSlowSpeed();
    }

    void ActivateNoDeath()
    {
        isNoDeathActive = true;
        noDeathTimer = noDeathDuration;
        playerCollider.enabled = false;
        Debug.Log("NoDeath activated");
    }

    void DeactivateNoDeath()
    {
        isNoDeathActive = false;

        playerCollider.enabled = true;
    }

    public void TriggerNoDeathAbility()
    {
        Debug.Log("ActivingNoDeath");
        ActivateNoDeath();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.GetComponent<GemScript>().ability)
        {
            case GemScript.AbilityObjects.NoDeathGem:
                if (!isNoDeathActive)
                {
                    Debug.Log("TriggeringNoDeath");
                    Destroy(collision.gameObject);
                    TriggerNoDeathAbility();
                }
                else
                    return;
                break;
            case GemScript.AbilityObjects.SlowSpeedGem:
                if (!isSlowSpeedActive)
                {
                    TriggerSlowSpeedAbility();
                    Destroy(collision.gameObject);
                }
                break;
            case GemScript.AbilityObjects.ResizeGem:
                if (!isResizeActive)
                {
                    TriggerResizeAbility();
                    Destroy(collision.gameObject);
                }

                break;
            case GemScript.AbilityObjects.ExtraLifeGem:
                AddLife();
                Destroy(collision.gameObject);
                break;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision Detected");
        if (collision.transform.GetComponent<GemScript>())
        {
            Debug.Log(lives);
            if (lives > 0)
            {
                lives--;
                if (lives <= 0)
                {
                    GameManager.PlayerHitFired?.Invoke();
                }
                else
                {
                    Debug.Log("Lost a life! Remaining lives: " + lives);
                }
                //rb.AddForce()
            }
        }
    }

}
