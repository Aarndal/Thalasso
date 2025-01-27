using System;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "NewSolvableObject", menuName = "Scriptable Objects/Solvable Object")]
public class SO_SolvableObject : ScriptableObject, INotifyValueChanged<uint, bool>
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

    private void Awake()
    {
#if UNITY_EDITOR
        IsSolved = false;
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        IsSolved = false;
#endif
    }
}
