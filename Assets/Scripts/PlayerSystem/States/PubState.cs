using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PubState : IStateSystem
{
    float _timer = 0;
    int iterationNumber = -1;

    public void OnEnter(SystemManager controller)
    {
        _timer = controller.CardTimerMove;

        for (int i = 0; i < controller.Hostel.CardPlaces.Count; i++)
        {
            if (controller.Hostel.CardPlaces[i].CardInPlace == null)
            {
                iterationNumber = i;
                break;
            }
        }
    }

    public void UpdateState(SystemManager controller)
    {
        if (controller.CardSelected != null)
        {
            if (_timer >= 0)
            {
                _timer -= Time.deltaTime;
                controller.CardSelected.transform.position = Vector3.Lerp(controller.CardSelected.transform.position, controller.Hostel.CardPlaces[iterationNumber].TransformCard.position, 0.01f);
            }
            else if (_timer <= 0)
                controller.ChangeState(controller.HostelState);
        }
    }

    public void OnExit(SystemManager controller)
    {
        CardData cardToChange = controller.CardSelected.CardData;

        controller.Hostel.AddCard(cardToChange);
        controller.Hostel.CardPlaces[iterationNumber].CardInPlace = cardToChange;

        controller.Pub.CardPlaces[controller.Pub.GetCardPosInPlaces(cardToChange)].CardInPlace = null;
        controller.Pub.DeleteCard(cardToChange);

        controller.CardSelected = null;
    }
}
