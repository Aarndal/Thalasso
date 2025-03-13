using UnityEngine;

public abstract class SO_Singleton<T> : ScriptableObject where T : ScriptableObject
{
    private T _instance;

    public T Instance
    {
        get => _instance;
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
