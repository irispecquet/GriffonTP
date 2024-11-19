using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDrawer : MonoBehaviour
{
    public List<Transform> CardsTransform;
    [SerializeField] List<CardData> _cards = new();

    public void AddCard(CardData card)
    {
        _cards.Add(card);
    }

    public void AddCard(CardData[] cards)
    {
        foreach (CardData cardData in cards)
        {
            _cards.Add(cardData);
        }
    }

    public void DeleteCard(CardData card)
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            if (_cards[i] == card)
            {
                _cards.RemoveAt(i);
                break;
            }
        }
    }

    private CardData GetCard(CardData card)
    {
        CardData cardData = null;

        for (int i = 0; i < _cards.Count; i++)
        {
            if (_cards[i] == card)
                cardData = _cards[i];
        }

        return cardData;
    }

    private CardData GetCard(int cardNumber)
    {
        if (cardNumber < _cards.Count)
            return _cards[cardNumber];
        
        Debug.LogError("Outside Length of " + gameObject.name + " card list of " + (cardNumber - _cards.Count));
        return null;
    }

    public CardData this[int cardNumber] => GetCard(cardNumber);
    public CardData this[CardData card] => GetCard(card);

    public int GetCardLength()
    {
        return _cards.Count;
    }
}