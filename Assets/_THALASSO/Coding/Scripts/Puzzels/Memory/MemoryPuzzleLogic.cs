using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoryPuzzleLogic : MonoBehaviour
{
    [SerializeField] private Button[] inputButtons;
    [SerializeField] private GameObject[] visualButtonOutput;
    [SerializeField] private GameObject[] visualLightOutput;

    [SerializeField] private int totalRounds = 1;
    [SerializeField] private int startCount = 3;
    [SerializeField] private int countToAddEachRound = 1;
    [SerializeField] private float visualOutputTimeInSec = 1f;

    private List<int> sequence = new List<int>();
    private List<int> inputSequence = new List<int>();
    private int roundCounter = 0;
    private bool inputLocked = true;

    private bool isRunning = false;

    private void Start()
    {
        foreach (var button in inputButtons)
        {
            button.onClick.RemoveAllListeners();
        }

        for (int i = 0; i < inputButtons.Length; i++)
        {
            int buttonIndex = i + 1;
            inputButtons[i].onClick.AddListener(() => NumButtonInput(buttonIndex));
        }
    }

    public void StartPuzzle()
    {
        if (isRunning)
            return;
        if(!isRunning)
            isRunning = true;

        roundCounter = 0;
        NextRound();
    }

    private void GenerateNewSequence()
    {
        System.Random rnd = new System.Random();
        if (roundCounter == 0)
        {
            sequence.Clear();
            for (int i = 0; i < startCount; i++)
            {
                sequence.Add(rnd.Next(1, inputButtons.Length + 1));
            }
        }
        else
        {
            for (int i = 0; i < countToAddEachRound; i++)
            {
                sequence.Add(rnd.Next(1, inputButtons.Length + 1));
            }
        }
    }

    private IEnumerator ShowSequence()
    {
        inputLocked = true;
        foreach (var light in visualLightOutput)
        {
            light.SetActive(false);
        }

        foreach (int index in sequence)
        {
            visualLightOutput[index - 1].SetActive(true);
            yield return new WaitForSeconds(visualOutputTimeInSec);
            visualLightOutput[index - 1].SetActive(false);
        }

        inputLocked = false;
    }

    private void NumButtonInput(int _num)
    {
        if (inputLocked) return;

        inputSequence.Add(_num);
        StartCoroutine(ButtonFeedback(_num));

        if (inputSequence.Count == sequence.Count)
        {
            CheckSequence();
        }
    }

    private IEnumerator ButtonFeedback(int index)
    {
        visualButtonOutput[index - 1].SetActive(false);
        yield return new WaitForSeconds(0.2f);
        visualButtonOutput[index - 1].SetActive(true);
    }

    private void CheckSequence()
    {
        inputLocked = true;

        if (inputSequence.Count != sequence.Count || !SequencesMatch())
        {
            StartCoroutine(ShowRetryFeedback());
            return;
        }

        if (roundCounter >= totalRounds - 1)
        {
            StartCoroutine(ShowSolvedFeedback());
        }
        else
        {
            roundCounter++;
            NextRound();
        }
    }

    private bool SequencesMatch()
    {
        for (int i = 0; i < sequence.Count; i++)
        {
            if (inputSequence[i] != sequence[i])
                return false;
        }
        return true;
    }

    private IEnumerator ShowRetryFeedback()
    {
        foreach (var light in visualLightOutput)
        {
            light.GetComponent<Renderer>().material.color = Color.red;
            light.SetActive(true);
        }

        yield return new WaitForSeconds(1f);
        foreach (var light in visualLightOutput)
        {
            light.SetActive(false);
        }

        inputSequence.Clear();
        yield return ShowSequence();
    }

    private IEnumerator ShowSolvedFeedback()
    {
        foreach (var light in visualLightOutput)
        {
            light.GetComponent<Renderer>().material.color = Color.green;
            light.SetActive(true);
        }

        yield return new WaitForSeconds(2f);
        PuzzleSolved();
    }

    private void NextRound()
    {
        GenerateNewSequence();
        inputSequence.Clear();
        StartCoroutine(ShowSequence());
    }

    private void PuzzleSolved()
    {
        Debug.Log("Puzzle solved");
    }
}
