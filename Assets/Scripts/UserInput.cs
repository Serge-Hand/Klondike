using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UserInput : MonoBehaviour
{
    public GameObject Pause;

    public GameObject slot1;
    private Solitare solitare;
    private float timer;
    private float doubleClickTime = 0.3f;
    private int clickCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        solitare = FindObjectOfType<Solitare>();
        slot1 = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Pause.activeSelf)
        {
            if (clickCount == 1)
            {
                timer += Time.deltaTime;
            }
            if (clickCount == 3)
            {
                timer = 0;
                clickCount = 1;
            }
            if (timer > doubleClickTime)
            {
                timer = 0;
                clickCount = 0;
            }
            GetMouseClick();
        }
    }

    void GetMouseClick()
    {
        if(Input.GetMouseButtonDown(0))
        {
            clickCount++;

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit)
            {
                //what has been hit? Deck/Card/EmptySlot...
                if(hit.collider.CompareTag("Deck"))
                {
                    //clicked deck
                    Deck();
                }
                else if(hit.collider.CompareTag("Card"))
                {
                    Card(hit.collider.gameObject);
                }
                else if(hit.collider.CompareTag("Top"))
                {
                    Top(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Bottom"))
                {
                    Bottom(hit.collider.gameObject);
                }
            }
        }
    }

    void Deck()
    {
        print("Clicked on deck");
        solitare.DealFromDeckOn();
        slot1 = this.gameObject;
    }
    void Card(GameObject selected)
    {
        print("Clicked on card");

        if (!selected.GetComponent<Selectable>().faceUp)//if the card clicked on is facedown
        {
            if (!Blocked(selected))//if the card clicked on is not blocked
            {
                //flip it over
                FindObjectOfType<AudioManager>().Play("FaceUpCard");
                selected.GetComponent<Selectable>().faceUp = true;
                slot1 = this.gameObject;
            }
        }
        else if (selected.GetComponent<Selectable>().inDeckPile)//if the card clicked on in the deck pile with the trips
        {
            //if it is not blocked
            //select it
            if (!Blocked(selected))
            {
                if(!(slot1 == selected))
                    FindObjectOfType<AudioManager>().Play("ClickCard");
                if (slot1 == selected)//if the same card is clicked twice
                {
                    if (DoubleClick())
                    {
                        //attempt auto stack
                        AutoStack(selected);
                    }
                }
                else
                {
                    slot1 = selected;
                }
            }
        }
        else
        {
            //if the card is face up
            //if there is no card currently selected
            //select the card

            if (slot1 == this.gameObject)
            {
                FindObjectOfType<AudioManager>().Play("ClickCard");
                slot1 = selected;
            }

            //if there is already a card selected (and it is not the same card)
            else if (slot1 != selected)
            {
                //if the new card is eligable to stack on the old card
                if (Stackable(selected))
                {
                    FindObjectOfType<AudioManager>().Play("StackCard");
                    Stack(selected);
                }
                else
                {
                    //select the new card
                    FindObjectOfType<AudioManager>().Play("ClickCard");
                    slot1 = selected;
                }
            }

            else if (slot1 == selected)//if the same card is clicked twice
            {
                if (DoubleClick())
                {
                    //attempt auto stack
                    AutoStack(selected);
                }
            }
        }
    }
    void Top(GameObject selected)
    {
        print("Clicked on top");
        if (slot1.CompareTag("Card"))
        {
            //if the card is an ace and the empty slot is top then stack
            FindObjectOfType<AudioManager>().Play("StackCard");
            if (slot1.GetComponent<Selectable>().value == 1)
            {
                Stack(selected);
            }
        }
    }
    void Bottom(GameObject selected)
    {
        print("Clicked on bottom");

        //if the card is a king and empty sot is bottom then stack
        if (slot1.CompareTag("Card"))
        {
            FindObjectOfType<AudioManager>().Play("StackCard");
            if (slot1.GetComponent<Selectable>().value == 13)
            {
                Stack(selected);
            }
        }
    }

    bool Stackable(GameObject selected)
    {
        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();
        //compare them to see if they stack

        if (!s2.inDeckPile)
        {
            if (s2.top)//if int the top pile must stack suited Ace to King
            {
                if (s1.suit == s2.suit || (s1.value == 1 && s2.suit == null))
                {
                    if (s1.value == s2.value + 1)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else //if in the bottom pile must stack alternate colours King to Ace
            {
                if (s1.value == s2.value - 1)
                {
                    bool card1Red = true;
                    bool card2Red = true;

                    if (s1.suit == "C" || s1.suit == "S")
                    {
                        card1Red = false;
                    }
                    if (s2.suit == "C" || s2.suit == "S")
                    {
                        card2Red = false;
                    }

                    if (card1Red == card2Red)
                    {
                        print("Not stackable");
                        return false;
                    }
                    else
                    {
                        print("Stackable");
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void Stack(GameObject selected)
    {
        //if on top of king or empty botoom stack the cards in place
        //else stack the cards with negative y offset

        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();
        float yOffset = 0.45f;

        if (s2.top || (!s2.top && s1.value == 13))
        {
            yOffset = 0;
        }

        slot1.transform.position = new Vector3(selected.transform.position.x, selected.transform.position.y - yOffset, selected.transform.position.z - 0.01f);
        slot1.transform.parent = selected.transform;//this makes the children move with the parents

        if (s1.inDeckPile)//removes the cards from the top pile to prevent duplicate cards
        {
            solitare.tripsOnDisplay.Remove(slot1.name);
        }
        else if (s1.top && s2.top && s1.value == 1)//allows movement of cards between top spots
        {
            solitare.topPos[s1.row].GetComponent<Selectable>().value = 0;
            solitare.topPos[s1.row].GetComponent<Selectable>().suit = null;
        }
        else if (s1.top)//keeps track of the current value of the top decks as a card has been removed
        {
            solitare.topPos[s1.row].GetComponent<Selectable>().value = s1.value - 1;
        }
        else//removes the card string from the approriate bottom list
        {
            solitare.bottoms[s1.row].Remove(slot1.name);
        }

        s1.inDeckPile = false; //you cannot add cards to the trips pile so this is always fine
        s1.row = s2.row;

        if (s2.top)//moves a card to the top and assigns the top's value and suit
        {
            solitare.topPos[s1.row].GetComponent<Selectable>().value = s1.value;
            solitare.topPos[s1.row].GetComponent<Selectable>().suit = s1.suit;
            s1.top = true;
        }
        else
        {
            s1.top = false;
        }

        //after completing move reset slot1 to be essentially null as being null will break thw logic
        slot1 = this.gameObject;
    }

    bool Blocked(GameObject selected)
    {
        Selectable s2 = selected.GetComponent<Selectable>();
        if (s2.inDeckPile == true)
        {
            if (s2.name == solitare.tripsOnDisplay.Last())//if it is the last trip it is not blocked
            {
                return false;
            }
            else
            {
                print(s2.name + " is blocked by " + solitare.tripsOnDisplay.Last());
                return true;
            }
        }
        else
        {
            if (s2.name == solitare.bottoms[s2.row].Last())//check if it is the bottom card
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    bool DoubleClick()
    {
        if (timer < doubleClickTime && clickCount == 2)
        {
            print("Double click");
            return true;
        }
        else
        {
            return false;
        }
    }

    void AutoStack(GameObject selected)
    {
        for (int i = 0; i  < solitare.topPos.Length; i++)
        {
            Selectable stack = solitare.topPos[i].GetComponent<Selectable>();
            if (selected.GetComponent<Selectable>().value == 1)//if it is an ace
            {
                if (solitare.topPos[i].GetComponent<Selectable>().value == 0)//and the top pos is empty
                {
                    slot1 = selected;
                    FindObjectOfType<AudioManager>().Play("AutoStack");
                    Stack(stack.gameObject);//stack the ace up top in the first empty position found
                    break;
                }
            }
            else
            {
                if ((solitare.topPos[i].GetComponent<Selectable>().suit == slot1.GetComponent<Selectable>().suit) && (solitare.topPos[i].GetComponent<Selectable>().value == slot1.GetComponent<Selectable>().value - 1))
                {
                    //if it is the last card (if it has no children)
                    if (HasNoChildren(slot1))
                    {
                        slot1 = selected;
                        //find a top spot that matches the conditions for auto stacking if it exists
                        string lastCardname = stack.suit + stack.value.ToString();
                        if (stack.value == 1)
                        {
                            lastCardname = stack.suit + "A";
                        }
                        if (stack.value == 11)
                        {
                            lastCardname = stack.suit + "J";
                        }
                        if (stack.value == 12)
                        {
                            lastCardname = stack.suit + "Q";
                        }
                        if (stack.value == 13)
                        {
                            lastCardname = stack.suit + "K";
                        }
                        GameObject lastCard = GameObject.Find(lastCardname);
                        FindObjectOfType<AudioManager>().Play("AutoStack");
                        Stack(lastCard);
                        break;
                    }
                }
            }
        }
    }

    bool HasNoChildren(GameObject card)
    {
        int i = 0;
        foreach(Transform child in card.transform)
        {
            i++;
        }
        if (i == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
