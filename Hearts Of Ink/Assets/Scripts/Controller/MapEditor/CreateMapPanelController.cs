using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMapPanelController : MonoBehaviour
{
    [SerializeField]
    private GameObject rightPanelController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnablePanel()
    {
        this.gameObject.SetActive(true);
        rightPanelController.SetActive(false);
    }

    public void CancelCreation()
    {
        this.gameObject.SetActive(false);
        rightPanelController.SetActive(true);
    }

    public void CreateMap()
    {

    }
}
