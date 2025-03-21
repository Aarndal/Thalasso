using ProgressionTracking;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MasterMindPuzzleLogic : SolvableObject, IAmPuzzle
{
    [SerializeField] private Button[] numButtons;
    [SerializeField] private TextMeshProUGUI[] outputText;
    [SerializeField] private int puzzleID = 1;

#if WWISE_2024_OR_LATER
    [Header("WWise Events")]
    [SerializeField]
    private SO_WwiseEvent _buttonPress = default;
    [SerializeField]
    private SO_WwiseEvent _wrongInput = default;
    [SerializeField]
    private SO_WwiseEvent _correctInput = default;
#endif

    private AkGameObj _akGameObject = default;

    private readonly int[] code = new int[4];
    private readonly int[] inputCode = new int[4];

    private void Awake()
    {
        if (!gameObject.TryGetComponent(out _akGameObject))
            _akGameObject = gameObject.AddComponent<AkGameObj>();

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
        _akGameObject.isEnvironmentAware = false;

        GenerateNewCode();
        for (int i = 0; i < numButtons.Length - 3; i++)
        {
            int buttonIndex = i + 1;
            numButtons[i].onClick.AddListener(() => NumButtonInput(buttonIndex));
        }
        numButtons[9].onClick.AddListener(() => Solve());
        numButtons[10].onClick.AddListener(() => NumButtonInput(0));
        numButtons[11].onClick.AddListener(() => Reset());
#if WWISE_2024_OR_LATER
        numButtons[11].onClick.AddListener(() => _buttonPress.Play(_akGameObject));
#endif
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
        System.Random rnd = new();
        List<int> availableNumbers = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

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
#if WWISE_2024_OR_LATER
        _buttonPress.Play(_akGameObject);
#endif
        if (inputIndex <= 3)
        {
            inputCode[inputIndex] = _num;
            outputText[inputIndex].text = _num.ToString();
            inputIndex++;
        }
    }

    public override bool Solve()
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
#if WWISE_2024_OR_LATER
                _wrongInput.Play(_akGameObject);
#endif
                return false;
            }
        }

        Debug.LogFormat("<color=green>{0} solved!</color>", name);
#if WWISE_2024_OR_LATER
        _correctInput.Play(_akGameObject);
#endif
        return IsSolved = true;
    }
}
