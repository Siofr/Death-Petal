using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    [SerializeField] private string _startState;

    private BaseState _currentState;
    private Dictionary<string, BaseState> _playerStates = new Dictionary<string, BaseState>();

    private void Awake()
    {
        foreach (Transform state in this.transform)
        {
            string stateName = state.name;
            BaseState stateComponent = state.GetComponent<BaseState>();
            stateComponent.enabled = false;
            _playerStates.Add(stateName, stateComponent);
        }

        _currentState = _playerStates[_startState];
        OnStateChanged(_startState);
    }

    void Update()
    {
        _currentState?.FrameUpdate();
    }

    private void LateUpdate()
    {
        _currentState?.LateFrameUpdate();
    }

    public void OnStateChanged(string newState)
    {
        _currentState.ExitState();
        _currentState.enabled = false;
        _currentState = _playerStates[newState];
        _currentState.enabled = true;
        _currentState.EnterState();
    }
}
