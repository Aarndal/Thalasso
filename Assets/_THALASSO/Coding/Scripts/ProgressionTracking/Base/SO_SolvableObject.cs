using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSolvableObject", menuName = "Scriptable Objects/Solvable Object")]
public class SO_SolvableObject : ScriptableObject
{
    public event Action<uint, bool> SolvedStateChanged;

    private uint _id;
    private bool _isSolved;

    public uint ID => _id;
    public bool IsSolved
    {
        get => _isSolved;
        set
        {
            if (value != _isSolved)
            {
                _isSolved = value;
                SolvedStateChanged?.Invoke(_id, _isSolved);
            }
        }
    }
}
