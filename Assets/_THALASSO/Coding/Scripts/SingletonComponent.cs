using UnityEngine;

public class SingletonComponent<T> : MonoBehaviour where T : MonoBehaviour
{
    private static readonly object _threadLock = new();

    private static bool _isQuitting = false;

    private static T _instance = null;
    public static T Instance
    {
        get
        {
            if (_isQuitting)
                return null;

            if(_instance == null)
            {
                _instance = FindAnyObjectByType<T>();

                lock(_threadLock)
                {
                    if(_instance == null)
                    {
                        var singeltonGameObject = new GameObject();
                        singeltonGameObject.name = typeof(T).Name + " (Persists)";
                        _instance = singeltonGameObject.AddComponent<T>();

                        DontDestroyOnLoad(singeltonGameObject);
                        Debug.Log($"New Instance of {_instance.gameObject.name} created!");
                    }
                }
            }
            return _instance;
        }
    }
    protected virtual void Awake()
    {
        if(_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = GetComponent<T>();
        DontDestroyOnLoad(gameObject);

        if (_instance == null)
            return;
    }

    protected virtual void OnDestroy()
    {
        _isQuitting = true;
    }
}
