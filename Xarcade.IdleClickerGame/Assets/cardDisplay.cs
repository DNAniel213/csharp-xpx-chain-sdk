using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uClicker;
using UnityEngine.UI;

public class cardDisplay : MonoBehaviour
{
    public Building card;

    public Text nameText;
    public Text costText;
    public Text descText;

    public Image artworkImage;

    // Start is called before the first frame update
    void Start()
    {
        nameText.text = card.cardName;
        costText.text = card.Cost.ToString();
        descText.text = card.description;
        artworkImage.sprite = card.artwork;
    }
}
