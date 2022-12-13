using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayText : MonoBehaviour
{
    Player player;
    private void Start()
    {
        player = FindObjectOfType<Player>();
        UpdateText();
    }

    public void UpdateText()
    {
        TMP_Text[] text = GetComponentsInChildren<TMP_Text>();
        text[0].text = text[0].gameObject.name + ": " + player.jumpCount;
        text[1].text = text[1].gameObject.name + ": " + player.dashCount;
        text[2].text = text[2].gameObject.name + ": " + player.wallClimbCount;
    }
}
