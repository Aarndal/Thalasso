using System;

[Serializable]
public class BoolReference
{
    public bool useReference = true;
    public bool constantValue;
    public BoolVariable boolVariable;

    public bool Value
    {
        get { return useReference ? boolVariable.value : constantValue; }
    }
}
