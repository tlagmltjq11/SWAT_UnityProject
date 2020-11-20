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
					
					if ( FindObjectsOfType(typeof(T)).Length > 1 )
					{
						Debug.LogError("--- FSMSingleton error ---");
						return _instance;
					}
					
					if (_instance == null)
					{
						GameObject singleton = new GameObject();
						_instance = singleton.AddComponent<T>();
						singleton.name = "(singleton) "+ typeof(T).ToString();
						singleton.hideFlags = HideFlags.HideAndDontSave;
					}
					else
						Debug.LogError("--- FSMSingleton already exists ---");
				}
				return _instance;
			}
		}
	}
}