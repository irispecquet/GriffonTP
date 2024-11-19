using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDrawer : MonoBehaviour
{
    [SerializeField] List<CardData> _cards;

    public void AddCard(CardData card)
    {
        _cards.Add(card);
    }

    public void AddCard(CardData[] card)
    {
        foreach (CardData cardData in _cards)
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

    public CardData GetCard(CardData card)
    {
        CardData cardData = null;

        for (int i = 0; i < _cards.Count; i++)
        {
            if (_cards[i] == card)
                cardData = _cards[i];
        }

        return cardData;
    }
}
