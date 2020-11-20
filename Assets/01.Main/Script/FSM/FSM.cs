using UnityEngine;

public class FSM <T>  : MonoBehaviour
{
	private T owner;
	private IFSMState<T> currentState = null;
	private IFSMState<T> previousState = null;

	public IFSMState<T> CurrentState{ get {return currentState;} }
	public IFSMState<T> PreviousState{ get {return previousState;} }

	//	초기 상태 설정..
	protected void InitState(T owner, IFSMState<T> initialState)
	{
		this.owner = owner;
		ChangeState(initialState);
	}

	//	각 상태의 Idle 처리..
	protected void  FSMUpdate() 
	{ 
		if (currentState != null) currentState.Execute(owner);
	}

	//	상태 변경..
	public void  ChangeState(IFSMState<T> newState)
	{
		previousState = currentState;
 
		if (currentState != null)
			currentState.Exit(owner);
 
		currentState = newState;
 
		if (currentState != null)
			currentState.Enter(owner);
	}

	//	이전 상태로 전환..
	public void  RevertState()
	{
		if (previousState != null)
			ChangeState(previousState);
	}

	public override string ToString() 
	{ 
		return currentState.ToString(); 
	}
}