using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerAbility : MonoBehaviour
{
    // For No Death Gem Ability
    private bool isNoDeathActive = false;
    private float noDeathDuration = 10f;
    private float noDeathTimer = 0f;
    public TextMeshProUGUI noDeathIndicator;
  
    // For BackGround Slow Ability
    private bool isSlowSpeedActive = false;
    private float slowSpeedDuration = 10f;
    public TextMeshProUGUI slowSpeed;
    private float slowSpeedTimer = 0f;
    private float originalBackgroundSpeed;
    public float slowBackgroundSpeed = 0.5f;
    public float backgroundSpeed = 2f;

    // For Resize Player
    private bool isResizeActive = false;
    public TextMeshProUGUI playerResizeTimer ;
    private float resizeDuration = 10f;
    private float resizeTimer = 0f;
    private Vector3 originalSize;
    public Vector3 resizedSize = new Vector3(0.5f, 0.5f, 0.5f);
    //for Extra Life
    private int lives = 1;
    public TextMeshProUGUI lifeCounter;
    private int maxLives = 2;


    private Collider2D playerCollider;
    public GameObject background;

    void Start()
    {
        playerCollider = GetComponent<Collider2D>();
        originalBackgroundSpeed = backgroundSpeed;
        originalSize = transform.localScale;
    }

    void Update()
    {
            lifeCounter.text = "Life Count:" + Mathf.Ceil(lives).ToString();
        if (lives>0)
        {
            MoveBackground();
            if (isNoDeathActive)
            {
                noDeathTimer -= Time.deltaTime;
                noDeathIndicator.text = "No Death: " + Mathf.Ceil(noDeathTimer).ToString();
                if (noDeathTimer <= 0)
                {
                    DeactivateNoDeath();
                }
            }
            if (isSlowSpeedActive)
            {
                slowSpeedTimer -= Time.deltaTime;
                slowSpeed.text = "Slow Speed Timer:" + Mathf.Ceil(slowSpeedTimer).ToString();
                if (slowSpeedTimer <= 0)
                {
                    DeactivateSlowSpeed();
                }
            }
            if (isResizeActive)
            {
                resizeTimer -= Time.deltaTime;
                playerResizeTimer.text = "Player Resize in:" + Mathf.Ceil(resizeTimer).ToString();
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
        background.transform.Translate(Vector3.left * backgroundSpeed * Time.deltaTime);
    }
    void ActivateSlowSpeed()
    {
        isSlowSpeedActive = true;
        slowSpeedTimer = slowSpeedDuration;
        backgroundSpeed = slowBackgroundSpeed;
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
    void OnTriggerEnter2D(Collider2D collision)
    {
        
        Debug.Log("Collision Detected");
        switch (collision.GetComponent<GemScript>().ability)
        {
             case GemScript.AbilityObjects.NoDeathGem:
                if (!isNoDeathActive)
                {
                    Debug.Log("TriggeringNoDeath");
                    TriggerNoDeathAbility();
                }
                else
                    return;
                break;
            case GemScript.AbilityObjects.SlowSpeedGem:
                if (!isSlowSpeedActive)
                {
                    TriggerSlowSpeedAbility();
                }
                break;
            case GemScript.AbilityObjects.ResizeGem:
                if (!isResizeActive)
                {
                    TriggerResizeAbility();
                }

                break;
            case GemScript.AbilityObjects.ExtraLifeGem:
                AddLife();
                break;
            case GemScript.AbilityObjects.Obstracles:
                if (lives > 0)
                {
                    lives--;
                    if (lives <= 0)
                    {
                        Debug.Log("Game Over!");
                    }
                    else
                    {
                        Debug.Log("Lost a life! Remaining lives: " + lives);
                    }
                }
                break;

            default:
                break;
        }
    
    }
    
}
