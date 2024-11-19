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
        int numberCardsPub = controller.Pub.GetCardLength();

        if (numberCardsPub >= 4)
            return;

        int numberCardsDrawer = controller.Drawer.GetCardLength();

        for (int i = 0; i < 4 - numberCardsPub; i++)
        {
            if (controller.Drawer.GetCardLength() <= 0)
                break;

            CardData cardToChange = controller.Drawer[numberCardsDrawer - 1 - i];

            controller.Pub.AddCard(cardToChange);
            controller.Drawer.DeleteCard(cardToChange);

            _movingCards.Add(SystemManager.Instance.CardsOnBoard[cardToChange.ID].transform);

            //SystemManager.Instance.CardsOnBoard[cardToChange.ID].transform.position = controller.Pub.CardsTransform[i].position;
        }

        _timer = (4 - numberCardsPub) * 100;
    }

    public void UpdateState(SystemManager controller)
    {
        if (_timer >= 0)
        {
            _timer -= Time.deltaTime;

            Vector3.Lerp(_movingCards[0].position, controller.Pub.CardsTransform[0].position, 0.01f);
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
