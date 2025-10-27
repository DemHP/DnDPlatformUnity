using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    public float zoomAmount = 1f;
    Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D
        float moveY = Input.GetAxis("Vertical"); // W/S

        // Movement
        Vector3 move = new Vector3(moveX, moveY, 0f);
        // Scroll
        Vector2 scroll = Mouse.current.scroll.ReadValue();

        // Apply movement
        transform.position += move * moveSpeed * Time.deltaTime;

        if (scroll.y > 0)
        {
            mainCam.orthographicSize -= zoomAmount;
        }
        if (scroll.y < 0)
        {
            mainCam.orthographicSize += zoomAmount;
        }
        mainCam.orthographicSize = Mathf.Clamp(mainCam.orthographicSize, 1f, 100f);

    }
}
