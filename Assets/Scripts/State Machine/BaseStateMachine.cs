using UnityEngine;

public class BaseStateMachine : MonoBehaviour
{
    bool canChangeStates = true;
    public void SetCanChangeStates(bool value) { canChangeStates = value; }

    BaseState currentState;
    public void ChangeState(BaseState state)
    {
        if (!canChangeStates || this == null) { return; }

        currentState.thisEnd();
        currentState = state;
        currentState.thisStart();
    }

    private void Start()
    {
        StartFunctions();

        if (InitialState() != null) { currentState = InitialState(); }

        currentState?.thisStart();
    }

    public virtual BaseState InitialState() { return null; }

    public virtual void StartFunctions()
    {

    }

    public virtual void UpdateFunctions()
    {

    }

    private void Update()
    {
        UpdateFunctions();
        currentState?.thisUpdate();
    }

    private void FixedUpdate()
    {
        currentState?.thisFixedUpdate();
    }
    private void LateUpdate()
    {
        currentState?.thisLateUpdate();
    }
}
