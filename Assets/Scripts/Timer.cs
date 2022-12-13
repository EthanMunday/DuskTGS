using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public bool stopTimer = false;
    TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!stopTimer)
        {
            text.text = (Mathf.Round(Time.timeSinceLevelLoad * 100) / 100).ToString("0.0");
        }
    }

    public void FinishTime()
    {
        text.text = "You beat the game in " + (Mathf.Round(Time.timeSinceLevelLoad * 100) / 100).ToString("0.0") + " seconds.";
    }
}
