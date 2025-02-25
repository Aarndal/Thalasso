using UnityEngine;

public class SwitchMusic : MonoBehaviour
{
    [SerializeField]
    private AK.Wwise.Switch _musicToPlayOnEnter;

    private Collider _collider = default;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("Player"))
        {
            _musicToPlayOnEnter.SetValue(other.gameObject);
        }
    }
}
