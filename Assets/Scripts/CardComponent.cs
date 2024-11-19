using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardComponent : MonoBehaviour
{
    public CardData CardData;

    private void OnMouseDown()
    {
        if (SystemManager.Instance.currentState == SystemManager.Instance.PubState && SystemManager.Instance.CardSelected == null)
        {
            SystemManager.Instance.CardSelected = this.gameObject;
            SystemManager.Instance.Pub.CardPlaces[SystemManager.Instance.Pub.GetCardPosInList(SystemManager.Instance.CardSelected.GetComponent<CardComponent>().CardData)].CardInPlace = null;
        }
    }
}
