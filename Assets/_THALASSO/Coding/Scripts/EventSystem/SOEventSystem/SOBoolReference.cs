using System;

[Serializable]
public class SOBoolReference
{
    public bool useReference = true;
    public bool constantValue;
    public SO_BoolVariable boolVariable;

    public bool Value
    {
        get { return useReference ? boolVariable.value : constantValue; }
    }
}
