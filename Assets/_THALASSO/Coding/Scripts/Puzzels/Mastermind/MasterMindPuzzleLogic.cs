using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MasterMindPuzzleLogic : MonoBehaviour
{
    [SerializeField] private Button[] numButtons;
    [SerializeField] private TextMeshProUGUI[] outputText;

    private int[] code = new int[4];
    private int[] inputCode = new int[4];



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
        Debug.Log(code);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    int inputIndex = 0;
    public void NumButtonInput(int _num)
    {

        if (inputIndex <= 3)
        {
            inputCode[inputIndex] = _num;
            outputText[inputIndex].text = _num.ToString();
            inputIndex++;
        }
    }

    private void CheckInput()
    {
        bool allCorrect = true;

        for (int i = 0; i < inputCode.Length; i++)
        {
            if (inputCode[i] == code[i])
            {
                outputText[i].color = Color.green;
            }
            else if (code.Contains(inputCode[i]))
            {
                outputText[i].color = Color.yellow;
                allCorrect = false;
            }
            else
            {
                outputText[i].color = Color.white;
                allCorrect = false;
            }
        }

        if (allCorrect)
        {
            PuzzleSolved();
        }
    }

    private void PuzzleSolved()
    {
        Debug.Log("Puzzle solved!");
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
}
