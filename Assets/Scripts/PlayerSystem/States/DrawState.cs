using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DrawState : IStateSystem
{
    float _timer = 0;
    List<Transform> _movingCards = new();
    List<Transform> _cardsDestination = new();

    public void OnEnter(SystemManager controller)
    {
        _timer = controller.CardTimerMove;

        if (controller.Pub.GetCardLength() >= 4)
            return;

        for (int j = 0; j < controller.Pub.CardPlaces.Count; j++)
        {
                if (controller.Drawer.GetCardLength() < 0)
                    break;

                CardData cardToChange = controller.Drawer.GetCard(controller.Drawer.GetCardLength() - 1);

            if (controller.Pub.CardPlaces[j].CardInPlace == null)
            {
                controller.Pub.AddCard(cardToChange);
                controller.Drawer.DeleteCard(cardToChange);

                controller.Pub.CardPlaces[j].CardInPlace = cardToChange;

                _movingCards.Add(SystemManager.Instance.CardsOnBoard[cardToChange.ID].transform);
                _cardsDestination.Add(controller.Pub.CardPlaces[j].TransformCard);
            }
        }
        //SystemManager.Instance.CardsOnBoard[cardToChange.ID].transform.position = controller.Pub.CardsTransform[i].position;
    }

    public void UpdateState(SystemManager controller)
    {
        if (_timer >= 0)
        {
            _timer -= Time.deltaTime;

            for (int i = 0; i < _movingCards.Count; i++)
            {
                _movingCards[i].position = Vector3.Lerp(_movingCards[i].position, _cardsDestination[i].position, 0.01f);
            }
        }
        else
        {
            controller.ChangeState(controller.PubState);
        }
    }

    public void OnExit(SystemManager controller)
    {
        _movingCards.Clear();
        _cardsDestination.Clear();
    }
}
