using System.Collections;
using UnityEngine;

public class PuzzleColliderLogic : MonoBehaviour
{
    [SerializeField] private string playerTag;
    private CamAnimation camAnim;
    private Coroutine curCoroutine;
    private void Start()
    {
        camAnim = GetComponent<CamAnimation>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            curCoroutine = StartCoroutine(AcceptInput());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            StopCoroutine(curCoroutine);
        }
    }

    private IEnumerator AcceptInput()
    {
        bool isfocused = false;
        while (true)
        {
            Debug.Log("acceptInput");
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!isfocused)
                {
                    camAnim.FocusPuzzle();
                    isfocused = true;
                }
                else
                {
                    camAnim.UnfocusPuzzle();
                    isfocused = false;
                }
            }
            yield return null;
        }
    }
}
