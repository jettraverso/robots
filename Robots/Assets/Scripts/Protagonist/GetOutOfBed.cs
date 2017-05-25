using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetOutOfBed : MonoBehaviour
{
    [SerializeField] Transform[] points;
    [SerializeField] float speed = 500, visionConeAngle = 90, maxNoiseLevel = 1, movementNoise = .5f;

    List<Transform> visitedPoints = new List<Transform>();

    Animator myAnim;
    Vector3 currentPos;

    float noiseLevel;

    private void Start()
    {
        myAnim = GetComponent<Animator>();
        currentPos = transform.position;
    }

    private void Update()
    {
        // search for a node which is close by
        // if you move in the direction of the node, change position and animation state
        // if you've been to the node, take it out of the pool ("positions" array)

        print(noiseLevel);
        if (noiseLevel >= maxNoiseLevel) print("too loud!");
        if (noiseLevel > 0) noiseLevel -= Time.deltaTime;
        else noiseLevel = 0;

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position = FindClosestPoint(Vector2.right);
            noiseLevel += movementNoise;
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position = FindClosestPoint(Vector2.left);
            noiseLevel += movementNoise;
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.position = FindClosestPoint(Vector2.up);
            noiseLevel += movementNoise;
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.position = FindClosestPoint(Vector2.down);
            noiseLevel += movementNoise;
        }
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
