using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]

public class DaneMovement : MonoBehaviour
{
    public Transform[] points;
    public float speed = 500;
    public float visionConeAngle = 90;

    Rigidbody2D myRigidBody;
    Vector3 currentPos;    // won't work with Vector2
    bool isUsingPhysics;

    void Start()
    {
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().backgroundColor = new Color32(100, 149, 237, 255);
        myRigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        GetState();
        Move();
    }

    void GetState()
    {
        if (Input.GetKeyDown(KeyCode.Space)) isUsingPhysics = !isUsingPhysics;
    }

    void Move()
    {
        if (isUsingPhysics)
        {
            #region booooring rigidbody crap
            if (Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                myRigidBody.AddForce(Vector2.right * speed);

            if (Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                myRigidBody.AddForce(Vector2.left * speed);

            if (Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                myRigidBody.AddForce(Vector2.up * speed);

            if (Input.GetKey(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                myRigidBody.AddForce(Vector2.down * speed);
            #endregion
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                transform.position = FindClosestPoint(Vector2.right);

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                transform.position = FindClosestPoint(Vector2.left);

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                transform.position = FindClosestPoint(Vector2.up);

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                transform.position = FindClosestPoint(Vector2.down);
        }
    }

    Vector2 FindClosestPoint()
    {
        currentPos = transform.position;
        Transform closestPoint = null;
        float distanceToClosestPoint = Mathf.Infinity;

        for (int i = 0; i < points.Length; i++)
        {
            Vector2 pointDirection = points[i].position - currentPos;
            float distanceToPoint = Vector2.Distance(points[i].position, currentPos);

            if (distanceToPoint < distanceToClosestPoint)
            {
                closestPoint = points[i];
                distanceToClosestPoint = distanceToPoint;
            }
        }
        return closestPoint.position;
    }   // not used

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

            // these two bools are just to make the big important if statement within this loop read nicely
            bool pointIsWithinVisionConeAngle = visionConeAngle > pointAngle;
            bool pointIsClosestPoint = distanceToPoint < distanceToClosestPoint;

            if (pointIsClosestPoint && pointIsWithinVisionConeAngle)
            {
                #region new, snazzy logic
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
                #endregion

                #region original logic that didn't work
                //if (pointAngle < visionConeAngle)   // this is where I think my biggest logic error is
                //    closestPoint = points[i];
                //else closestPoint = points[Random.Range(0, points.Length)]; // HACK: to get around null reference exception when above if statement doesn't work
                //distanceToClosestPoint = distanceToPoint;
                #endregion
            }
        }

        if (closestPoint != null) return closestPoint.position;
        else return currentPos;
    }
}
