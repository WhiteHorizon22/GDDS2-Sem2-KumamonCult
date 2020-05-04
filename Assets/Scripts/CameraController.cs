using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target; // The object that the camera will follow
    public float followAhead;

    private Vector3 targetPosition;

    public float smoothing;

    public float stageTop;
    public float stageBottom;

    public bool followTarget; //whether following our target or not

    // Start is called before the first frame update
    void Start()
    {
        followTarget = true; //at the start of the game, camera follows player
    }

    // Update is called once per frame
    void Update()
    {
        if (followTarget)
        {
            targetPosition = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
            if (target.transform.localScale.x > 0.0f)
            {
                targetPosition = new Vector3(targetPosition.x + followAhead, targetPosition.y, targetPosition.z);
            }
            else
            {
                targetPosition = new Vector3(targetPosition.x - followAhead, targetPosition.y, targetPosition.z);
            }
            //transform.position = targetPosition;
            if (target.transform.position.y > stageBottom && target.transform.position.y < stageTop)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.unscaledDeltaTime);
            }
            else
            {
                return;
            }
        }
    }
}
