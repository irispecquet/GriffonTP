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


    public IStateSystem currentState;
    public PubState PubState = new PubState();
    public HostelState HostelState = new HostelState();
    public DrawState DrawState = new DrawState();

    private void Start()
    {
        ChangeState(PubState);
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
        }
    }
}

public interface IStateSystem
{
    public void OnEnter(SystemManager controller);
    public void UpdateState(SystemManager controller);
    public void OnExit(SystemManager controller);
}