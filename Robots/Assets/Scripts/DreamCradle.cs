using Cradle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DreamCradle : MonoBehaviour
{
    [SerializeField] Story story;
    [SerializeField] Text StoryText, LinkText;

    private void Start()
    {
        story.OnOutput += story_OnOutput;
        story.Begin();
    }

    void story_OnOutput(StoryOutput output)
    {
        if (output is StoryText) StoryText.text = output.Text;
        else if (output is StoryLink) LinkText.text = output.Text;
    }

    public void OnButtonClicked()
    {
        story.DoLink(LinkText.text);
    }
}
