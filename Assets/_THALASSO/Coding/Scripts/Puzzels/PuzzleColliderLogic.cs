using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PuzzleColliderLogic : MonoBehaviour
{
    [SerializeField] private string playerTag;
    [SerializeField] private CinemachineVirtualCamera cinemachineCamera;
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
        Debug.Log("Listening for input");
        bool isfocused = false;
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!isfocused)
                {

                    Debug.Log("focus");
                    cinemachineCamera.enabled = false;
                    camAnim.FocusPuzzle();
                    isfocused = true;
                }
                else
                {
                    Debug.Log("unfocus");
                    camAnim.UnfocusPuzzle(cinemachineCamera);
                    isfocused = false;
                }
            }
            yield return null;
        }
    }
}
