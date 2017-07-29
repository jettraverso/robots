using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamText_02 : MonoBehaviour
{
    [SerializeField] List<Transform> dreamTextNodes;
    [SerializeField] float movementSpeed = 2, lerpSpeed = 4, range = 2,
        mouseMovementThreshold = .5f, disappearTimer = 1, coolDownTime = 1, focalNodeZValue = 0, closestNodeProximity = .25f;
    [SerializeField] Color outOfFocusColor = Color.gray, InFocusColor = Color.white;

    Transform nodeBeingFocusedOn, nodeToDisappear;
    Vector3 currentPosition, mousePosition;
    float mouseMovement;
    bool canControlMovement = true, canFadeText;

    private void Start()
    {
        foreach (Transform t in dreamTextNodes) t.GetComponent<TextMesh>().color = outOfFocusColor;
        nodeBeingFocusedOn = dreamTextNodes[0];
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        UpdateMovement();
        UpdateFocalNodeEffects();
    }

    void UpdateMovement()
    {
        mouseMovement = Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y");
        mousePosition += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);

        if (canControlMovement)
        {
            transform.position = Vector3.Lerp(transform.position, mousePosition, movementSpeed * Time.deltaTime);

            if (mouseMovement > mouseMovementThreshold)
            {
                if (Vector3.Distance(transform.position, FindClosestNode().position) < range)
                {
                    canControlMovement = false;
                }

                if (canFadeText) StartCoroutine(DeactivateNode(nodeToDisappear));
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, FindClosestNode().position, lerpSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, FindClosestNode().position) < closestNodeProximity)
            {
                nodeBeingFocusedOn = FindClosestNode().transform;
                Invoke("CoolDown", coolDownTime);
            }
        }
    }

    void UpdateFocalNodeEffects()
    {
        // TODO make text appear sharper, clearer (remove whatever visual effects we put on it)

        Vector3 newNodePosition = new Vector3(nodeBeingFocusedOn.position.x, nodeBeingFocusedOn.position.y, 0);
        nodeBeingFocusedOn.transform.position = Vector3.Lerp(nodeBeingFocusedOn.transform.position, newNodePosition, lerpSpeed * Time.deltaTime);
        nodeBeingFocusedOn.GetComponent<TextMesh>().color = Color.Lerp(nodeBeingFocusedOn.GetComponent<TextMesh>().color, InFocusColor, lerpSpeed * Time.deltaTime);
    }

    void CoolDown()
    {
        nodeToDisappear = nodeBeingFocusedOn;
        dreamTextNodes.Remove(nodeBeingFocusedOn);
        mousePosition = Vector3.zero;
        canControlMovement = true;
        canFadeText = true;
    }

    Transform FindClosestNode()
    {
        currentPosition = transform.position;
        Transform closestNode = null;
        float distanceToClosestNode = Mathf.Infinity;

        for (int i = 0; i < dreamTextNodes.Count; i++)
        {
            Vector2 NodeDirection = dreamTextNodes[i].position - currentPosition;
            float distanceToNode = Vector2.Distance(dreamTextNodes[i].position, currentPosition);

            if (distanceToNode < distanceToClosestNode)
            {
                closestNode = dreamTextNodes[i];
                distanceToClosestNode = distanceToNode;
            }
        }
        return closestNode;
    }

    IEnumerator DeactivateNode(Transform previousNode)
    {
        canFadeText = false;

        TextMesh nodeText = previousNode.GetComponent<TextMesh>();
        Color originalColor = nodeText.color;

        float elapsedTime = 0;
        while (elapsedTime < disappearTimer)
        {
            nodeText.color = Color.Lerp(originalColor, Color.clear, elapsedTime / disappearTimer);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        previousNode.gameObject.SetActive(false);
        dreamTextNodes.Remove(previousNode);
    }
}
