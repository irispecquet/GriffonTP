using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostelState : IStateSystem
{
    float _timer;
    List<Transform> _movingCards = new();

    public void OnEnter(SystemManager controller)
    {
        _timer = controller.CardTimerMove;

        for (int i = 0; i < controller.Hostel.GetCardLength(); i++)
        {
            int id = controller.Hostel.GetCard(i).ID;
            CardComponent cardComponent = SystemManager.Instance.CardsOnBoard[id];

            if (!cardComponent.IsStaying())
            {
                _movingCards.Add(cardComponent.transform);
                CardData cardToChange = cardComponent.CardData;
                controller.Trash.AddCard(cardToChange);
                controller.Hostel.CardPlaces[controller.Hostel.GetCardPosInPlaces(cardToChange)].CardInPlace = null;
                controller.Hostel.DeleteCard(cardToChange);
            }
        }

        //Check les effets des cartes
    }

    public void UpdateState(SystemManager controller)
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;

            for (int i = 0; i < _movingCards.Count; i++)
            {
                _movingCards[i].position = Vector3.Lerp(_movingCards[i].position, controller.Trash.CardPlaces[0].TransformCard.position, 0.01f);
            }
        }
        else
        {
            _movingCards.Clear();
            controller.ChangeState(controller.DrawState);
        }
    }

    public void OnExit(SystemManager controller)
    {
    }
}
