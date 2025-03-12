using System;
using System.Collections.Generic;
using UnityEngine;

namespace WwiseHelper
{
    public enum SoundBankLoadMethod
    {
        Awake = 0,
        OnEnable = 1,
        Start = 2,
    }

    public enum SoundBankUnloadMethod
    {
        OnDestroy = 0,
        OnDisable = 1,
    }

    [Serializable]
    public class WwiseSoundBanks : MonoBehaviour
    {
#if WWISE_2024_OR_LATER

        [SerializeField]
        private List<AK.Wwise.Bank> _soundBanks = new();
        [SerializeField]
        private bool _loadAsynchronously = false;
        [SerializeField]
        private SoundBankLoadMethod _loadMethod = SoundBankLoadMethod.Awake;
        [SerializeField]
        private SoundBankUnloadMethod _unloadMethod = SoundBankUnloadMethod.OnDestroy;

        public Dictionary<uint, AK.Wwise.Bank> ByID = new();
        public Dictionary<string, AK.Wwise.Bank> ByName = new();

        private void Awake()
        {
            if (_soundBanks.Count == 0)
            {
                Debug.LogWarningFormat("<color=yellow>No SoundBanks were assigned</color> on <color=cyan>{0}</color>!", gameObject.name);
                return;
            }

            int nonValidElements = 0;

            foreach (var soundBank in _soundBanks)
            {
                if (!soundBank.IsValid())
                {
                    nonValidElements++;
                    continue;
                }

                if (!ByID.TryAdd(soundBank.Id, soundBank))
                {
                    Debug.LogWarningFormat("SoundBank <color=cyan>{1} (ID: {2})</color>  <color=yellow>is assigned multiple times</color> on <color=cyan>{0}</color>!", gameObject.name, soundBank.Name, soundBank.Id);

                    continue;
                }

                ByName.TryAdd(soundBank.Name, soundBank);
            }

            if (nonValidElements == 1)
            {
                Debug.LogWarningFormat("<color=yellow>{1} SoundBank has not been assigned</color> on <color=cyan>{0}</color>!", gameObject.name, nonValidElements);
            }

            if (nonValidElements > 1)
            {
                Debug.LogWarningFormat("<color=yellow>{1} SoundBanks have not been assigned</color> on <color=cyan>{0}</color>!", gameObject.name, nonValidElements);
            }

            TryLoadSoundBanks(SoundBankLoadMethod.Awake);
        }

        private void OnEnable()
        {
            TryLoadSoundBanks(SoundBankLoadMethod.OnEnable);
        }

        private void Start()
        {
            TryLoadSoundBanks(SoundBankLoadMethod.Start);
        }

        private void OnDisable()
        {
            TryUnloadSoundBanks(SoundBankUnloadMethod.OnDisable);
        }

        private void OnDestroy()
        {
            TryUnloadSoundBanks(SoundBankUnloadMethod.OnDestroy);
        }

        private bool TryLoadSoundBanks(SoundBankLoadMethod loadMethod)
        {
            if (loadMethod != _loadMethod)
                return false;

            if (_loadAsynchronously)
            {
                foreach (var soundBank in ByID.Values)
                {
                    soundBank.LoadAsync();
                }

                return true;
            }

            foreach (var soundBank in ByID.Values)
            {
                soundBank.Load();
            }

            return true;
        }

        private bool TryUnloadSoundBanks(SoundBankUnloadMethod unloadMethod)
        {
            if (unloadMethod != _unloadMethod)
                return false;

            foreach (var soundBank in ByID.Values)
            {
                soundBank.Unload();
            }

            return true;
        }
#endif
    }
}
