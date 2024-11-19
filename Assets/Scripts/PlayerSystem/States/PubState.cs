using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PubState : IStateSystem
{
    float _timer = 0;
    public void OnEnter(SystemManager controller)
    {
        _timer = controller.CardTimerMove;
    }

    public void UpdateState(SystemManager controller)
    {
        if (controller.CardSelected != null && _timer >= 0)
        {
            _timer -= Time.deltaTime;
            controller.CardSelected.transform.position = Vector3.Lerp(controller.CardSelected.transform.position, controller.Hostel.CardPlaces[controller.Hostel.GetCardLength()].TransformCard.position, 0.01f);
        }
        else if (_timer <= 0)
            controller.ChangeState(controller.HostelState);
    }

    public void OnExit(SystemManager controller)
    {
        CardData cardToChange = controller.CardSelected.GetComponent<CardComponent>().CardData;

        controller.Hostel.AddCard(cardToChange);
        controller.Pub.DeleteCard(cardToChange);

        controller.CardSelected = null;
    }
}
