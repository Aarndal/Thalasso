using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test_SceneSwitch : Responder
{
    [SerializeField]
    private SceneReference _sceneToLoad;

    public override void Respond(GameObject triggeringObject, ResponderState responderState)
    {
        SceneManager.LoadScene(_sceneToLoad.Name, LoadSceneMode.Single);
    }
}
