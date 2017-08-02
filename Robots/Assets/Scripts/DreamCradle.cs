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
        //Debug.Log(output.Text);
        //StoryText.text = output.Text;

        if (output is StoryText)
        {
            StoryText.text = output.Text;
        }
        else if (output is StoryLink)
        {
            LinkText.text = output.Text;
        }
        else if (output is LineBreak)
        {

        }
        else if (output is OutputGroup)
        {

        }
    }
}
