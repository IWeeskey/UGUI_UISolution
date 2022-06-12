using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleTweaker : MonoBehaviour
{
    [Range(0.2f, 10f)]
    public float Timescale = 1f;

    private void Update()
    {
        Time.timeScale = Timescale;
    }
}
