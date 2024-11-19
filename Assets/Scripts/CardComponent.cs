using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class CardComponent : MonoBehaviour
{
    public CardData CardData;

    #region CONDITIONS

    [Button]
    public bool IsStaying()
    {
        if (!CheckPenalty())
        {
            Debug.Log($"{CardData.Name} (ID : {CardData.ID}) IS NOT STAYING");
            return false;
        }

        foreach (CardCondition condition in CardData.Conditions)
        {
            CardData[] dataByPlace = GetDataByPlace(condition.Place);
            int cardIndex = Array.IndexOf(dataByPlace, CardData);

            if (!EvaluateCondition(condition, dataByPlace, cardIndex))
            {
                Debug.Log($"{CardData.Name} (ID : {CardData.ID}) IS NOT STAYING");
                return false;
            }
        }

        Debug.Log($"{CardData.Name} (ID : {CardData.ID}) IS STAYING");
        return true;
    }

    private bool CheckPenalty()
    {
        if (CardData.CurrentPenalty == CardPenalty.NONE)
            return true;

        int currentPenaltyCount = 0;

        foreach (CardData card in GetDataByPlace(Place.BAR))
        {
            if (card.Penalty == CardData.CurrentPenalty)
            {
                currentPenaltyCount += card.PenaltyMultiplier;
                return currentPenaltyCount < 3;
            }
        }

        return true;
    }

    private bool EvaluateCondition(CardCondition condition, CardData[] dataByPlace, int cardIndex)
    {
        switch (condition.Type)
        {
            case ConditionType.RESOURCE:
                return EvaluateResourceCondition(condition, dataByPlace, cardIndex);
            case ConditionType.CUSTOMER:
                return EvaluateCustomerCondition(condition, dataByPlace);
            case ConditionType.FLOOR:
                return EvaluateFloorCondition(condition, dataByPlace, cardIndex);
            case ConditionType.PENALTY:
                return EvaluatePenaltyCondition(condition, dataByPlace, cardIndex);
            default:
                return true;
        }
    }

    private bool EvaluateResourceCondition(CardCondition condition, CardData[] dataByPlace, int cardIndex)
    {
        if (condition.Comparison == ComparisonType.OPERATOR)
        {
            int resourceCount = CountMatchingResources(dataByPlace, condition.Resource);
            int resourceTargetCount = GetResourceTargetCount(condition, dataByPlace);

            return !ComparisonEvaluator.Evaluate(resourceCount, condition.Operator, resourceTargetCount);
        }

        if (condition.Comparison == ComparisonType.RELATIVE_POSITION)
            return CheckRelativePosition(dataByPlace, cardIndex, condition.Resource, condition.Position);

        return true;
    }

    private bool EvaluateCustomerCondition(CardCondition condition, CardData[] dataByPlace)
    {
        if (condition.Comparison != ComparisonType.OPERATOR)
            return true;

        int customerCount = CountMatchingCustomers(dataByPlace, condition.CustomerName);
        int customerTargetCount = condition.TargetType == ConditionType.VALUE ? condition.TargetValue : 0;

        return !ComparisonEvaluator.Evaluate(customerCount, condition.Operator, customerTargetCount);
    }

    private bool EvaluateFloorCondition(CardCondition condition, CardData[] dataByPlace, int cardIndex)
    {
        if (condition.Comparison == ComparisonType.RELATIVE_POSITION)
        {
            return (condition.Position != RelativePosition.HIGHEST || cardIndex > 0) &&
                   (condition.Position != RelativePosition.LOWEST || cardIndex < dataByPlace.Length - 1);
        }

        return condition.Comparison != ComparisonType.OPERATOR ||
               !ComparisonEvaluator.Evaluate(cardIndex, condition.Operator, condition.TargetValue);
    }

    private bool EvaluatePenaltyCondition(CardCondition condition, CardData[] dataByPlace, int cardIndex)
    {
        return condition.Comparison != ComparisonType.RELATIVE_POSITION ||
               condition.Position != RelativePosition.ABOVE ||
               cardIndex <= 0 ||
               dataByPlace[cardIndex - 1].Penalty != condition.Penalty;
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

    #endregion // CONDITIONS

    #region EFFECTS

    public void ExecuteEffect()
    {
        CardData[] customers = GetDataByPlace(Place.HOSTEL);
        int cardIndex = Array.IndexOf(customers, CardData);

        foreach (CardEffect effect in CardData.Effects)
        {
            if (effect.Leave)
            {
                Leave();
                continue;
            }

            if (effect.MakeOtherCustomersLeave || effect.MakeOtherCustomersEffect)
            {
                List<CardComponent> leavingCustomers = new List<CardComponent>();
                List<CardComponent> effectCustomers = new List<CardComponent>();

                EvaluateEffectTargets(effect, customers, cardIndex, leavingCustomers, effectCustomers);

                ApplyEffectToCustomers(leavingCustomers, effectCustomers);
            }
        }
    }

    private void EvaluateEffectTargets(CardEffect effect, CardData[] customers, int cardIndex, List<CardComponent> leavingCustomers, List<CardComponent> effectCustomers)
    {
        switch (effect.CustomerConditionType)
        {
            case ConditionType.PENALTY:
                AddCustomersByPenalty(effect, customers, leavingCustomers, effectCustomers);
                break;
            case ConditionType.CUSTOMER:
                AddCustomersByName(effect, customers, leavingCustomers, effectCustomers);
                break;
            case ConditionType.FLOOR:
                AddCustomersByFloor(effect, customers, cardIndex, leavingCustomers);
                break;
        }
    }

    private void AddCustomersByPenalty(CardEffect effect, CardData[] customers, List<CardComponent> leavingCustomers, List<CardComponent> effectCustomers)
    {
        foreach (CardData customer in customers)
        {
            if (customer.Penalty != effect.CustomerPenalty)
                continue;

            CardComponent cardComponent = GetCardComponent(customer.ID);

            if (effect.MakeOtherCustomersLeave)
                leavingCustomers.Add(cardComponent);

            if (effect.MakeOtherCustomersEffect)
                effectCustomers.Add(cardComponent);
        }
    }

    private void AddCustomersByName(CardEffect effect, CardData[] customers, List<CardComponent> leavingCustomers, List<CardComponent> effectCustomers)
    {
        foreach (CardData customer in customers)
        {
            if (!customer.Name.Contains(effect.CustomerName))
                continue;

            CardComponent cardComponent = GetCardComponent(customer.ID);

            if (effect.MakeOtherCustomersLeave)
                leavingCustomers.Add(cardComponent);

            if (effect.MakeOtherCustomersEffect)
                effectCustomers.Add(cardComponent);
        }
    }

    private void AddCustomersByFloor(CardEffect effect, CardData[] customers, int cardIndex, List<CardComponent> leavingCustomers)
    {
        int index = cardIndex + (int)Mathf.Sign(effect.CustomerIndex) * Mathf.Abs(effect.CustomerIndex);

        if (index >= 0 && index < customers.Length)
        {
            CardComponent cardComponent = GetCardComponent(customers[index].ID);
            leavingCustomers.Add(cardComponent);
        }
    }

    private void ApplyEffectToCustomers(List<CardComponent> leavingCustomers, List<CardComponent> effectCustomers)
    {
        foreach (CardComponent leavingCustomer in leavingCustomers)
            leavingCustomer.Leave();

        foreach (CardComponent effectCustomer in effectCustomers)
            effectCustomer.ExecuteEffect();
    }

    private CardComponent GetCardComponent(int customerId)
    {
        return SystemManager.Instance.CardsOnBoard[customerId].GetComponent<CardComponent>();
    }

    #endregion // EFFECTS

    private void Leave()
    {
    }

    private void OnMouseDown()
    {
        if (SystemManager.Instance.currentState == SystemManager.Instance.PubState &&
            SystemManager.Instance.CardSelected == null)
        {
            SystemManager.Instance.CardSelected = this.gameObject;
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