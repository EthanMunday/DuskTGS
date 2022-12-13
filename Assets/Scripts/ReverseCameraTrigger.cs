using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseCameraTrigger : MonoBehaviour
{
    private bool isActive = false;
    public float addedSpeed;
    public int addedJumps;
    public int addedWallClimbs;
    public int addedDashes;


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            Debug.Log(isActive);
            if (!isActive && collision.transform.position.x < transform.position.x)
            {
                isActive = true;
                FindObjectOfType<CameraScript>().AdvancePostion();
                Player player = FindObjectOfType<Player>();
                player.AddSpeed(addedSpeed);
                player.AddJumps(addedJumps);
                player.AddWallClimbs(addedWallClimbs);
                player.AddDashes(addedDashes);
                FindObjectOfType<DisplayText>().UpdateText();

            }

            else if (isActive && collision.transform.position.x > transform.position.x)
            {
                isActive = false;
                FindObjectOfType<CameraScript>().RetreatPosition();
                Player player = FindObjectOfType<Player>();
                player.MinusSpeed(addedSpeed);
                player.MinusJumps(addedJumps);
                player.MinusWallClimbs(addedWallClimbs);
                player.MinusDashes(addedDashes);
                FindObjectOfType<DisplayText>().UpdateText();
            }
        }
    }
}
