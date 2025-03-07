using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WwiseSwitchTrigger : MonoBehaviour
{
#if WWISE_2024_OR_LATER
    [SerializeField]
    private AK.Wwise.Switch _onEnterSwitchTo;
    [SerializeField]
    private AK.Wwise.Switch _onStaySwitchTo;
    [SerializeField]
    private AK.Wwise.Switch _onExitSwitchTo;

    private Collider _collider = default;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) => ChangeValueOfAkSoundBank(other, _onEnterSwitchTo);

    private void OnTriggerStay(Collider other) => ChangeValueOfAkSoundBank(other, _onStaySwitchTo);

    private void OnTriggerExit(Collider other) => ChangeValueOfAkSoundBank(other, _onExitSwitchTo);

    private void ChangeValueOfAkSoundBank(Collider collider, AK.Wwise.Switch @switch)
    {
        if (@switch != null)
        {
            if (collider.TryGetComponent<AkBank>(out _))
            {
                @switch.SetValue(collider.gameObject);
            }
        }
    }
#endif
}
