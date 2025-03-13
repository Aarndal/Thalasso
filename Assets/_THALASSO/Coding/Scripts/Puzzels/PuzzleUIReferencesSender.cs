using System;
using UnityEngine;

public class PuzzleUIReferencesSender : MonoBehaviour
{
    [SerializeField] private int puzzleID;


    public static event Action<GameObject, int> puzzleUIReferenceLogger;

    private bool send = false;
    private void OnEnable()
    {
        if (!send)
        {
            puzzleUIReferenceLogger?.Invoke(this.gameObject, puzzleID);
            send = true;
        }

    }
}
