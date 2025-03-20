using UnityEngine;

public class UIMenuSound : MonoBehaviour
{
#if WWISE_2024_OR_LATER
    [SerializeField]
    private SO_WwiseEvent _enterMenuSound = default;
    [SerializeField]
    private SO_WwiseEvent _exitMenuSound = default;

    private AkGameObj _akGameObject = default;
#endif

    private void Awake()
    {
#if WWISE_2024_OR_LATER
        if (!gameObject.TryGetComponent(out _akGameObject))
            _akGameObject = gameObject.AddComponent<AkGameObj>();
#endif
    }

    private void OnEnable()
    {
        GlobalEventBus.Register(GlobalEvents.UI.MenuOpened, OnMenuOpened);
        GlobalEventBus.Register(GlobalEvents.UI.MenuClosed, OnMenuClosed);
    }

    private void OnDisable()
    {
        GlobalEventBus.Deregister(GlobalEvents.UI.MenuClosed, OnMenuClosed);
        GlobalEventBus.Deregister(GlobalEvents.UI.MenuOpened, OnMenuOpened);
    }

    private void OnMenuClosed(object[] eventArgs)
    {
#if WWISE_2024_OR_LATER
        PlayMenuSound(eventArgs, _exitMenuSound);
#endif
    }


    private void OnMenuOpened(object[] eventArgs)
    {
#if WWISE_2024_OR_LATER
        PlayMenuSound(eventArgs, _enterMenuSound);
#endif
    }

#if WWISE_2024_OR_LATER
    private void PlayMenuSound(object[] eventArgs, SO_WwiseEvent menuSound)
    {
        if (menuSound == null)
            return;

        foreach (var arg in eventArgs)
        {
            if (arg is string menuName && menuName == gameObject.name)
            {
                menuSound.Play(_akGameObject);
                return;
            }
        }
    }
#endif
}
