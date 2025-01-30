using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostWwiseEvent : MonoBehaviour
{
    [SerializeField]
    private AK.Wwise.Event wwiseEvents;

    [SerializeField]
    private Animator _animator;

    private void Awake()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();

            if (_animator == null)
                Debug.LogError("No Animator component found on " + gameObject.name);
        }
    }

    public void PlayFootStep()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            wwiseEvents.Post(gameObject);
        }
    }
}
