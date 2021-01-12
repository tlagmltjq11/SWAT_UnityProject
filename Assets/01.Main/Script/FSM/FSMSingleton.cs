using UnityEngine;
public class FSMSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;	
	private static object _lock = new object();

	public static T Instance
	{
		get
		{
			lock(_lock)
			{
				if (_instance == null)
				{
					_instance = (T) FindObjectOfType(typeof(T)); 
					
					if ( FindObjectsOfType(typeof(T)).Length > 1 ) //1개 이상 생성된 경우
					{
						Debug.LogError("--- FSMSingleton error ---"); //에러메세지 출력
						return _instance;
					}
					
					if (_instance == null) //찾아도 없을 경우
					{
						//새로 인스턴스화 해서 리턴
						GameObject singleton = new GameObject(); 
						_instance = singleton.AddComponent<T>();
						singleton.name = "(singleton) "+ typeof(T).ToString();
						singleton.hideFlags = HideFlags.HideAndDontSave;
					}
					else //있을 경우
						Debug.LogError("--- FSMSingleton already exists ---");
				}
				return _instance; //반환
			}
		}
	}
}