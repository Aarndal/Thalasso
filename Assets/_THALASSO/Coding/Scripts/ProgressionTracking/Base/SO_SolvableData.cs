using System;
using UnityEngine;

namespace ProgressionTracking
{
    [CreateAssetMenu(fileName = "NewSolvableObject", menuName = "Scriptable Objects/Solvable Data")]
    public class SO_SolvableData : SO_ResettableData, INotifyValueChanged<bool>
    {
        [SerializeField]
        private uint _id = 0;
        [SerializeField]
        private bool _isSolved = false;

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
        
        private Action<uint, bool> _valueChanged;
        
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
