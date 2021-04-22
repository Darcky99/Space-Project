using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrameRate : MonoBehaviour
{
    int frameCount = 0;
    float timeCount = 0.0f;
    float RefreshTime = 0.1f;
    
    public TextMeshProUGUI Text;

    private void Update()
    {
        if (timeCount < RefreshTime)
        {
            timeCount += Time.deltaTime;
            frameCount++;
        }
        else
        {
            float lastFramerate = frameCount / timeCount;
            frameCount = 0;
            timeCount = 0.0f;
            Text.text = lastFramerate.ToString("n2");
        }
    }
}
