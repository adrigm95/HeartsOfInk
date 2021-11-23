using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSMetrics : MonoBehaviour
{
    // Average FPS.
    private float totalFPSCount;
    private float totalFPSSum;

    private float highFPSValue;
    private float lowFPSValue;

    public float CurrentFPS;
    public float AverageFPS;
    public float HighFPSValue;
    public float LowFPSValue;
    public float MinFPSExpected = 30;
    public string LastFPSFall;
    public List<string> FPSFalls;
    

    // Start is called before the first frame update
    void Start()
    {
        totalFPSCount = 0;
        totalFPSSum = 0;
        highFPSValue = 0;
        lowFPSValue = float.MaxValue;
        FPSFalls = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        CurrentFPS = 1 / Time.deltaTime;

        totalFPSSum += CurrentFPS;
        totalFPSCount += 1;

        if (CurrentFPS > highFPSValue)
        {
            highFPSValue = CurrentFPS;
        }

        if (CurrentFPS < lowFPSValue)
        {
            lowFPSValue = CurrentFPS;
        }

        HighFPSValue = highFPSValue;
        LowFPSValue = lowFPSValue;
        AverageFPS = totalFPSSum / totalFPSCount;

        if (CurrentFPS < MinFPSExpected)
        {
            LastFPSFall = "FPS: " + CurrentFPS + "; Time: " + Time.time;
            FPSFalls.Add(LastFPSFall);
        }
    }
}
