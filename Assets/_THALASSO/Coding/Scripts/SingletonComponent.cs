using UnityEngine;

public class SingletonComponent<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    private static bool _isQuitting = false;

    private static readonly object _threadLock = new();

    #region Properties
    public static bool HasInstance => _instance != null;

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
                        var singeltonGameObject = new GameObject(typeof(T).Name + " (Auto-Generated)");
                        _instance = singeltonGameObject.AddComponent<T>();

                        DontDestroyOnLoad(singeltonGameObject);
                        Debug.LogFormat("New Instance of {0} created!", _instance.gameObject.name);
                    }
                }
            }
            return _instance;
        }
    }
    #endregion

    #region Unity Lifecycle Methods
    protected virtual void Awake()
    {
        InitSingleton();
    }

    protected virtual void OnDestroy()
    {
        _isQuitting = true;
    }
    #endregion

    public static T TryGetInstance() => HasInstance ? _instance : null;

    protected void InitSingleton()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this as T;
        DontDestroyOnLoad(gameObject);

        if (Instance == null)
            return;
    }
}
