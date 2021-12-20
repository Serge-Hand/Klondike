using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtons : MonoBehaviour
{
    public GameObject highScorePanel;
    public GameObject deckButton;

    public GameObject timer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAgain()
    {
        ResetScene();
    }

    public void ResetScene()
    {
        ////find all the cards and remove them
        //deckButton.GetComponent<Renderer>().enabled = true;
        //UpdateSprite[] cards = FindObjectsOfType<UpdateSprite>();
        //foreach (UpdateSprite card in cards)
        //{
        //    Destroy(card.gameObject);
        //}
        ////clear the top values
        //ClearTopValues();
        ////deal new cards
        //ScoreKeeper.won = false;
        //FindObjectOfType<Solitare>().PlayCards();
        //timer.GetComponent<TimeCounter>().StartTimeCounter();
        FindObjectOfType<AudioManager>().Play("ClickButton");
        SceneManager.LoadScene("SampleScene");
    }

    void ClearTopValues()
    {
        Selectable[] selectables = FindObjectsOfType<Selectable>();
        foreach (Selectable selectable in selectables)
        {
            if (selectable.CompareTag("Top"))
            {
                selectable.suit = null;
                selectable.value = 0;
            }
        }
    }
}
