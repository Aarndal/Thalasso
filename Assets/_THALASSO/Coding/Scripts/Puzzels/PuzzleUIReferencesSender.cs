using System;
using UnityEngine;

public class PuzzleUIReferencesSender : MonoBehaviour
{
    [SerializeField] private int puzzleID;


    public static event Action<GameObject, int> puzzleUIReferenceLogger;

    private void OnEnable()
    {
        puzzleUIReferenceLogger?.Invoke(this.gameObject, puzzleID);
        this.gameObject.SetActive(false);
    }
}
