using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeIn : MonoBehaviour
{
    [SerializeField] Image panelImage;
    [SerializeField] AnimationCurve curve;
    [SerializeField] float fadeTime = 2;

    GetOutOfBed_MouseMovement roanMovement;
    
	void Start ()
    {
        roanMovement = FindObjectOfType<GetOutOfBed_MouseMovement>();
        GetOutOfBed_MouseMovement.OnWon += CallFadeOut;

        StartCoroutine(Fade(true));
	}

    void CallFadeOut()
    {
        StartCoroutine(Fade(false));
    }

    IEnumerator Fade(bool fadeIn)
    {
        Color originalColor = panelImage.color, targetColor = fadeIn ? Color.clear : Color.black;

        float elapsedTime = 0;
        while (elapsedTime < fadeTime)
        {
            panelImage.color = Color.Lerp(originalColor, targetColor, curve.Evaluate(elapsedTime / fadeTime));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        panelImage.color = targetColor;
        if (fadeIn) roanMovement.ChangeState(GameState.GAME);
        else SceneManager.LoadScene("DemoEnding");
    }
}
