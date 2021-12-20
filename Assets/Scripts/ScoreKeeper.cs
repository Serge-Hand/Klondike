using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour
{
    public Selectable[] topStacks;
    public GameObject highScorePanel;
    public GameObject newTimer;
    public bool won = false;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        if (HasWon() && !won)
        {
            Win();
            won = true;
        }
    }

    public bool HasWon()
    {
        int i = 0;
        foreach (Selectable topstack in topStacks)
        {
            i += topstack.value;
        }
        if (i >= 52)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Win()
    {
        FindObjectOfType<AudioManager>().Play("Win");
        highScorePanel.SetActive(true);
        newTimer.GetComponent<WinTime>().Win();
        print("You have won!");
    }
}
