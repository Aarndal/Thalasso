using System.Threading.Tasks;
using UnityEngine;

public class QuitGameButton : ButtonClick
{
    [SerializeField]
    private int _delayInMilliseconds = 1500;

    protected async override void OnClicked()
    {
        base.OnClicked();

        await Task.Delay(_delayInMilliseconds);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
