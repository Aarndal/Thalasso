using System;

[Serializable]
public class FloatReference
{
    public bool useReference = true;
    public float constantValue;
    public FloatVariable floatVariable;

    public float Value
    {
        get { return useReference ? floatVariable.value : constantValue; }
    }
}
