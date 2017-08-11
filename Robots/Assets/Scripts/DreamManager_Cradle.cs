﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cradle;

public class DreamManager_Cradle : MonoBehaviour
{
    [SerializeField] Story story;
    [SerializeField] List<TextMesh> dreamTextNodes;
    [SerializeField] float movementSpeed = .5f, lerpSpeed = 2, range = 8,
        mouseMovementDeadZone = .2f, disappearTimer = 2, coolDownTime = 1,
        focalNodeZValue = 0, closestNodeProximity = .25f;
    [SerializeField] Color outOfFocusColor = Color.gray, InFocusColor = Color.white;

    Transform nodeBeingFocusedOn, nodeToDisappear;
    Vector3 currentPosition, mousePosition;
    float mouseMovement;
    bool canControlMovement = true, canFadeText;

    private void Start()
    {
        // all nodes start as vague and blurry/blotchy shapes
        foreach (TextMesh t in dreamTextNodes) t.color = outOfFocusColor;

        // except the first node in the list
        nodeBeingFocusedOn = dreamTextNodes[0].transform;

        // get that dang cursor out of my face
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // boilerplate Cradle event for making stuff happen when text is processed
        story.OnOutput += story_OnOutput;

        story.Begin();
    }

    private void Update()
    {
        UpdateMovement();
        UpdateFocalNodeEffects();
    }

    void story_OnOutput(StoryOutput output)
    {
        List<StoryLink> currentLinks = new List<StoryLink>();
        foreach (StoryLink o in story.GetCurrentLinks())
        {
            currentLinks.Add(o);
        }

            // print the current story text to the node being focused on
            if (output is StoryText) nodeBeingFocusedOn.GetComponent<TextMesh>().text = output.Text;

        // print the same link text to every other node
        // TODO this treats all links as the same - we need to differentiate between the links,
        // possibly with a switch statement that checks for the link's name or ID
        // (the function story.GetPassage and the dictionary story.Passages weren't working for me
        // when I tried to fill each node's text field with the text of an individual passage)
        else if (output is StoryLink)
        {
            for (int i = 0; i < currentLinks.Count; i++)
            {
                dreamTextNodes[i + 1].text = currentLinks[i].Text;
            }

            //foreach (TextMesh t in dreamTextNodes)
            //{

            //        //if (t.transform != nodeBeingFocusedOn)
            //        //{
            //        //    t.text = output.Text;
            //        //}
            //}

            //Cycle through available links -> foreach storyLink
            //foreach (StoryOutput o in story.GetCurrentLinks())
            //{
            //    //proof that o is all current links
            //    Debug.Log(o.Text);

            //    //attach to text mesh

            //}

        }
    }

    void UpdateMovement()
    {
        mouseMovement = Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y");
        mousePosition += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);

        // if the player can move the camera around with the mouse
        if (canControlMovement)
        {
            // move the dream manager (the camera lerps to the position of the dream manager's x and z axes)
            transform.position = Vector3.Lerp(transform.position, mousePosition, movementSpeed * Time.deltaTime);

            // this if statement gives the mouse a dead zone, defined by the mouseMovementDeadZone inspector variable
            if (mouseMovement > mouseMovementDeadZone)
            {
                // if the player comes in range of a node
                if (Vector3.Distance(transform.position, FindClosestNode().position) < range)
                    canControlMovement = false;

                // if the player is leaving a node behind, deactivate it
                if (canFadeText) StartCoroutine(DeactivateNode(nodeToDisappear));
            }
        }

        // if the camera is focused on a text node
        else
        {
            // lerp to the node
            transform.position = Vector3.Lerp(transform.position, FindClosestNode().position, lerpSpeed * Time.deltaTime);

            // if the node is very close, bring it into focus
            if (Vector3.Distance(transform.position, FindClosestNode().position) < closestNodeProximity)
                FocusOnText();
        }
    }

    void UpdateFocalNodeEffects()
    {
        // the new node position is the same as its previous position, but with a z axis value of 0
        Vector3 newNodePosition = new Vector3(nodeBeingFocusedOn.position.x, nodeBeingFocusedOn.position.y, 0);

        // move the node to 0 on the z axis, so the player can get a good, clear look at it
        nodeBeingFocusedOn.transform.position = Vector3.Lerp(nodeBeingFocusedOn.transform.position, newNodePosition, lerpSpeed * Time.deltaTime);

        // change the node's color to be clearer and easier to read (placeholder effect)
        nodeBeingFocusedOn.GetComponent<TextMesh>().color = Color.Lerp(nodeBeingFocusedOn.GetComponent<TextMesh>().color, InFocusColor, lerpSpeed * Time.deltaTime);
    }

    void FocusOnText()
    {
        // the node being focused on is the closest one to this gameObject
        nodeBeingFocusedOn = FindClosestNode().transform;

        // when the node is being focused on, that is the same as clicking on its link in Twine
        string nodeText = nodeBeingFocusedOn.GetComponent<TextMesh>().text;
        if (story.HasLink(nodeText)) story.DoLink(nodeText);

        // give the player enough time to stop moving the mouse, so that they can read the passage
        // without accidentally skipping it
        Invoke("CoolDown", coolDownTime);
    }

    void CoolDown()
    {
        // the current node being focused on is next in line for deletion, as soon as the player mouses away
        nodeToDisappear = nodeBeingFocusedOn;

        // it is no longer part of the available nodes to browse through
        dreamTextNodes.Remove(nodeBeingFocusedOn.GetComponent<TextMesh>());

        // reset mouse position to this new node, so that the camera can move away from it easily
        mousePosition = Vector3.zero;

        // the player can now control the camera movement again
        canControlMovement = true;

        // the text on this node will fade out when the player mouses away
        canFadeText = true;
    }

    Transform FindClosestNode()
    {
        // I'm not going to comment this function right now, for brevity -
        // its purpose is very straightforward, it simply locates the node closest to this gameObject

        currentPosition = transform.position;
        Transform closestNode = null;
        float distanceToClosestNode = Mathf.Infinity;

        for (int i = 0; i < dreamTextNodes.Count; i++)
        {
            Vector2 NodeDirection = dreamTextNodes[i].transform.position - currentPosition;
            float distanceToNode = Vector2.Distance(dreamTextNodes[i].transform.position, currentPosition);

            if (distanceToNode < distanceToClosestNode)
            {
                closestNode = dreamTextNodes[i].transform;
                distanceToClosestNode = distanceToNode;
            }
        }
        return closestNode;
    }

    IEnumerator DeactivateNode(Transform previousNode)
    {
        // the coroutine cannot be called more than once per node
        canFadeText = false;

        TextMesh nodeText = previousNode.GetComponent<TextMesh>();
        Color originalColor = nodeText.color;

        float elapsedTime = 0;
        while (elapsedTime < disappearTimer)
        {
            // darken the node
            nodeText.color = Color.Lerp(originalColor, Color.clear, elapsedTime / disappearTimer);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // disable the node
        previousNode.gameObject.SetActive(false);

        // remove the node from the pool of interactable nodes
        dreamTextNodes.Remove(previousNode.GetComponent<TextMesh>());
    }
}