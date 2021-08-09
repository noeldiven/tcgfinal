using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_controller : MonoBehaviour
{
    public Transform[] views;
    public float transitionspeed;
    Transform currentviews;

    void Start()
    {
        currentviews = transform;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            currentviews = views[0];
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            currentviews = views[1];
        }
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, currentviews.position, Time.deltaTime * transitionspeed);
        Vector3 currentangle = new Vector3(
            Mathf.Lerp(transform.rotation.eulerAngles.x, currentviews.transform.rotation.eulerAngles.x, Time.deltaTime * transitionspeed),
            Mathf.Lerp(transform.rotation.eulerAngles.y, currentviews.transform.rotation.eulerAngles.y, Time.deltaTime * transitionspeed),
            Mathf.Lerp(transform.rotation.eulerAngles.z, currentviews.transform.rotation.eulerAngles.z, Time.deltaTime * transitionspeed)
            );
        transform.eulerAngles = currentangle;
    }
}
