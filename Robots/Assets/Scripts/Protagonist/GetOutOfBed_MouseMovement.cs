using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetOutOfBed_MouseMovement : MonoBehaviour
{
    [SerializeField] Animator lumiAnim;
    [SerializeField] float maxNoiseLevel = 1, movementNoise = .05f, noiseDecreaseRate = .5f;

    Animator roanAnim;
    float mouseMovement, noiseLevel;
    int jostles;
    bool canJostle = true;

    private void Start()
    {
        roanAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        mouseMovement = Mathf.Abs(Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y"));
        roanAnim.speed = mouseMovement;
        noiseLevel = mouseMovement;

        #region TODO - this noise level inication system is too bare-bones. we should replace it with one that allows the partner to react to consecutive disturbances
        if (noiseLevel >= maxNoiseLevel)
        {
            print("too loud!");
            if (canJostle)
            {
                jostles++;
                canJostle = false;
                Invoke("CoolDown", noiseDecreaseRate);
            }
            lumiAnim.SetInteger("Jostles", jostles);
        }

        //if (mouseMovement > 0) noiseLevel += movementNoise;
        //if (noiseLevel > 0) noiseLevel -= Time.deltaTime * noiseDecreaseRate;
        //else noiseLevel = 0;
        #endregion

        print(noiseLevel);
    }

    void CoolDown()
    {
        canJostle = true;
    }
}
