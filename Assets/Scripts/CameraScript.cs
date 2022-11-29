using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Camera camera;
    private float moveTimer = 0.0f;
    private float refVelocity;
    private int currentPosition;
    public CameraPosition[] positions;

    [System.Serializable]
    public class CameraPosition
    {
        public Vector3 pos;
        public float size;
    }


    public void AdvancePostion()
    {
        if (currentPosition + 1 < positions.Length)
        {
            currentPosition += 1;
            moveTimer = 0.2f;
            Debug.Log(currentPosition);
        }   
        
        else
        {
            Debug.Log("Error: Out of Scope");
        }
    }

    public void RetreatPosition()
    {
        if (currentPosition - 1 > -1)
        {
            currentPosition -= 1;
            moveTimer = 0.2f;
            Debug.Log(currentPosition);
        }

        else
        {
            Debug.Log("Error: Out of Scope");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentPosition = 0;
        camera = this.GetComponent<Camera>();
        camera.transform.position = positions[currentPosition].pos;
        camera.orthographicSize = positions[currentPosition].size;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            AdvancePostion();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            RetreatPosition();
        }

        if (moveTimer > 0.0f)
        {
            moveTimer -= Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, positions[currentPosition].pos, 0.05f);
            camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, positions[currentPosition].size, ref refVelocity, moveTimer);
            if (moveTimer <= 0)
            {
                camera.transform.position = positions[currentPosition].pos;
                camera.orthographicSize = positions[currentPosition].size;
            }
        }
    }
}
