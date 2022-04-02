using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] float smoothSpeed;
    [SerializeField] Vector3 offset;
    [SerializeField] Transform target;

    bool isCinematicDone;

    void Start()
    {
        Vector3 offsetPlus = new Vector3(offset.x, offset.y + 5, offset.z - 5);
        transform.position = target.position + offsetPlus;
        StartCoroutine(Cinematic());
    }
    void FixedUpdate()
    {
        if (isCinematicDone)
        {
            Vector3 desiredPos = target.position + offset;
            Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
            transform.position = smoothedPos;
            transform.LookAt(target);
        }
    }

     IEnumerator Cinematic()
    {
        while (transform.position != target.position + offset)
        {
            Vector3 smoothedPosPlus = Vector3.Lerp(transform.position, target.position + offset, 1.5f * Time.deltaTime);
            transform.position = smoothedPosPlus;
            yield return null;
        }
        isCinematicDone = true;
    }
}
