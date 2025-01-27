using Unity.VisualScripting;
using UnityEngine;

public class SO_Singleton : ScriptableObject
{
    private SO_Singleton _instance;

    public SO_Singleton Instance
    {
        get => _instance;
        protected set => _instance = value;
    }

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
        Debug.Log(Instance.name + " active");
    }
}
