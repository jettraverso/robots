using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ClothingArticles { HAT, SHIRT, PANTS }

public class GetDressed : MonoBehaviour
{
    [SerializeField] SpriteRenderer hatRenderer, shirtRenderer, pantsRenderer;
    [SerializeField] Sprite[] hatSprites, shirtSprites, pantsSprites;
    [SerializeField] Button hatButton, shirtButton, pantsButton;

    ClothingArticles currentlySelectedClothingArticle = ClothingArticles.HAT;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // confirm choices
        }

        bool cyclingRight = Input.GetButtonDown("CycleRight");

        if (cyclingRight || Input.GetButtonDown("CycleLeft"))
        {
            switch (currentlySelectedClothingArticle)
            {
                case ClothingArticles.HAT:
                    CycleClothing(hatRenderer, hatSprites, cyclingRight);
                    break;
                case ClothingArticles.SHIRT:
                    CycleClothing(shirtRenderer, shirtSprites, cyclingRight);
                    break;
                case ClothingArticles.PANTS:
                    CycleClothing(pantsRenderer, pantsSprites, cyclingRight);
                    break;
                default:
                    break;
            }
        }
            
    }

    void CycleClothing(SpriteRenderer clothingRenderer, Sprite[] clothingSprites, bool cyclingRight)
    {
        if (cyclingRight)
        {
            for (int i = 0; i < clothingSprites.Length; i++)
            {
                if (clothingRenderer.sprite == clothingSprites[i])
                {
                    if (i == clothingSprites.Length - 1)
                    {
                        clothingRenderer.sprite = clothingSprites[0];
                        break;
                    }
                    else
                    {
                        clothingRenderer.sprite = clothingSprites[i + 1];
                        break;
                    }
                }
                else if (clothingRenderer.sprite == null)
                {
                    clothingRenderer.sprite = clothingSprites[0];
                    break;
                }
            }
        }
        else
        {
            for (int i = clothingSprites.Length - 1; i >= 0; i--)
            {
                if (clothingRenderer.sprite == clothingSprites[i])
                {
                    if (i == 0)
                    {
                        clothingRenderer.sprite = clothingSprites[clothingSprites.Length - 1];
                        break;
                    }
                    else
                    {
                        clothingRenderer.sprite = clothingSprites[i - 1];
                        break;
                    }
                }
                else if (clothingRenderer.sprite == null)
                {
                    clothingRenderer.sprite = clothingSprites[clothingSprites.Length - 1];
                    break;
                }
            }
        }
    }

    public void OnHatButtonSelected()
    {
        currentlySelectedClothingArticle = ClothingArticles.HAT;

        hatButton.GetComponent<Text>().fontStyle = FontStyle.Bold;
        shirtButton.GetComponent<Text>().fontStyle = FontStyle.Normal;
        pantsButton.GetComponent<Text>().fontStyle = FontStyle.Normal;
    }

    public void OnShirtButtonSelected()
    {
        currentlySelectedClothingArticle = ClothingArticles.SHIRT;

        hatButton.GetComponent<Text>().fontStyle = FontStyle.Normal;
        shirtButton.GetComponent<Text>().fontStyle = FontStyle.Bold;
        pantsButton.GetComponent<Text>().fontStyle = FontStyle.Normal;
    }

    public void OnPantsButtonSelected()
    {
        currentlySelectedClothingArticle = ClothingArticles.PANTS;

        hatButton.GetComponent<Text>().fontStyle = FontStyle.Normal;
        shirtButton.GetComponent<Text>().fontStyle = FontStyle.Normal;
        pantsButton.GetComponent<Text>().fontStyle = FontStyle.Bold;
    }
}
