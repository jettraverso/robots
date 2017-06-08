using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class Defragmentation : MonoBehaviour
{
    [SerializeField] TextAsset textToParse;
    [SerializeField] Text defragTextTop, defragTextBottom, dreamText;
    [SerializeField] float timeBetweenCharacters = .01f;

    [Tooltip("there is a 1 in ___ chance that the space will be an i instead of an o")]
    [SerializeField] int iProbability = 5;
    
    string[,] defragLines = new string[10, 18];
    string[] lines;

    private void Start()
    {
        string wholeText = textToParse.text;
        lines = Regex.Split(wholeText, "\n|\r|\r\n");

        GenerateDefragmentation();

        StartCoroutine(Defragment());
    }

    void GenerateDefragmentation()
    {
        defragTextTop.text = "";

        for (int i = 0; i < defragLines.GetLength(0); i++)
        {
            for (int j = 0; j < defragLines.GetLength(1); j++)
            {
                defragLines[i, j] = ReturnIorO(iProbability);
                defragTextTop.text += defragLines[i, j];
                if (j == defragLines.GetLength(1) - 1) defragTextTop.text += "\n";
            }
        }
    }

    void FillGaps(int gapsToFill, int linesIndex)
    {
        int counter = 0;
        char[] chars = defragTextTop.text.ToCharArray();

        List<char> charList = new List<char>();
        foreach (char c in chars) charList.Add(c);

        for (int i = 0; i < gapsToFill; i++)
        {
            if (charList[i] == ' ' && counter < 2)
            {
                counter++;
                charList.Remove(charList[i]);
            }
            else if (charList[i] == ' ' && counter == 2)
            {
                charList[i] = 'i';
                counter = 0;
            }
        }

        string newString = "";
        foreach (char c in charList)
        {
            newString += c;
        }
        defragTextTop.text = newString;

        // TODO: the lines array is only one line?
        dreamText.text = lines[linesIndex];
    }

    string ReturnIorO(int likelihood)
    {
        int randomInt = Random.Range(0, likelihood);

        switch (randomInt)
        {
            case 0:
                return "i";
            case 1:
                return "o";
            default:
                return "  ";
        }
    }

    IEnumerator Defragment()
    {
        yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
        FillGaps(10, 0);
        defragTextBottom.text += ReturnIorO(2);
        while (!Input.GetKeyDown(KeyCode.Space)) yield return null;
        FillGaps(20, 1);
        defragTextBottom.text += ReturnIorO(2);
        while (!Input.GetKeyDown(KeyCode.Space)) yield return null;
        FillGaps(30, 2);
        defragTextBottom.text += ReturnIorO(2);
        while (!Input.GetKeyDown(KeyCode.Space)) yield return null;
        FillGaps(40, 3);
        defragTextBottom.text += ReturnIorO(2);
        while (!Input.GetKeyDown(KeyCode.Space)) yield return null;
        FillGaps(50, 4);
        defragTextBottom.text += ReturnIorO(2);
        while (!Input.GetKeyDown(KeyCode.Space)) yield return null;
        FillGaps(60, 5);
        defragTextBottom.text += ReturnIorO(2);
    }
}
