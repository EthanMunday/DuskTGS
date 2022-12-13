using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            Timer timer = FindObjectOfType<Timer>();
            timer.stopTimer = true;
            timer.FinishTime();

        }
    }
}
