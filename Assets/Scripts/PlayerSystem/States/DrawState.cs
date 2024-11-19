using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawState : IStateSystem
{
    public void OnEnter(SystemManager controller)
    {
        int numberCardsPub = controller.Pub.GetCardLength();

        if (numberCardsPub >= 4)
            return;

        int numberCardsDrawer = controller.Drawer.GetCardLength();


        for (int i = 0; i < 4 - numberCardsPub; i++)
        {
            CardData cardToChange = controller.Drawer.GetCard(numberCardsDrawer - 1 - i);

            controller.Pub.AddCard(cardToChange);
            controller.Drawer.DeleteCard(cardToChange);
        }
    }

    public void UpdateState(SystemManager controller)
    {

    }

    public void OnExit(SystemManager controller)
    {
        controller.ChangeState(controller.PubState);
    }
}
