using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MasterMindPuzzleLogic : MonoBehaviour
{
    [SerializeField] private Button[] numButtons;
    [SerializeField] private TextMeshProUGUI[] outputText;

    private int[] code = new int[4];
    private int[] inputCode = new int[4];

    private bool inputLocked = false;


    private void Start()
    {
        GenerateNewCode();
        for (int i = 0; i < numButtons.Length; i++)
        {
            int buttonIndex = i + 1;
            numButtons[i].onClick.AddListener(() => NumButtonInput(buttonIndex));
        }
    }

    private void GenerateNewCode()
    {
        System.Random rnd = new System.Random();
        List<int> availableNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        for (int i = 0; i < code.Length; i++)
        {
            int randomIndex = rnd.Next(availableNumbers.Count);
            code[i] = availableNumbers[randomIndex];
            availableNumbers.RemoveAt(randomIndex);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    int inputIndex = 0;
    public void NumButtonInput(int _num)
    {
        if(inputLocked) 
            return;

        if (inputIndex == 3)
        {
            inputCode[inputIndex] = _num;
            outputText[inputIndex].text = _num.ToString();
            CheckInput();
            inputIndex = 0;
        }
        else
        {
            inputCode[inputIndex] = _num;
            outputText[inputIndex].text = _num.ToString();
            inputIndex++;
        }

    }

    private void CheckInput()
    {
        bool allCorrect = true;
        inputLocked = true;

        for (int i = 0; i < inputCode.Length; i++)
        {
            if (inputCode[i] == code[i])
            {
                outputText[i].color = Color.green;
            }
            else if (System.Array.Exists(code, element => element == inputCode[i]))
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
        else
        {
            StartCoroutine(ResetAfterSeconds(3));
        }
    }

    private void PuzzleSolved()
    {
        Debug.Log("Puzzle solved!");
    }

    private IEnumerator ResetAfterSeconds(int seconds)
    {
        yield return new WaitForSeconds(seconds);

        inputIndex = 0;
        for (int i = 0; i < outputText.Length; i++)
        {
            outputText[i].text = "";
            outputText[i].color = Color.white;
        }

        inputLocked = false;
    }
}
