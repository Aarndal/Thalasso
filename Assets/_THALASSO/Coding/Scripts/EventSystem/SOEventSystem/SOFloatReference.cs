using System;

[Serializable]
public class SOFloatReference
{
    public bool useReference = true;
    public float constantValue;
    public SO_FloatVariable floatVariable;

    public float Value
    {
        get { return useReference ? floatVariable.value : constantValue; }
    }
}
