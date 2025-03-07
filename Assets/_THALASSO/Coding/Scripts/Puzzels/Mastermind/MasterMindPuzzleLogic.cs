using ProgressionTracking;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MasterMindPuzzleLogic : SolvableObjectBase, IAmRiddle
{
    [SerializeField] private Button[] numButtons;
    [SerializeField] private TextMeshProUGUI[] outputText;
    [SerializeField] private int puzzleID = 1;

    [Header("WWise Events")]
    [SerializeField]
    private AkGameObj akGameObject = default;
    [SerializeField]
    private AK.Wwise.Event buttonPress = default;
    [SerializeField]
    private AK.Wwise.Event wrongInput = default;
    [SerializeField]
    private AK.Wwise.Event correctInput = default;

    private int[] code = new int[4];
    private int[] inputCode = new int[4];

    private void Awake()
    {
        PuzzleUIReferencesSender.PuzzleUIReferenceLogger += GetUIReference;
    }

    private void Reset()
    {
        inputIndex = 0;
        for (int i = 0; i < outputText.Length; i++)
        {
            outputText[i].text = "";
            outputText[i].color = Color.white;
        }
    }

    private void Start()
    {
        GenerateNewCode();
        for (int i = 0; i < numButtons.Length - 3; i++)
        {
            int buttonIndex = i + 1;
            numButtons[i].onClick.AddListener(() => NumButtonInput(buttonIndex));
        }
        numButtons[9].onClick.AddListener(() => CheckInput());
        numButtons[10].onClick.AddListener(() => NumButtonInput(0));
        numButtons[11].onClick.AddListener(() => Reset());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void StartPuzzle()
    {
        return;
    }

    private void GenerateNewCode()
    {
        System.Random rnd = new System.Random();
        List<int> availableNumbers = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        for (int i = 0; i < code.Length; i++)
        {
            int randomIndex = rnd.Next(availableNumbers.Count);
            code[i] = availableNumbers[randomIndex];
            availableNumbers.RemoveAt(randomIndex);
        }

        //foreach (var item in code)
        //{
        //    Debug.Log(item);
        //}
    }

    private void GetUIReference(GameObject reference, int ID)
    {
        if (ID == puzzleID)
        {
            Button[] allChildWithButtons = reference.transform.GetComponentsInChildren<Button>();
            numButtons = new Button[allChildWithButtons.Length];
            for (int i = 0; i < allChildWithButtons.Length; i++)
            {
                numButtons[i] = allChildWithButtons[i];
            }
        }
    }

    int inputIndex = 0;
    public void NumButtonInput(int _num)
    {
        buttonPress.Post(akGameObject.gameObject);

        if (inputIndex <= 3)
        {
            inputCode[inputIndex] = _num;
            outputText[inputIndex].text = _num.ToString();
            inputIndex++;
        }
    }

    private void CheckInput()
    {
        for (int i = 0; i < inputCode.Length; i++)
        {
            if (inputCode[i] == code[i])
            {
                outputText[i].color = Color.green;
            }
            else if (code.Contains(inputCode[i]))
            {
                outputText[i].color = Color.yellow;
            }
            else
            {
                outputText[i].color = Color.white;
            }
        }

        for (int i = 0; i < inputCode.Length; i++)
        {
            if (outputText[i].color != Color.green)
            {
                wrongInput.Post(akGameObject.gameObject);
                return;
            }
        }

        Debug.LogFormat("<color=green>{0} solved!</color>", name);
        Solve();
    }

    public override bool Solve()
    {
        correctInput.Post(akGameObject.gameObject);
        return IsSolved = true;
    }
}
