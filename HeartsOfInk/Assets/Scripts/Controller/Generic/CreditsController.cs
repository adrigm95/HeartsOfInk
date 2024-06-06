using Assets.Scripts.Utils;
using UnityEngine;

/// <summary>
/// Controlador que maneja el panel de créditos (CreditsPanel) en la pantalla de créditos.
/// </summary>
public class CreditsController : MonoBehaviour
{
    public SceneChangeController sceneChangeController;

    /// <summary>
    /// Velocidad a la que se mueve el texto verticalmente.
    /// </summary>
    public float Speed;

    /// <summary>
    /// Posición que, una vez alcanzada, hace que se salga de la pantalla de créditos para volver al menú principal.
    /// </summary>
    public float EndPosition;

    void Start()
    {
        
    }

    void Update()
    {
        TimeUtils.SetTimeToDefualt();

        float newYPosition = transform.position.y + Speed * Time.deltaTime;

        if (newYPosition > EndPosition)
        {
            Debug.Log("Changing scene, newYPosition: " + newYPosition);
            sceneChangeController.ChangeScene(SceneChangeController.Scenes.MainMenu);
        }
        else
        {
            Debug.Log("newYPosition: " + newYPosition);
            transform.position = new Vector3(transform.position.x, newYPosition, transform.position.z);
        }
    }
}