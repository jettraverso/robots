﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cradle;
using System;

public class DreamManager_Cradle : MonoBehaviour
{
    [SerializeField] Story story;
    [SerializeField] List<TextMesh> dreamTextNodes;
    [SerializeField] float movementSpeed = .5f, lerpSpeed = 2, range = 8,
        mouseMovementDeadZone = .2f, disappearTimer = 2, coolDownTime = 1,
        focalNodeZValue = 0, closestNodeProximity = .25f, fadeOutTime = 3;
    [SerializeField] Color outOfFocusColor = Color.gray, InFocusColor = Color.white;

    Transform nodeBeingFocusedOn, nodeToDisappear;
    Vector3 mousePosition;
    List<StoryLink> links = new List<StoryLink>();
    float mouseMovement;
    int focalNodeIndex, counter;
    bool canControlMovement = true, canFadeText, dreamComplete;

    private void Start()
    {
        for (int i = 0; i < dreamTextNodes.Count; i++)
        {
            // all nodes start as vague and blurry/blotchy shapes
            dreamTextNodes[i].color = outOfFocusColor;

            // all nodes are populated with text from the story
            dreamTextNodes[i].text = "*";
        }

        // except the first node in the list
        nodeBeingFocusedOn = dreamTextNodes[0].transform;

        // get that dang cursor out of my face
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // boilerplate Cradle event for making stuff happen when text is processed
        story.OnOutput += story_OnOutput;

        story.OnPassageEnter += passage_Enter;

        story.Begin();
    }

    private void Update()
    {
        UpdateMovement();
        UpdateFocalNodeEffects();
    }

    void story_OnOutput(StoryOutput output)
    {
        //dreamTextNodes[focalNodeIndex].text = "";
        dreamTextNodes[focalNodeIndex].text += "\n" + output.Text;

        links.Clear();
        foreach (StoryLink o in story.GetCurrentLinks())
        {
            links.Add(o);
            if(links.Count > 1)
            {
                dreamTextNodes[focalNodeIndex + 2].text = o.Text;
            }
            if (dreamTextNodes[focalNodeIndex + 1].text == "*")
                dreamTextNodes[focalNodeIndex + 1].text = o.Text;
            //else if (dreamTextNodes[focalNodeIndex + 2].text == "*")
            //    dreamTextNodes[focalNodeIndex + 2].text = o.Text;
        }
    }

    void passage_Enter(StoryPassage passage)
    {
        nodeBeingFocusedOn.GetComponent<TextMesh>().text = "";
    }

    void UpdateMovement()
    {
        mouseMovement = Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y");
        mousePosition += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);

        // if the player can move the camera around with the mouse
        if (canControlMovement && !dreamComplete)
        {
            // move the dream manager (the camera lerps to the position of the dream manager's x and z axes)
            transform.position = Vector3.Lerp(transform.position, mousePosition, movementSpeed * Time.deltaTime);

            // this if statement gives the mouse a dead zone, defined by the mouseMovementDeadZone inspector variable
            if (mouseMovement > mouseMovementDeadZone)
            {
                // if the player comes in range of a node and the player won't be skipping ahead in the story,
                // lock movement
                if (Vector3.Distance(transform.position, FindClosestNode().position) < range &&
                ClosestNodeIsNextNodeInStory())
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

        for (int i = 0; i < dreamTextNodes.Count; i++)
            if (nodeBeingFocusedOn.GetComponent<TextMesh>() == dreamTextNodes[i]) focalNodeIndex = i;

        // when the node is being focused on, that is the same as clicking on its link in Twine
        // TODO this is problematic, because it changes the link text when the node is focused on
        string nodeText = nodeBeingFocusedOn.GetComponent<TextMesh>().text;
        if (story.HasLink(nodeText))
        {
            story.DoLink(nodeText);
        }

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

    bool ClosestNodeIsNextNodeInStory()
    {
        TextMesh closestNodeText = FindClosestNode().GetComponent<TextMesh>();
        
        foreach (StoryLink o in links)
        {
            if (links.Count == 2)
            {
                if (closestNodeText == dreamTextNodes[0] || closestNodeText == dreamTextNodes[1])
                {
                    if (closestNodeText == dreamTextNodes[0]) StartCoroutine(DeactivateNode(dreamTextNodes[1].transform));
                    if (closestNodeText == dreamTextNodes[1]) StartCoroutine(DeactivateNode(dreamTextNodes[0].transform));

                    return true;
                }
                else return false;
            }
            else if (links.Count == 1)
            {
                if (closestNodeText == dreamTextNodes[0]) return true;
                else return false;
            }
            else
            {
                Debug.Log("WHOOPS TOO MANY NODES");
                return false;
            }
        }

        return false;
    }

    Transform FindClosestNode()
    {
        // I'm not going to comment this function right now, for brevity -
        // its purpose is very straightforward, it simply locates the node closest to this gameObject

        Vector2 currentPosition = transform.position;
        Transform closestNode = null;
        float distanceToClosestNode = Mathf.Infinity;

        for (int i = 0; i < dreamTextNodes.Count; i++)
        {
            Vector2 nodePosition = new Vector2(dreamTextNodes[i].transform.position.x, dreamTextNodes[i].transform.position.y);

            Vector2 NodeDirection = nodePosition - currentPosition;
            float distanceToNode = Vector2.Distance(nodePosition, currentPosition);

            if (distanceToNode < distanceToClosestNode)
            {
                closestNode = dreamTextNodes[i].transform;
                distanceToClosestNode = distanceToNode;
            }
        }
        if (closestNode != null) return closestNode;
        else
        {
            // if there are no more nodes left, change scene
            StartCoroutine(ChangeScene());
            return nodeToDisappear;
        }
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

    IEnumerator ChangeScene()
    {
        dreamComplete = true;

        // wait for node to disappear
        yield return new WaitForSeconds(disappearTimer);

        // change scene
        SceneManager.LoadScene("WakeUp_Day1");
    }
}
