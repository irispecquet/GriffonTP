using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

                Transform cardTransform = SystemManager.Instance.CardsOnBoard[cardToChange.ID].transform;
                _movingCards.Add(cardTransform);

                controller.Pub.CardPlaces[j].CardInPlace = cardToChange;

                int a = j;
                cardTransform.DOMove(controller.Pub.CardPlaces[j].TransformCard.position, _timer).SetEase(Ease.OutQuad).OnComplete(() => controller.Pub.CardPlaces[a].CardInPlace = cardToChange);
            }
        }
    }

    public void UpdateState(SystemManager controller)
    {
        if (_timer <= 0)
            controller.ChangeState(controller.PubState);
        else
            _timer -= Time.deltaTime;
    }

    public void OnExit(SystemManager controller)
    {
        _movingCards.Clear();
        _cardsDestination.Clear();
    }
}