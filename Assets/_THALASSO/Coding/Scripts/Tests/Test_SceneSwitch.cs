using UnityEngine.SceneManagement;
using UnityEngine;

public class Test_SceneSwitch : MonoBehaviour
{
    private Scene _currentScene;
    [SerializeField]
    private int _sceneToLoadIndex;

    private void Start()
    {
        _currentScene = SceneManager.GetActiveScene();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(_sceneToLoadIndex, LoadSceneMode.Single);
        }
    }
}
