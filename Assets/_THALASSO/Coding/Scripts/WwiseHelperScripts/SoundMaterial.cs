using UnityEngine;

namespace WwiseHelper
{
    public class SoundMaterial : MonoBehaviour
    {
#if WWISE_2024_OR_LATER
        [SerializeField]
        private AK.Wwise.Switch _soundMaterial;

        public AK.Wwise.Switch Get() => _soundMaterial;
#endif
    }
}
