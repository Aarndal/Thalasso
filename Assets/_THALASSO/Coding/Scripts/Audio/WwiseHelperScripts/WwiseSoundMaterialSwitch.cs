using System;
using UnityEngine;

namespace WwiseHelper
{
    public sealed class WwiseSoundMaterialSwitch : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private WwiseSoundMaterialChecker _soundMaterialChecker = default;

#if WWISE_2024_OR_LATER
        [Header("Wwise Settings")]
        [SerializeField]
        private AK.Wwise.Switch _defaultSoundMaterial = default;

        private AkGameObj _akGameObject = default;

        private void Awake()
        {
            _akGameObject = _akGameObject != null ? _akGameObject : GetComponent<AkGameObj>();

            if (_akGameObject == null)
                _akGameObject = gameObject.AddComponent<AkGameObj>();

            _soundMaterialChecker = _soundMaterialChecker != null ? _soundMaterialChecker : GetComponentInChildren<WwiseSoundMaterialChecker>();
        }

        private void OnEnable()
        {
            _soundMaterialChecker.ValueChanged += OnSoundMaterialChanged;
        }

        private void Start()
        {
            _defaultSoundMaterial.SetValue(_akGameObject.gameObject);
        }

        private void OnDisable()
        {
            _soundMaterialChecker.ValueChanged -= OnSoundMaterialChanged;
        }

        private void OnSoundMaterialChanged(uint id, AK.Wwise.Switch soundMaterial)
        {
            if (soundMaterial != null)
                soundMaterial.SetValue(_akGameObject.gameObject);
            else
                _defaultSoundMaterial.SetValue(_akGameObject.gameObject);
        }
#endif
    }
}
