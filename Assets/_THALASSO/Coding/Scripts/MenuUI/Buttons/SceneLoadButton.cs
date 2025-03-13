using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadButton : ButtonClick
{
    [SerializeField]
    private SceneReference _scene = default;
    [SerializeField]
    private bool _loadAsynchronosly = true;

    protected override void OnClicked()
    {
        base.OnClicked();

        if (_loadAsynchronosly)
            SceneManager.LoadSceneAsync(_scene.BuildIndex);
        else
            SceneManager.LoadScene(_scene.BuildIndex);
    }
}
