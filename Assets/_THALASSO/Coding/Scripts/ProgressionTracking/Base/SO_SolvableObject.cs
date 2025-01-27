using System;
using UnityEngine;

namespace ProgressionTracking
{
    [CreateAssetMenu(fileName = "NewSolvableObject", menuName = "Scriptable Objects/Solvable Object")]
    public class SO_SolvableObject : SO_ResettableData, INotifyValueChanged<uint, bool>
    {
        [SerializeField]
        private uint _id;
        [SerializeField]
        private bool _isSolved;

        private Action<uint, bool> _valueChanged;

        public uint ID => _id;
        public bool IsSolved
        {
            get => _isSolved;
            set
            {
                if (value != _isSolved)
                {
                    _isSolved = value;
                    _valueChanged?.Invoke(_id, _isSolved);
                }
            }
        }

        public event Action<uint, bool> ValueChanged
        {
            add
            {
                _valueChanged -= value;
                _valueChanged += value;
            }
            remove
            {
                _valueChanged -= value;
            }
        }

        public override void ResetData() => IsSolved = false;
    }
}
