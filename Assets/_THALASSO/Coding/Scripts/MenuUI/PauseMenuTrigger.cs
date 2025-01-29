using System;
using UnityEngine;

public class PauseMenuTrigger : MonoBehaviour
{
    [SerializeField]
    private SO_GameInputReader _input;

    private ButtonActions buttonActions;

    private void Awake()
    {
        buttonActions = GetComponent<ButtonActions>();
    }

    private void OnEnable()
    {
        _input.PauseIsPerformed += OnPauseIsPerformed;
    }

    private void OnDisable()
    {
        _input.PauseIsPerformed -= OnPauseIsPerformed;
    }

    private void OnPauseIsPerformed()
    {
        if (buttonActions.pauseMenuToggle.activeSelf)
        {
            GetComponent<ButtonActions>().ResumeGame();
        }
        else
        {
            GetComponent<ButtonActions>().PauseGame();
        }
    }
}
