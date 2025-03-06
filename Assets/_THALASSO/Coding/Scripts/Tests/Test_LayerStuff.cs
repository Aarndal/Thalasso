
using UnityEngine;

namespace Tests
{
    internal class Test_LayerStuff : MonoBehaviour
    {
        [SerializeField]
        private Layers layer = Layers.Default;
        [SerializeField]
        private LayerMaskFlags layerMaskFlags = LayerMaskFlags.Default;
        [SerializeField]
        private LayerMask layerMask;
    }
}
