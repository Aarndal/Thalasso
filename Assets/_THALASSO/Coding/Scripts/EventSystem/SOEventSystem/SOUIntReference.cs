using System;

[Serializable]
public class SOUIntReference
{
    public bool useReference = true;
    public uint constantValue;
    public SO_UIntVariable uintVariable;

    public float Value
    {
        get { return useReference ? uintVariable.value : constantValue; }
    }
}
