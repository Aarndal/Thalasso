using UnityEngine;

public class PauseMenuTrigger : MonoBehaviour
{
    private ButtonActions buttonActions;
    private void Start()
    {
        buttonActions = GetComponent<ButtonActions>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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
}
