using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { CUTSCENE, GAME, WON }

public class GetOutOfBed_MouseMovement : MonoBehaviour
{
    public delegate void Won();
    public static event Won OnWon;

    [SerializeField] Animator lumiAnim;
    [SerializeField] float maxNoiseLevel = 1, movementNoise = .05f, noiseDecreaseRate = .5f;

    Animator roanAnim;
    GameState currentGameState = GameState.CUTSCENE;
    float mouseMovement, noiseLevel;
    int jostles;
    bool canJostle = true;

    private void OnEnable()
    {
        DreamText.OnStartGame += WakeUp;
    }
    private void OnDisable()
    {
        DreamText.OnStartGame -= WakeUp;
    }

    private void Start()
    {
        roanAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (currentGameState == GameState.GAME) UpdatePlayerMovement();
        else roanAnim.speed = 0;
    }

    void UpdatePlayerMovement()
    {
        mouseMovement = Mathf.Abs(Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y"));
        roanAnim.speed = mouseMovement;
        noiseLevel = mouseMovement;

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

        if (roanAnim.GetCurrentAnimatorStateInfo(0).IsName("Roan_OutOfBed"))
        {
            currentGameState = GameState.WON;
            if (OnWon != null) OnWon();
        }
    }

    void CoolDown()
    {
        canJostle = true;
    }

    void WakeUp()
    {
        currentGameState = GameState.GAME;
    }
}
