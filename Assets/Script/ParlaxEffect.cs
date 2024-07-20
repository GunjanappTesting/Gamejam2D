using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParlaxEffect : MonoBehaviour
{
    public Transform[] backgrounds; // Array of all the back and foregrounds to be parallaxed
    public float[] parallaxScales; // The proportion of the camera's movement to move the backgrounds by
    public float smoothing = 1f; // How smooth the parallax is going to be. Set this above 0.

    private Vector3 previousCameraPosition; // The position of the camera in the previous frame

    // Called before Start(). Great for references.
    private void Awake()
    {
        // Set up camera reference
        previousCameraPosition = transform.position;
    }

    // Called once per frame
    private void Update()
    {
        // For each background
        for (int i = 0; i < backgrounds.Length; i++)
        {
            // The parallax is the opposite of the camera movement because the previous frame multiplied by the scale
            float parallax = (previousCameraPosition.x - transform.position.x) * parallaxScales[i];

            // Set a target x position which is the current position plus the parallax
            float backgroundTargetPositionX = backgrounds[i].position.x + parallax;

            // Create a target position which is the background's current position with its target x position
            Vector3 backgroundTargetPosition = new Vector3(backgroundTargetPositionX, backgrounds[i].position.y, backgrounds[i].position.z);

            // Fade between current position and the target position using Lerp
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, backgroundTargetPosition, smoothing * Time.deltaTime);
        }

        // Set the previous camera position to the camera's position at the end of the frame
        previousCameraPosition = transform.position;
    }
}
