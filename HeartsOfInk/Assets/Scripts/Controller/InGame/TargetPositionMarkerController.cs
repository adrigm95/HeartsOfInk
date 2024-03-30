using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPositionMarkerController : MonoBehaviour
{
    private const float AnimationDuration = 0.5f;
    private float startAnimation;
    private GameObject objectTarget;
    private bool animationEnabled;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimation();
        UpdatePosition();
    }

    public void SetTargetPosition(GameObject target, bool isTemporal)
    {
        Debug.Log("Start - SetTargetPosition - GameObject");
        if (target != null)
        {
            this.gameObject.SetActive(true);
            this.transform.position = target.transform.position;
            objectTarget = target;
        }
        SetTemporalAnimation(isTemporal);
    }

    public void SetTargetPosition(Vector3 targetPosition, bool isTemporal)
    {
        Debug.Log("Start - SetTargetPosition - Vector3");
        this.gameObject.SetActive(true);
        this.transform.position = targetPosition;
        SetTemporalAnimation(isTemporal);
    }

    public void RemoveTargetPosition()
    {
        if (!animationEnabled)
        {
            this.gameObject.SetActive(false);
            objectTarget = null;
        }
    }

    private void UpdateAnimation()
    {
        if (animationEnabled && (startAnimation < Time.realtimeSinceStartup - AnimationDuration))
        {
            Debug.Log("Target market animation Finished.");
            animationEnabled = false;
            this.gameObject.SetActive(false);
        }
    }

    private void UpdatePosition()
    {
        if (objectTarget != null)
        {
            this.transform.position = objectTarget.transform.position;
        }
    }

    private void SetTemporalAnimation(bool isTemporal)
    {
        if (isTemporal)
        {
            startAnimation = Time.realtimeSinceStartup;
        }

        animationEnabled = isTemporal;
    }
}
