using ProgressionTracking;
using TMPro;
using UnityEngine;

public class SecurityPuzzleLogic : SolvableObject
{
    [SerializeField] private int puzzleID = 4;
    [SerializeField] private string password = "password";


    private GameObject uiCanvas;
    private TMP_InputField passwordInputField;

    private void Awake()
    {
        PuzzleUIReferencesSender.PuzzleUIReferenceLogger += GetUIReference;

    }
    private void OnDestroy()
    {
        PuzzleUIReferencesSender.PuzzleUIReferenceLogger -= GetUIReference;
    }
    private void GetUIReference(GameObject _reference, int _ID)
    {
        if (_ID == puzzleID)
        {
            uiCanvas = _reference;
            passwordInputField = uiCanvas.GetComponentInChildren<TMP_InputField>();
        }
    }
    public void OnCheckPassword()
    {
        string inputPassword = passwordInputField.text;
        if (inputPassword == password)
        {
            Solve();
            GetComponent<PuzzleColliderLogic>().Interact(transform);
        }
    }
    public void OnResetPassword()
    {
        passwordInputField.text = "";
    }

    public override bool Solve() => IsSolved = true;
}
