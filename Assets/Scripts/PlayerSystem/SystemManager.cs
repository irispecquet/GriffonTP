using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    #region Singleton
    public static SystemManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("⚠2 SystemManager⚠");
    }
    #endregion Singleton

    #region States
    public IStateSystem currentState;
    public DrawState DrawState = new();
    public PubState PubState = new();
    public HostelState HostelState = new();
    #endregion States

    public CardDrawer Drawer;
    public CardDrawer Trash;
    public CardDrawer Pub;
    public CardDrawer Hostel;

    [SerializeField] CardComponent _cardPrefab;
    [SerializeField] Transform _cardPlacementPrefab;
    public CardData[] CardDatas;
    public Dictionary<int, CardComponent> CardsOnBoard = new();
    public CardComponent CardSelected;
    public float CardTimerMove = 1.5f;

    private void Start()
    {
        for (int i = 0; i < CardDatas.Length; i++)
        {
            CardComponent card = Instantiate(_cardPrefab, Drawer.transform.position, Quaternion.identity);
            card.CardData = CardDatas[i];
            CardsOnBoard.Add(card.CardData.ID, card);

            //Transform place = Instantiate(_cardPlacementPrefab, Hostel.transform);
        }

        foreach (var item in CardsOnBoard)
        {
            Drawer.AddCard(item.Value.GetComponent<CardComponent>().CardData);
        }

        ChangeState(DrawState);
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
    }

    public void ChangeState(IStateSystem newState)
    {
        if (currentState != newState)
        {
            if (currentState != null)
            {
                currentState.OnExit(this);
            }
            currentState = newState;
            currentState.OnEnter(this);

            // Debug.Log("State change " + newState.ToString());
        }
    }
}

public interface IStateSystem
{
    public void OnEnter(SystemManager controller);
    public void UpdateState(SystemManager controller);
    public void OnExit(SystemManager controller);
}