using UnityEngine;

public class FSM <T>  : MonoBehaviour
{
	private T m_owner;
	private IFSMState<T> m_currentState = null;
	private IFSMState<T> m_previousState = null;

	public IFSMState<T> CurrentState{ get {return m_currentState;} }
	public IFSMState<T> PreviousState{ get {return m_previousState;} }

	//	초기 상태 설정..
	protected void InitState(T owner, IFSMState<T> initialState)
	{
		this.m_owner = owner;
		ChangeState(initialState);
	}

	//	각 상태의 Idle 처리..
	protected void  FSMUpdate() 
	{ 
		if (m_currentState != null) m_currentState.Execute(m_owner);
	}

	//	상태 변경..
	public void  ChangeState(IFSMState<T> newState)
	{
		m_previousState = m_currentState;
 
		if (m_currentState != null)
			m_currentState.Exit(m_owner);

		m_currentState = newState;
 
		if (m_currentState != null)
			m_currentState.Enter(m_owner);
	}

	//	이전 상태로 전환..
	public void  RevertState()
	{
		if (m_previousState != null)
			ChangeState(m_previousState);
	}

	public override string ToString() 
	{ 
		return m_currentState.ToString(); 
	}
}