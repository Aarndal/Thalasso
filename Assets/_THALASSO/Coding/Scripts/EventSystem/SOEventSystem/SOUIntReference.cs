using System;

[Serializable]
public class SOUIntReference
{
    public bool useReference = true;
    public uint constantValue;
    public SO_UIntVariable uintVariable;

    public uint Value
    {
        get { return useReference ? uintVariable.Value : constantValue; }
    }
}
