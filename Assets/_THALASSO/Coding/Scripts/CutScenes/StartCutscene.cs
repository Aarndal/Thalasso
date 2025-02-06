using Unity.Cinemachine;
using UnityEngine;

public class StartCutscene : MonoBehaviour
{
    [SerializeField] private SO_GameInputReader _input;

    private CinemachineBrain cinemachineBrain;
    private PCAnimation pCAnimation;
    private void Start()
    {
        cinemachineBrain = Object.FindFirstObjectByType<CinemachineBrain>();
        _input.SwitchCurrentActionMapTo("Cutscene");
        cinemachineBrain.enabled = false;
    }
    public void EndCutscene()
    {
        FindAnyObjectByType<PCAnimation>().inCutscene = false;
        cinemachineBrain.enabled = true;
        cinemachineBrain.gameObject.GetComponent<Animator>().enabled = false;
        _input.SwitchCurrentActionMapTo("Player");
    }
}
