using Assets.Scripts.Data;
using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
    }

    /// <summary>
    /// Recibe el input de movimiento de cámara realiza por el usuario para realizar ese movimiento.
    /// </summary>
    /// <param name="horizontalAxis"> Movimiento el en el eje horizontal. </param>
    /// <param name="verticalAxis"> Movimiento en el eje vertical. </param>
    public void InputCameraMovement(float horizontalAxis, float verticalAxis)
    {
        Vector3 cameraPosition = mainCamera.transform.position;

        cameraPosition.x = LimitMovement(AxisData.Axis.HORIZONTAL, cameraPosition.x + DefaultMovementForce * horizontalAxis);
        cameraPosition.y = LimitMovement(AxisData.Axis.HORIZONTAL, cameraPosition.y + DefaultMovementForce * verticalAxis);

        mainCamera.transform.position = cameraPosition;
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
}
