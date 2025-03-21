using AK.Wwise;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWwiseEnvironment", menuName = "Scriptable Objects/WwiseEnvironment")]
public class SO_WwiseEnvironmentData : ScriptableObject
{
    [SerializeField]
    private AuxBus _wwiseAuxBus = default;
    [SerializeField]
    private int _priority = 0;
    [SerializeField]
    private bool _isDefault = false;
    [SerializeField]
    private bool _excludeOthers = false;

    public AuxBus AuxBus => _wwiseAuxBus;
    public bool IsAuxBusValid => IsWwiseAuxBusValid(_wwiseAuxBus);


    public bool SetEnvironmentData(AkEnvironment akEnvironment)
    {
        akEnvironment.priority = _priority;
        akEnvironment.isDefault = _isDefault;
        akEnvironment.excludeOthers = _excludeOthers;
        
        if (!IsAuxBusValid)
            return false;
        
        akEnvironment.data = _wwiseAuxBus;

        return true;
    }


    private bool IsWwiseAuxBusValid(AuxBus wwiseAuxBus)
    {
        if (!wwiseAuxBus.IsValid())
        {
            wwiseAuxBus.Validate();
            return false;
        }

        if (wwiseAuxBus == null)
        {
            Debug.LogErrorFormat("The referenced WwiseAuxBus on {0} is null!", name);
            return false;
        }

        return true;
    }
}
