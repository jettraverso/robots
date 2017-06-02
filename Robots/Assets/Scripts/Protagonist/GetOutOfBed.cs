using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { CUTSCENE, GAME, WON }

public class GetOutOfBed : MonoBehaviour
{
    public delegate void Won();
    public static event Won OnWon;

    [SerializeField] Transform[] points;
    [SerializeField] float speed = 500, visionConeAngle = 90, maxNoiseLevel = 1, movementNoise = .5f, noiseDecreaseRate = .5f;
    [SerializeField] Animator partnerAnimator;

    List<Transform> visitedPoints = new List<Transform>();

    Animator myAnim;
    GameState currentGameState = GameState.CUTSCENE;
    Vector3 currentPos, targetPos;

    float noiseLevel;

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
        myAnim = GetComponent<Animator>();
        currentPos = transform.position;
        targetPos = currentPos;

        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // search for a node which is close by
        // if you move in the direction of the node, change position and animation state
        // if you've been to the node, take it out of the pool ("positions" array)

        transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);

        //print(noiseLevel);

        #region TODO - this noise level inication system is too bare-bones. we should replace it with one that allows the partner to react to consecutive disturbances
        if (noiseLevel >= maxNoiseLevel)
        {
            print("too loud!");
            partnerAnimator.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            Invoke("ChangePartnerColorBack", .5f);
        }
        else partnerAnimator.gameObject.GetComponent<SpriteRenderer>().color = Color.white;

        if (noiseLevel > 0) noiseLevel -= Time.deltaTime * noiseDecreaseRate;
        else noiseLevel = 0;
        #endregion

        if (currentGameState == GameState.GAME)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                targetPos = FindClosestPoint(Vector2.right);
                noiseLevel += movementNoise;
            }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                targetPos = FindClosestPoint(Vector2.left);
                noiseLevel += movementNoise;
            }

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                targetPos = FindClosestPoint(Vector2.up);
                noiseLevel += movementNoise;
            }

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                targetPos = FindClosestPoint(Vector2.down);
                noiseLevel += movementNoise;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            if (OnWon != null) OnWon();
            currentGameState = GameState.WON;
            GetComponent<SpriteRenderer>().color = Color.green;
            print("you're out of bed");
        }
    }

    void WakeUp()
    {
        GetComponent<SpriteRenderer>().color = Color.yellow;
        currentGameState = GameState.GAME;
    }

    Vector2 FindClosestPoint(Vector2 movementDirection)
    {
        currentPos = transform.position;
        Transform closestPoint = null;                  // temporarily null
        float distanceToClosestPoint = Mathf.Infinity;  // temporarily infinite

        for (int i = 0; i < points.Length; i++)
        {
            Vector2 pointDirection = points[i].position - currentPos;
            float distanceToPoint = Vector2.Distance(points[i].position, currentPos);
            float pointAngle = Vector2.Angle(pointDirection, movementDirection);
            
            bool pointIsClosestPoint = distanceToPoint < distanceToClosestPoint;
            bool pointIsWithinVisionConeAngle = visionConeAngle > pointAngle;

            if (pointIsClosestPoint && pointIsWithinVisionConeAngle && !visitedPoints.Contains(points[i]))
            {
                if (closestPoint != null)
                {
                    // if closestPoint is not null, compare to see if this new one is closer
                    bool newPointIsCloserThanPreviousClosest =
                        Vector2.Distance(transform.position, points[i].position) <
                        Vector2.Distance(transform.position, closestPoint.position);

                    if (newPointIsCloserThanPreviousClosest)
                        closestPoint = points[i];
                }
                else closestPoint = points[i];
            }
        }

        if (closestPoint != null)
        {
            visitedPoints.Add(closestPoint);
            return closestPoint.position;
        }
        else return currentPos;
    }
}
