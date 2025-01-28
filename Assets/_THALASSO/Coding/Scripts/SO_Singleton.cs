using UnityEngine;

public abstract class SO_Singleton : ScriptableObject
{
    private SO_Singleton _instance;

    public SO_Singleton Instance { get => _instance; private set => _instance = value; }

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        // This will prevent the object from being destroyed when loading a new scene
        DontDestroyOnLoad(this);

        if (Instance == null)
        {
            Debug.LogErrorFormat("<color=red>{0} not active!</color>", nameof(Instance));
            return;
        }
        
        Debug.LogFormat("<color=green>{0} active.</color> | ID: {1}", nameof(Instance), GetInstanceID());
    }
}
