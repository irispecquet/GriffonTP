using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Card Data", order = 0)]
public class CardData : ScriptableObject
{
    [field:SerializeField] public string Name { get; private set; }
    [field:SerializeField] public Consumption Consumption { get; private set; }
    [field:SerializeField] public NegativeEffect[] NegativeEffects { get; private set; }
    // Condition[]
}

public enum Consumption
{
    Food,
    Beer,
}

public enum NegativeEffect
{
    Fight,
    Smell,
    Loud
}
