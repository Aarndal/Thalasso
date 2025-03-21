using UnityEngine;

namespace WwiseHelper
{
    [RequireComponent(typeof(Collider), typeof(AkEnvironment))]
    public class WwiseEnvironmentSetter : MonoBehaviour
    {
        [SerializeField]
        private SO_WwiseEnvironmentData _environmentData = default;

        private Collider _collider = default;
        private AkEnvironment _akEnvironment = default;

        private void Awake()
        {
            if (!gameObject.TryGetComponent(out _collider))
                Debug.LogErrorFormat("<color=cyan>{0} (ID: {1})</color> <color=red>has no Collider attached</color> for its {2} and AkEnvironment components to work properly!", gameObject.name, gameObject.GetInstanceID(), this);

            if (!gameObject.TryGetComponent(out _akEnvironment))
                _akEnvironment = gameObject.AddComponent<AkEnvironment>();

            _collider.isTrigger = true;
            _environmentData.SetEnvironmentData(_akEnvironment);
        }

        private void OnValidate()
        {
            if (!gameObject.TryGetComponent(out _collider))
                _collider.isTrigger = true;

            if (gameObject.TryGetComponent(out _akEnvironment) && _environmentData != null)
                _environmentData.SetEnvironmentData(_akEnvironment);
        }
    }
}
