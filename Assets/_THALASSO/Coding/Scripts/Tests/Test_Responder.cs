using System.Collections;
using UnityEngine;

public class Test_Responder : ResponderBase
{
    [SerializeField]
    private Color _baseColor = Color.black;
    [SerializeField]
    private float _defaultDiscoTime = 5.0f;

    private MeshRenderer _meshRenderer = default;
    private float _discoTime = 0.0f;

    protected override void Awake()
    {
        base.Awake();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    protected virtual void Start()
    {
        _meshRenderer.material.color = _baseColor;
        _discoTime = _defaultDiscoTime;
    }

    public override bool Respond(IAmTriggerable trigger)
    {
        if(_discoTime < _defaultDiscoTime)
            return false;

        StartCoroutine(Disco());
        return true;
    }

    private IEnumerator Disco()
    {
        while (_discoTime >= 0.0f)
        {
            GetComponent<MeshRenderer>().material.color = UnityEngine.Random.ColorHSV(0.0f, 1.0f, 0.75f, 1.0f, 0.5f, 1.0f);

            _discoTime -= 0.1f;

            yield return new WaitForSeconds(0.1f);
        }

        _meshRenderer.material.color = _baseColor;
        _discoTime = _defaultDiscoTime;
        yield return null;
    }
}
