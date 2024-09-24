using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Transform gun;       // Reference to the gun's position and direction   
    public LineRenderer ray;   // Line renderer to display the laser

    private GameObject destroyObject;
    private IGunRotator gunRotator;
    private ILaserCaster laserCaster;

    private void Awake()
    {
        gunRotator = new GunRotator(transform);
        laserCaster = new LaserCaster(ray,gun);
    }

    private void Update()
    {
        destroyObject= laserCaster.CastLaser();
        gunRotator.RotateGun();
        BlastObject();
    }   

    private void BlastObject()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (destroyObject != null)
            {
                Destroy(destroyObject);
            }
        }
    }   
}
public interface IGunRotator
{
    void RotateGun();
}

public interface ILaserCaster
{
    GameObject CastLaser();
}


public class LaserCaster : ILaserCaster
{
    private readonly LineRenderer lineRenderer;
    private readonly Transform gun;
    private readonly float maxDistance = 50;  // Max distance the laser can travel
    private readonly int maxBounces = 10;     // Max number of bounces

    public LaserCaster(LineRenderer lineRenderer, Transform gun)
    {
        this.lineRenderer = lineRenderer;
        this.gun = gun;
    }

    public GameObject CastLaser()
    {
        
        GameObject hitObject = null;
        Vector2 currentPosition = gun.position;
        Vector2 currentDirection = gun.right;

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, currentPosition);

        List<Vector3> laserPositions = new List<Vector3> { currentPosition };

        for (int i = 0; i < maxBounces; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentPosition, currentDirection, maxDistance);

            if (hit.collider != null)
            {
                laserPositions.Add(hit.point);

                Vector2 reflectionDirection = Vector2.Reflect(currentDirection, hit.normal);

                currentPosition = hit.point + reflectionDirection * 0.01f;
                currentDirection = reflectionDirection;

                if (hit.collider.CompareTag("Player"))
                {
                    // destryabel object
                    hitObject = hit.collider.gameObject;
                    break;
                }
                if (hit.collider.CompareTag("Finish"))
                {
                    // non reflection object
                    break;
                }
            }
            else
            {
                laserPositions.Add(currentPosition + currentDirection * maxDistance);
                break;
            }
        }

        lineRenderer.positionCount = laserPositions.Count;
        lineRenderer.SetPositions(laserPositions.ToArray());
        return hitObject;
    }
}


public class GunRotator : IGunRotator
{
    private readonly Transform gunTransform;
    private readonly float rotationSpeed = 20;  // Speed of rotation
    private readonly float minRotation = 20;     // Minimum allowed Z-axis rotation
    private readonly float maxRotation = 170;    // Maximum allowed Z-axis rotation
    public GunRotator(Transform gunTransform)
    {
        this.gunTransform = gunTransform;
    }

    public void RotateGun()
    {
        float zRotation = gunTransform.eulerAngles.z;

        if (zRotation > 180f)
        {
            zRotation -= 360f;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            zRotation += rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            zRotation -= rotationSpeed * Time.deltaTime;
        }

        zRotation = Mathf.Clamp(zRotation, minRotation, maxRotation);

        gunTransform.rotation = Quaternion.Euler(0, 0, zRotation);
    }
}
