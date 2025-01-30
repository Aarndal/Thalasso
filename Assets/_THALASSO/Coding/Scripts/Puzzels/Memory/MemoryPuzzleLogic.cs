using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoryPuzzleLogic : MonoBehaviour
{
    [SerializeField] private GameObject[] visualButtonOutput;
    [SerializeField] private GameObject[] visualLightOutput;

    [SerializeField] private int totalRounds = 3;
    [SerializeField] private int startCount = 2;
    [SerializeField] private int countToAddEachRound = 2;
    [SerializeField] private float visualOutputTimeInSec = 0.5f;

    [SerializeField] private float buttonVisualFeedbackTravelDistance = 0.1f;
    [SerializeField] private float buttonVisualFeedbackTravelDuration = 0.5f;


    [SerializeField] private int puzzleID = 2;

    private List<int> sequence = new List<int>();
    private List<int> inputSequence = new List<int>();
    private int roundCounter = 0;
    private bool inputLocked = true;

    private bool isRunning = false;

    private Button[] inputButtons;
    private void Awake()
    {
        PuzzleUIReferencesSender.puzzleUIReferenceLogger += GetUIReference;
    }

    private void GetUIReference(GameObject reference, int ID)
    {
        if (ID == puzzleID)
        {
            Button[] allChildWithButtons = reference.transform.GetComponentsInChildren<Button>();
            inputButtons = new Button[allChildWithButtons.Length];
            for (int i = 0; i < allChildWithButtons.Length; i++)
            {
                inputButtons[i] = allChildWithButtons[i];
            }
        }
    }
    private void Start()
    {
        foreach (Button button in inputButtons)
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
        StartCoroutine(NextRound());
    }

    private IEnumerator NextRound()
    {
        yield return new WaitForSeconds(1f);

        GenerateNewSequence();
        inputSequence.Clear();
        StartCoroutine(ShowSequence());
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

    private IEnumerator ShowSequence() //tempVisual
    {
        inputLocked = true;
        foreach (GameObject light in visualLightOutput)
        {
            light.SetActive(false);
        }

        foreach (int index in sequence)
        {
            visualLightOutput[index - 1].SetActive(true);
            yield return new WaitForSeconds(visualOutputTimeInSec);
            visualLightOutput[index - 1].SetActive(false);
            yield return new WaitForSeconds(0.2f);
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

    private IEnumerator ButtonFeedback(int index) //tempVisual
    {
        GameObject button = visualButtonOutput[index - 1].gameObject;
        Vector3 originPos = button.transform.position;

        Vector3 newPos = originPos + Vector3.down * buttonVisualFeedbackTravelDistance;

        TransformTransitionSystem.Instance.TransitionPos(button, newPos, buttonVisualFeedbackTravelDuration);
        yield return new WaitForSeconds(buttonVisualFeedbackTravelDuration + 0.2f);

        TransformTransitionSystem.Instance.TransitionPos(button, originPos, buttonVisualFeedbackTravelDuration);

    }

    private void CheckSequence()
    {
        inputLocked = true;

        if (inputSequence.Count != sequence.Count || !CheckInput())
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
            StartCoroutine(NextRound());
        }
    }

    private bool CheckInput()
    {
        for (int i = 0; i < sequence.Count; i++)
        {
            if (inputSequence[i] != sequence[i])
                return false;
        }
        return true;
    }

    private IEnumerator ShowRetryFeedback() //tempVisual
    {
        foreach (GameObject light in visualLightOutput)
        {
            light.GetComponent<Renderer>().material.color = Color.red;
            light.SetActive(true);
        }

        yield return new WaitForSeconds(1f);
        foreach (GameObject light in visualLightOutput)
        {
            light.GetComponent<Renderer>().material.color = Color.gray;
            light.SetActive(false);
        }


        yield return new WaitForSeconds(1f);
        inputSequence.Clear();
        yield return ShowSequence();
    }

    private IEnumerator ShowSolvedFeedback() //tempVisual
    {
        foreach (GameObject light in visualLightOutput)
        {
            light.GetComponent<Renderer>().material.color = Color.green;
            light.SetActive(true);
        }

        yield return new WaitForSeconds(0.5f);
        PuzzleSolved();
    }

    

    private void PuzzleSolved()
    {
        StopAllCoroutines();
        Debug.Log("Puzzle solved");
    }
}
