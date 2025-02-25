using AK.Wwise;
using UnityEngine;

public class SoundMaterial : MonoBehaviour
{
    [SerializeField]
    private Switch _soundMaterial;

    public Switch Get() => _soundMaterial;
}
