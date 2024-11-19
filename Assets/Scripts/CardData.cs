using System;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDefinition", menuName = "Card Definition", order = 0)]
public class CardData : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public ResourceType ResourceType { get; private set; }
    [field: SerializeField] public CardPenalty Penalty { get; private set; }
    [field: SerializeField] public int PenaltyMultiplier { get; private set; } = 1;
    [field: SerializeField] public CardCondition[] Conditions { get; private set; }
    [field: SerializeField] public CardPenalty CurrentPenalty { get; private set; }
}

[Flags]
public enum ResourceType
{
    FOOD = 1 << 0,
    BEER = 1 << 1,
}

[Flags]
public enum CardPenalty
{
    NONE = 0,
    FIGHT = 1 << 0,
    SMELL = 1 << 1,
    LOUD = 1 << 2,
}

public enum Place
{
    HOSTEL = 1 << 0,
    BAR = 1 << 1,
}

[Serializable]
public class CardCondition
{
    [field: SerializeField] public ConditionType Type { get; private set; }

    [field: SerializeField, ShowIf("@this.Type == ConditionType.VALUE")] public int Value { get; private set; }
    [field: SerializeField, ShowIf("@this.Type == ConditionType.PENALTY")] public CardPenalty Penalty { get; private set; }
    [field: SerializeField, ShowIf("@this.Type == ConditionType.RESOURCE")] public ResourceType Resource { get; private set; }
    [field: SerializeField, ShowIf("@this.Type == ConditionType.CUSTOMER")] public string CustomerName { get; private set; }

    [field: SerializeField] public ComparisonType Comparison { get; private set; }
    [field: SerializeField, ShowIf("@this.Comparison == ComparisonType.OPERATOR")] public ComparisonOperator Operator { get; private set; }
    [field: SerializeField, ShowIf("@this.Comparison == ComparisonType.RELATIVE_POSITION")] public RelativePosition Position { get; private set; }

    [field: SerializeField, ShowIf("@this.Comparison != ComparisonType.RELATIVE_POSITION")] public ConditionType TargetType { get; private set; }
    [field: SerializeField, ShowIf("@this.TargetType == ConditionType.VALUE && this.Comparison != ComparisonType.RELATIVE_POSITION")] public int TargetValue { get; private set; }
    [field: SerializeField, ShowIf("@this.TargetType == ConditionType.PENALTY && this.Comparison != ComparisonType.RELATIVE_POSITION")] public int TargetPenalty { get; private set; }
    [field: SerializeField, ShowIf("@this.TargetType == ConditionType.RESOURCE && this.Comparison != ComparisonType.RELATIVE_POSITION")] public ResourceType TargetResource { get; private set; }
    [field: SerializeField, ShowIf("@this.TargetType == ConditionType.CUSTOMER && this.Comparison != ComparisonType.RELATIVE_POSITION")] public string TargetCustomerName { get; private set; }

    [field: SerializeField] public Place Place { get; private set; }
}

public enum ConditionType
{
    VALUE = 0,
    PENALTY = 1,
    RESOURCE = 2,
    CUSTOMER = 3,
    FLOOR = 4,
}

public enum ComparisonType
{
    OPERATOR = 0,
    RELATIVE_POSITION = 1
}

public enum RelativePosition
{
    ABOVE = 0,
    BELOW = 1,
    HIGHEST = 2,
    LOWEST = 3,
}

public enum ComparisonOperator
{
    INFERIOR = 0,
    INFERIOR_OR_EQUAL = 1,
    EQUAL = 2,
    SUPERIOR_OR_EQUAL = 3,
    SUPERIOR = 4,
}

public static class ComparisonEvaluator
{
    public static bool Evaluate(int value, ComparisonOperator operatorValue, int targetValue)
    {
        return operatorValue switch
        {
            ComparisonOperator.INFERIOR => value < targetValue,
            ComparisonOperator.INFERIOR_OR_EQUAL => value <= targetValue,
            ComparisonOperator.EQUAL => value == targetValue,
            ComparisonOperator.SUPERIOR_OR_EQUAL => value >= targetValue,
            ComparisonOperator.SUPERIOR => value > targetValue,
            _ => throw new ArgumentOutOfRangeException(nameof(operatorValue), operatorValue, null)
        };
    }
}
