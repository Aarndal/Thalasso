using UnityEngine;

public class QuitGameButton : ButtonClick
{
    protected override void OnClicked()
    {
        base.OnClicked();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
