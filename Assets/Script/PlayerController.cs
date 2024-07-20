using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 targetPosition, initialMousePos ;
    private Camera mainCamera;
    private bool isDragging;
    private float zCoord, verticalOffset;

    public float speed = 5.0f; // Speed of the movement
    public float dragThreshold;
    void Start()
    {
        mainCamera = Camera.main;
        targetPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            initialMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0) && !isDragging)
        {
            float f = initialMousePos.y - Input.mousePosition.y;
            if (Vector3.Distance(initialMousePos, Input.mousePosition) > dragThreshold)
            {
                isDragging = true;
                Vector3 mouseWorldPos = GetMouseWorldPos();
                verticalOffset = transform.position.y - mouseWorldPos.y;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
        Debug.Log(isDragging);
        if (isDragging)
        {
            Vector3 mouseWorldPos = GetMouseWorldPos();
            //float distance = Vector3.Distance(mouseWorldPos, transform.position);
            //Debug.Log(distance);
            //if (distance < 10.5f)
            //{
                targetPosition = new Vector3(transform.position.x, mouseWorldPos.y + verticalOffset, transform.position.z);
                targetPosition = ClampPositionToScreen(targetPosition);
                transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
           // }
        }
           
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    private Vector3 ClampPositionToScreen(Vector3 pos)
    {
        Vector3 screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, zCoord));

        float objectHeight = GetComponent<Renderer>().bounds.size.y / 2;

        pos.y = Mathf.Clamp(pos.y, -screenBounds.y + objectHeight, screenBounds.y - objectHeight);

        return pos;
    }
}
