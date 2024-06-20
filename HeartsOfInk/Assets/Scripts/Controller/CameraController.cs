using Assets.Scripts.Data;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float DefaultMovementForce = 0.1f;
    private Camera mainCamera;

    public int minZoom;
    public int maxZoom;
    public int zoomPerClick;
    public int minXPosition;
    public int maxXPosition;
    public int minYPosition;
    public int maxYPosition;
    public bool movementEnabled;

    void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        movementEnabled = true;
    }

    void Update()
    {
        InputCameraMovement();
    }

    /// <summary>
    /// Recibe el input de movimiento de cámara realiza por el usuario para realizar ese movimiento.
    /// </summary>
    /// <param name="horizontalAxis"> Movimiento el en el eje horizontal. </param>
    /// <param name="verticalAxis"> Movimiento en el eje vertical. </param>
    public void InputCameraMovement()
    {
        Vector3 cameraPosition;

        if (movementEnabled)
        {
            float horizontalAxis = Input.GetAxis(AxisData.HorizontalAxis);
            float verticalAxis = Input.GetAxis(AxisData.VerticalAxis);

            cameraPosition = mainCamera.transform.position;
            cameraPosition.x = LimitMovement(AxisData.Axis.HORIZONTAL, cameraPosition.x + DefaultMovementForce * horizontalAxis);
            cameraPosition.y = LimitMovement(AxisData.Axis.VERTICAL, cameraPosition.y + DefaultMovementForce * verticalAxis);
            mainCamera.transform.position = cameraPosition;
        }
    }

    private float LimitMovement(AxisData.Axis axis, float newPosition)
    {
        switch (axis)
        {
            case AxisData.Axis.HORIZONTAL:
                if (newPosition < minXPosition)
                {
                    return minXPosition;
                }
                else if (newPosition > maxXPosition)
                {
                    return maxXPosition;
                }
                else
                {
                    return newPosition;
                }
            case AxisData.Axis.VERTICAL:
                if (newPosition < minYPosition)
                {
                    return minYPosition;
                }
                else if (newPosition > maxYPosition)
                {
                    return maxYPosition;
                }
                else
                {
                    return newPosition;
                }
            default:
                Debug.LogError("Unexpected Axis on LimitMovement.");
                return newPosition;
        }
    }

    public void ZoomIn()
    {
        Vector3 cameraPosition = mainCamera.transform.position;

        if (cameraPosition.z < maxZoom)
        {
            cameraPosition.z += zoomPerClick;
            mainCamera.transform.position = cameraPosition;
        }
    }

    public void ZoomOut()
    {
        Vector3 cameraPosition = mainCamera.transform.position;

        if (cameraPosition.z > minZoom)
        {
            cameraPosition.z -= zoomPerClick;
            mainCamera.transform.position = cameraPosition;
        }
    }

    /// <summary>
    /// Método utilizado para obtener la posición en pantalla de un click.
    /// 
    /// </summary>
    /// <returns></returns>
    public Vector3 ScreenToWorldPoint()
    {
        Vector3 cameraPosition = transform.position;
        Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraPosition.z);

        position = Camera.main.ScreenToViewportPoint(position);
        position = Camera.main.ViewportToWorldPoint(position);
        position *= -1f;
        position.x += cameraPosition.x * 2;
        position.y += cameraPosition.y * 2;
        position.z = 0;

        return position;
    }
}
