using Godot;

namespace SadChromaLib.AI.States;

/// <summary> An object that handles processing through states. Sub-classing allows for fully customising how states are handled. </summary>
public partial class StateMachine : RefCounted
{
    public delegate void StateMachineDelegate(State state);
    public delegate void StateClearedDelegate();

    /// <summary> This event is called when the state machine is 'preparing' a state before it becomes active. </summary>
    public StateMachineDelegate onConfigure;
    /// <summary> This event is called when the 'CompleteState' function is called. </summary>
    public StateMachineDelegate onCompletion;
    /// <summary> This event is called when the state machine has successfully transitioned to a valid state. </summary>
    public StateMachineDelegate onTransition;
    /// <summary> This event is called when the state machine transitions to a null state. </summary>
    public StateClearedDelegate onStateClear;

    protected State _currentState;
    private Node _owner;

    public StateMachine(Node owner, StateMachineDelegate configurationHandler = null, StateMachineDelegate completionHandler = null, StateMachineDelegate transitionHandler = null)
    {
        _owner = owner;

        onConfigure = configurationHandler;
        onCompletion = completionHandler;
        onTransition = transitionHandler;
    }

    #region Main Functions

    /// <summary> Processes the currently-active state. </summary>
    public void Exec(float delta)
    {
        if (!IsInstanceValid(_currentState))
            return;

        _currentState.OnUpdate(this, delta);
    }

    /// <summary> Processes the currently-active state. </summary>
    public void Exec(double delta)
    {
        Exec((float) delta);
    }

    /// <summary> Returns a RefCounted to the currently-active state. </summary>
    public State GetState()
    {
        return _currentState;
    }

    /// <summary> Returns a RefCounted to the state machine's owner. </summary>
    public T GetOwner<T>() where T: Node
    {
        return (T) _owner;
    }

    /// <summary> Tells the state machine that all tasks have been completed. This must only be called by the currently-active state. </summary>
    public void CompleteState(State state)
    {
        if (_currentState != state)
            return;

        onCompletion?.Invoke(state);
    }

    public void TransitionState(State nextState)
    {
        // Prevent transitioning to the same state
        if (_currentState == nextState)
            return;

        // Prepare the next state
        State previousState = _currentState;
        onConfigure?.Invoke(nextState);

        // Cleanup
        if (IsInstanceValid(_currentState)) {
            _currentState.OnExit(this);
        }

        // Finalisation
        _currentState = nextState;

        if (!IsInstanceValid(_currentState)) {
            onStateClear?.Invoke();
            return;
        }

        onTransition?.Invoke(previousState);
        _currentState.OnEnter(this);
    }

    #endregion
}