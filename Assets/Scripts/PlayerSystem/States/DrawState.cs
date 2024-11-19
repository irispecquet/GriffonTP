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

        if (controller.Pub.GetCardLength() >= 4)
            return;

        for (int j = 0; j < 4; j++)
        {
            if (controller.Pub.CardPlaces[j].CardInPlace == null)
            {
                if (controller.Drawer.GetCardLength() < 0)
                    break;

                CardData cardToChange = controller.Drawer.GetCard(controller.Drawer.GetCardLength() - 1);

                controller.Pub.AddCard(cardToChange);
                controller.Drawer.DeleteCard(cardToChange);

                _movingCards.Add(SystemManager.Instance.CardsOnBoard[cardToChange.ID].transform);

                controller.Pub.CardPlaces[j].CardInPlace = cardToChange;
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
                for (int j = 0; j < controller.Pub.CardPlaces.Count; j++)
                {
                    CardData card = controller.Pub.CardPlaces[j].CardInPlace;

                    if (card == _movingCards[i].GetComponent<CardComponent>().CardData)
                    {
                        _movingCards[i].position = Vector3.Lerp(_movingCards[i].position, controller.Pub.CardPlaces[j].TransformCard.position, 0.01f);
                    }
                }
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

    }
}
