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

    public enum SystemState
    {
        Draw,
        Pub,
        Hostel
    }

    public SystemState State;
}
