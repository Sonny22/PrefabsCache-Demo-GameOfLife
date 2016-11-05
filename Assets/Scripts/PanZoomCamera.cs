using UnityEngine;
using System.Collections;
using System;

public class PanZoomCamera : MonoBehaviour 
{
    public float panMultiplier = 1.5f;
    public float zoomMultiplier = 1.5f;

    private bool isPaning = false;

	private Camera thisCamera;

    private Vector3 startPanPos;
    private Vector3 cameraPreviousPos;

    private float verticalExtent;
    private float horizontalExtent;

    private Vector3 velocity = Vector3.zero;
    private Vector3 target;

	void Start () 
	{
		thisCamera = GetComponent<Camera> ();
		verticalExtent = thisCamera.orthographicSize;
        horizontalExtent = verticalExtent * Screen.width / Screen.height;
        transform.position = new Vector3(horizontalExtent/2f, verticalExtent, -10f);
        target = transform.position;
	}
	
	void Update ()
    {
        // Zoom
        if (Input.mouseScrollDelta.y != 0f)
        {
			thisCamera.orthographicSize += zoomMultiplier * Input.mouseScrollDelta.y * -1f;
        }

        // Pan based on current zoom
		verticalExtent = thisCamera.orthographicSize;
        horizontalExtent = verticalExtent * Screen.width / Screen.height;

        if (isPaning == false && Input.GetMouseButtonDown(1))
        {
            isPaning = true;
            startPanPos = Input.mousePosition;
            cameraPreviousPos = transform.position;
        }
        else if (isPaning && Input.GetMouseButton(1))
        {
            Vector3 delta = (startPanPos - Input.mousePosition);
            delta.x = delta.x / Screen.width * horizontalExtent;
            delta.y = delta.y / Screen.height * verticalExtent;

            target = cameraPreviousPos + delta * panMultiplier;
        }
        else if (isPaning && Input.GetMouseButtonUp(1))
        {
            isPaning = false;

            Vector3 delta = (startPanPos - Input.mousePosition);
            delta.x = delta.x / Screen.width * horizontalExtent;
            delta.y = delta.y / Screen.height * verticalExtent;

            target = cameraPreviousPos + delta * panMultiplier;
        }
    }

    void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, 0.05f);
	}
}
