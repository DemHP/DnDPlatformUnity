using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float zoomAmount = 2f;

    public SpriteRenderer background;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        // Movement
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        transform.position += new Vector3(moveX, moveY, 0f) * moveSpeed * Time.deltaTime;

        // Zoom
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll != 0)
        {
            mainCam.orthographicSize -= scroll * zoomAmount * Time.deltaTime;
            mainCam.orthographicSize = Mathf.Clamp(mainCam.orthographicSize, 1f, 100f);
        }
    }

    void LateUpdate()
    {
        ScaleBackground();
    }

    void ScaleBackground()
    {
        // Keep background centered
        background.transform.position = new Vector3(
            mainCam.transform.position.x,
            mainCam.transform.position.y,
            background.transform.position.z
        );

        float screenHeight = mainCam.orthographicSize * 2f;
        float screenWidth = screenHeight * mainCam.aspect;

        Vector2 spriteSize = background.sprite.bounds.size;

        background.transform.localScale = new Vector3(
            screenWidth / spriteSize.x,
            screenHeight / spriteSize.y,
            1f
        );
    }
}
