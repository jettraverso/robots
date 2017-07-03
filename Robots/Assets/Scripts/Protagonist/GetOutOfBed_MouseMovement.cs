using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetOutOfBed_MouseMovement : MonoBehaviour
{
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        anim.speed = Mathf.Abs(Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y"));
    }
}
