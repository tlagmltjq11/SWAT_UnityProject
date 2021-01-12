using UnityEngine;

public class FSM <T>  : MonoBehaviour
{
	private T m_owner;
	private IFSMState<T> m_currentState = null;
	private IFSMState<T> m_previousState = null;

	public IFSMState<T> CurrentState{ get {return m_currentState;} }
	public IFSMState<T> PreviousState{ get {return m_previousState;} }

	//	�ʱ� ���� ����..
	protected void InitState(T owner, IFSMState<T> initialState)
	{
		this.m_owner = owner;
		ChangeState(initialState);
	}

	//	�� ������ Idle ó��..
	protected void  FSMUpdate() 
	{ 
		if (m_currentState != null) m_currentState.Execute(m_owner);
	}

	//	���� ����..
	public void  ChangeState(IFSMState<T> newState)
	{
		m_previousState = m_currentState;
 
		if (m_currentState != null)
			m_currentState.Exit(m_owner);

		m_currentState = newState;
 
		if (m_currentState != null)
			m_currentState.Enter(m_owner);
	}

	//	���� ���·� ��ȯ..
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