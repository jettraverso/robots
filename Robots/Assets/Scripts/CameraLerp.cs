using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLerp : MonoBehaviour
{
    [SerializeField] Transform objectToFollow;
    [SerializeField] float lerpSpeed = 2;

    private void Update()
    {
        Vector3 newPosition = new Vector3(objectToFollow.position.x, objectToFollow.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, newPosition, lerpSpeed * Time.deltaTime);
    }
}
