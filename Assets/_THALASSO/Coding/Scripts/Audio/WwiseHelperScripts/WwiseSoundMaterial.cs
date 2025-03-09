using UnityEngine;

namespace WwiseHelper
{
    public class WwiseSoundMaterial : MonoBehaviour
    {
#if WWISE_2024_OR_LATER
        [SerializeField]
        private AK.Wwise.Switch _soundMaterial;

        public AK.Wwise.Switch Get() => _soundMaterial;
#endif
    }
}
