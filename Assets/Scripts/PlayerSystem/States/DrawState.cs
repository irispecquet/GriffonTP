using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DrawState : IStateSystem
{
    float _timer = 0;
    List<Transform> _movingCards = new();

    public void OnEnter(SystemManager controller)
    {
        _timer = controller.CardTimerMove;

        int numberCardsPub = controller.Pub.GetCardLength();

        if (numberCardsPub >= 4)
            return;

        int numberCardsDrawer = controller.Drawer.GetCardLength();

        for (int i = 0; i < 4 - numberCardsPub; i++)
        {
            if (controller.Drawer.GetCardLength() <= 0)
                break;

            CardData cardToChange = controller.Drawer.GetCard(numberCardsDrawer - 1 - i);

            controller.Pub.AddCard(cardToChange);
            controller.Drawer.DeleteCard(cardToChange);

            _movingCards.Add(SystemManager.Instance.CardsOnBoard[cardToChange.ID].transform);
            controller.Pub.CardPlaces[i].CardInPlace = cardToChange;
            //SystemManager.Instance.CardsOnBoard[cardToChange.ID].transform.position = controller.Pub.CardsTransform[i].position;
        }
    }

    public void UpdateState(SystemManager controller)
    {
        if (_timer >= 0)
        {
            _timer -= Time.deltaTime;

            for (int j = 0; j < _movingCards.Count; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    var card = controller.Pub.CardPlaces[i].CardInPlace;

                    if (card != null && card == _movingCards[j].GetComponent<CardComponent>().CardData)
                    {
                        _movingCards[j].position = Vector3.Lerp(_movingCards[j].position, controller.Pub.CardPlaces[i].TransformCard.position, 0.01f);
                    }
                }
            }
        }
        else
        {
            _movingCards.Clear();
            controller.ChangeState(controller.PubState);
        }
    }

    public void OnExit(SystemManager controller)
    {

    }
}
