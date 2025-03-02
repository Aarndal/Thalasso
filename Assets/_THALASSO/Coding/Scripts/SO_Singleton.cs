using UnityEngine;

public abstract class SO_Singleton<T> : ScriptableObject where T : ScriptableObject
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                T[] singletons = Resources.LoadAll<T>("");

                if(singletons == null || singletons.Length < 1)
                {
                    Debug.LogErrorFormat("No {0} singleton objects found.", typeof(T).Name);
                }
                else if (singletons.Length > 1)
                {
                    Debug.LogWarningFormat("<color=yellow>More than one</color> <color=cyan>{0} singleton object</color> found.", typeof(T).Name);
                }

                _instance = singletons[0];
                Debug.LogFormat("<color=cyan>{0} singleton</color> instance fetched!", typeof(T).Name);
            }
            return _instance;
        }
        private set => _instance = value;
    }

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this as T;

        DontDestroyOnLoad(this);
    }
}
