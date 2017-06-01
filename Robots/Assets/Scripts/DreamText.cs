﻿using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DreamText : MonoBehaviour
{
    public delegate void StartGame();
    public static event StartGame OnStartGame;

    [SerializeField] TextAsset textToParse;
    [SerializeField] string[] dream01Lines, bootingUpLines, endOfDemoLines; // TODO - delete, replace with info from text file
    [SerializeField] float timeBetweenLetters = .01f, fadeInTimer = 10; // TODO - delete timeBetweenLetters, replace with info from text file
    [SerializeField] Image panelImage;
    [SerializeField] AnimationCurve fadeCurve;

    StreamReader reader;
    Text myText;
    Color textColor;

    private void OnEnable()
    {
        GetOutOfBed.OnWon += CallFadeOut;
    }
    private void OnDisable()
    {
        GetOutOfBed.OnWon -= CallFadeOut;
    }

    private void Start()
    {
        reader = new StreamReader(textToParse.name);
        // use reader to split the text file into a string array

        myText = GetComponent<Text>();
        textColor = myText.color;

        StartCoroutine(DreamCutscene01());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopAllCoroutines();
            myText.text = "";
            StartCoroutine(Fade(true));
        }
    }

    void CallFadeOut()
    {
        fadeInTimer = 5;
        timeBetweenLetters *= 2;
        StartCoroutine(Fade(false));
        StartCoroutine(EndOfDemoCutscene());
    }

    IEnumerator DreamCutscene01()
    {
        yield return new WaitForSeconds(1);

        // TODO - this sequence could be abbreviated into some kind of for/foreach loop,
        // if there were a system for associating a wait time with each string in lines
        // (since wait time will not always be the same, like it is here)
        
        StartCoroutine(ScrollText(dream01Lines, 0));
        yield return new WaitForSeconds(3);
        
        StartCoroutine(ScrollText(dream01Lines, 1));
        yield return new WaitForSeconds(5);
        
        StartCoroutine(ScrollText(dream01Lines, 2));
        yield return new WaitForSeconds(5);
        
        StartCoroutine(ScrollText(dream01Lines, 3));
        yield return new WaitForSeconds(5);
        
        StartCoroutine(ScrollText(dream01Lines, 4));
        yield return new WaitForSeconds(5);
        
        StartCoroutine(ScrollText(dream01Lines, 5));
        yield return new WaitForSeconds(3);

        StartCoroutine(ScrollText(dream01Lines, 6));
        yield return new WaitForSeconds(5);

        StartCoroutine(ScrollText(dream01Lines, 7));
        yield return new WaitForSeconds(5);

        #region delete lines one at a time. TODO - refactor
        myText.text = ">  " + dream01Lines[0] + "\n\n>  " + dream01Lines[1] + "\n\n>  " + dream01Lines[2] + "\n\n>  " + dream01Lines[3] + "\n\n>  " + dream01Lines[4] + "\n\n>  " + dream01Lines[5] + "\n\n>  " + dream01Lines[6];
        yield return new WaitForSeconds(1);
        myText.text = ">  " + dream01Lines[0] + "\n\n>  " + dream01Lines[1] + "\n\n>  " + dream01Lines[2] + "\n\n>  " + dream01Lines[3] + "\n\n>  " + dream01Lines[4] + "\n\n>  " + dream01Lines[5];
        yield return new WaitForSeconds(1);
        myText.text = ">  " + dream01Lines[0] + "\n\n>  " + dream01Lines[1] + "\n\n>  " + dream01Lines[2] + "\n\n>  " + dream01Lines[3] + "\n\n>  " + dream01Lines[4];
        yield return new WaitForSeconds(1);
        myText.text = ">  " + dream01Lines[0] + "\n\n>  " + dream01Lines[1] + "\n\n>  " + dream01Lines[2] + "\n\n>  " + dream01Lines[3];
        yield return new WaitForSeconds(1);
        myText.text = ">  " + dream01Lines[0] + "\n\n>  " + dream01Lines[1] + "\n\n>  " + dream01Lines[2];
        yield return new WaitForSeconds(1);
        myText.text = ">  " + dream01Lines[0] + "\n\n>  " + dream01Lines[1];
        yield return new WaitForSeconds(1);
        myText.text = ">  " + dream01Lines[0];
        yield return new WaitForSeconds(1);
        myText.text = ">  ";
        yield return new WaitForSeconds(2);
        myText.text = "";
        #endregion

        yield return new WaitForSeconds(4);

        #region boot up
        myText.text += bootingUpLines[0];
        myText.text += "\n\n";
        yield return new WaitForSeconds(5);
        myText.text += bootingUpLines[1];
        myText.text += "\n\n";
        yield return new WaitForSeconds(1);
        myText.text += bootingUpLines[2];
        myText.text += "\n\n";
        yield return new WaitForSeconds(1);
        myText.text += bootingUpLines[3];
        myText.text += "\n\n";
        yield return new WaitForSeconds(1);
        myText.text += bootingUpLines[4];
        myText.text += "\n\n";
        yield return new WaitForSeconds(3);
        myText.text += bootingUpLines[5];
        myText.text += "\n";
        yield return new WaitForSeconds(.1f);
        myText.text += bootingUpLines[6];
        myText.text += "\n";
        yield return new WaitForSeconds(.1f);
        myText.text += bootingUpLines[7];
        myText.text += "\n";
        yield return new WaitForSeconds(.1f);
        myText.text += bootingUpLines[8];
        myText.text += "\n";
        yield return new WaitForSeconds(.1f);
        myText.text += bootingUpLines[9];
        myText.text += "\n";
        yield return new WaitForSeconds(.1f);
        myText.text += bootingUpLines[10];
        myText.text += "\n";
        yield return new WaitForSeconds(.01f);
        myText.text += bootingUpLines[5];
        myText.text += "\n";
        yield return new WaitForSeconds(.01f);
        myText.text += bootingUpLines[6];
        myText.text += "\n";
        yield return new WaitForSeconds(.01f);
        myText.text += bootingUpLines[7];
        myText.text += "\n";
        yield return new WaitForSeconds(.01f);
        myText.text += bootingUpLines[8];
        myText.text += "\n";
        yield return new WaitForSeconds(.01f);
        myText.text += bootingUpLines[9];
        myText.text += "\n";
        yield return new WaitForSeconds(.01f);
        myText.text += bootingUpLines[10];
        myText.text += "\n";
        yield return new WaitForSeconds(.01f);
        myText.text += bootingUpLines[11];
        myText.text += "\n";
        yield return new WaitForSeconds(.1f);
        myText.text += bootingUpLines[12];
        myText.text += "\n\n";
        yield return new WaitForSeconds(2);
        myText.text += bootingUpLines[13];
        yield return new WaitForSeconds(3);
        myText.text += bootingUpLines[14];
        yield return new WaitForSeconds(.75f);
        myText.text = "";
        yield return new WaitForSeconds(3f);
        #endregion

        StartCoroutine(Fade(true));
    }

    IEnumerator EndOfDemoCutscene()
    {
        yield return new WaitForSeconds(7);

        myText.text = ">  ";
        yield return new WaitForSeconds(2);
        StartCoroutine(ScrollText(endOfDemoLines, 0));
        yield return new WaitForSeconds(3);
        StartCoroutine(ScrollText(endOfDemoLines, 1));
        yield return new WaitForSeconds(3);
        StartCoroutine(ScrollText(endOfDemoLines, 2));
        yield return new WaitForSeconds(3);

        Color oldColor = myText.color;

        float elapsedTime = 0;
        while (elapsedTime < fadeInTimer)
        {
            myText.color = Color.Lerp(oldColor, Color.clear, fadeCurve.Evaluate(elapsedTime / fadeInTimer));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ScrollText(string[] lines, int lineIndex)
    {
        char[] letters = lines[lineIndex].ToCharArray();

        foreach (char letter in letters)
        {
            myText.text += letter;
            yield return new WaitForSeconds(timeBetweenLetters);
        }

        if (lineIndex < lines.Length - 1) myText.text += "\n\n>  ";
    }

    IEnumerator Fade(bool fadeIn)
    {
        Color oldColor = panelImage.color;
        Color newColor = fadeIn ? Color.clear : Color.black;

        float elapsedTime = 0;
        while (elapsedTime < fadeInTimer)
        {
            panelImage.color = Color.Lerp(oldColor, newColor, fadeCurve.Evaluate(elapsedTime / fadeInTimer));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        panelImage.color = newColor;
        if (OnStartGame != null) OnStartGame();
    }
}
