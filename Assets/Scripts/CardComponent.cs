using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class CardComponent : MonoBehaviour
{
    public CardData CardData;

    [Button]
    public bool IsStaying()
    {
        bool isStaying = true;

        if (!CheckPenalty(ref isStaying))
            return isStaying;

        foreach (CardCondition condition in CardData.Conditions)
        {
            CardData[] dataByPlace = GetDataByPlace(condition.Place);
            int cardIndex = Array.IndexOf(dataByPlace, CardData);

            if (!EvaluateCondition(condition, dataByPlace, cardIndex, ref isStaying))
                break;
        }

        Debug.Log($"{CardData.Name} (ID : {CardData.ID}) IS {(isStaying ? "" : "NOT")} STAYING");
        return isStaying;
    }

    private bool CheckPenalty(ref bool isStaying)
    {
        if (CardData.CurrentPenalty == CardPenalty.NONE)
            return true;

        int currentPenaltyCount = 0;

        foreach (CardData card in GetDataByPlace(Place.BAR))
        {
            if (card.Penalty == CardData.CurrentPenalty)
            {
                currentPenaltyCount += card.PenaltyMultiplier;

                if (currentPenaltyCount >= 3)
                {
                    isStaying = false;
                    return false;
                }
            }
        }

        return true;
    }

    private bool EvaluateCondition(CardCondition condition, CardData[] dataByPlace, int cardIndex, ref bool isStaying)
    {
        switch (condition.Type)
        {
            case ConditionType.RESOURCE:
                return EvaluateResourceCondition(condition, dataByPlace, cardIndex, ref isStaying);
            case ConditionType.CUSTOMER:
                return EvaluateCustomerCondition(condition, dataByPlace, ref isStaying);
            case ConditionType.FLOOR:
                return EvaluateFloorCondition(condition, dataByPlace, cardIndex, ref isStaying);
            case ConditionType.PENALTY:
                return EvaluatePenaltyCondition(condition, dataByPlace, cardIndex, ref isStaying);
            default:
                return true;
        }
    }

    private bool EvaluateResourceCondition(CardCondition condition, CardData[] dataByPlace, int cardIndex,
        ref bool isStaying)
    {
        if (condition.Comparison == ComparisonType.OPERATOR)
        {
            int resourceCount = CountMatchingResources(dataByPlace, condition.Resource);
            int resourceTargetCount = GetResourceTargetCount(condition, dataByPlace);

            if (ComparisonEvaluator.Evaluate(resourceCount, condition.Operator, resourceTargetCount))
            {
                isStaying = false;
                return false;
            }
        }
        else if (condition.Comparison == ComparisonType.RELATIVE_POSITION)
        {
            if (!CheckRelativePosition(dataByPlace, cardIndex, condition.Resource, condition.Position))
            {
                isStaying = false;
                return false;
            }
        }

        return true;
    }

    private bool EvaluateCustomerCondition(CardCondition condition, CardData[] dataByPlace, ref bool isStaying)
    {
        if (condition.Comparison != ComparisonType.OPERATOR)
            return true;

        int customerCount = CountMatchingCustomers(dataByPlace, condition.CustomerName);
        int customerTargetCount = condition.TargetType == ConditionType.VALUE ? condition.TargetValue : 0;

        if (ComparisonEvaluator.Evaluate(customerCount, condition.Operator, customerTargetCount))
        {
            isStaying = false;
            return false;
        }

        return true;
    }

    private bool EvaluateFloorCondition(CardCondition condition, CardData[] dataByPlace, int cardIndex,
        ref bool isStaying)
    {
        if (condition.Comparison == ComparisonType.RELATIVE_POSITION)
        {
            if ((condition.Position == RelativePosition.HIGHEST && cardIndex <= 0) ||
                (condition.Position == RelativePosition.LOWEST && cardIndex >= dataByPlace.Length - 1))
            {
                isStaying = false;
                return false;
            }
        }
        else if (condition.Comparison == ComparisonType.OPERATOR)
        {
            if (ComparisonEvaluator.Evaluate(cardIndex, condition.Operator, condition.TargetValue))
            {
                isStaying = false;
                return false;
            }
        }

        return true;
    }

    private bool EvaluatePenaltyCondition(CardCondition condition, CardData[] dataByPlace, int cardIndex,
        ref bool isStaying)
    {
        if (condition.Comparison == ComparisonType.RELATIVE_POSITION &&
            condition.Position == RelativePosition.ABOVE &&
            cardIndex > 0 &&
            dataByPlace[cardIndex - 1].Penalty == condition.Penalty)
        {
            isStaying = false;
            return false;
        }

        return true;
    }

    private int CountMatchingResources(CardData[] dataByPlace, ResourceType resourceType)
    {
        int count = 0;
        foreach (CardData card in dataByPlace)
        {
            if (card.ResourceType == resourceType)
                count++;
        }

        return count;
    }

    private int GetResourceTargetCount(CardCondition condition, CardData[] dataByPlace)
    {
        if (condition.TargetType == ConditionType.VALUE)
            return condition.TargetValue;

        if (condition.TargetType == ConditionType.RESOURCE)
            return CountMatchingResources(dataByPlace, condition.TargetResource);

        return 0;
    }

    private int CountMatchingCustomers(CardData[] dataByPlace, string customerName)
    {
        int count = 0;
        foreach (CardData card in dataByPlace)
        {
            if (card.Name.Contains(customerName))
                count++;
        }

        return count;
    }

    private bool CheckRelativePosition(CardData[] dataByPlace, int cardIndex, ResourceType resourceType,
        RelativePosition position)
    {
        if (position == RelativePosition.ABOVE && cardIndex > 0)
            return dataByPlace[cardIndex - 1].ResourceType != resourceType;

        if (position == RelativePosition.BELOW && cardIndex < dataByPlace.Length - 1)
            return dataByPlace[cardIndex + 1].ResourceType != resourceType;

        return true;
    }

    private void OnMouseDown()
    {
        if (SystemManager.Instance.currentState == SystemManager.Instance.PubState && SystemManager.Instance.CardSelected == null)
        {
            SystemManager.Instance.CardSelected = this.gameObject;
            SystemManager.Instance.Pub.CardPlaces[SystemManager.Instance.Pub.GetCardPosInList(SystemManager.Instance.CardSelected.GetComponent<CardComponent>().CardData)].CardInPlace = null;
        }
    }
    
    private CardData[] GetDataByPlace(Place place)
    {
        if (place == Place.BAR)
            return SystemManager.Instance.Pub.GetAllCards();

        if (place == Place.HOSTEL)
            return SystemManager.Instance.Hostel.GetAllCards();

        return null;
    }
}
