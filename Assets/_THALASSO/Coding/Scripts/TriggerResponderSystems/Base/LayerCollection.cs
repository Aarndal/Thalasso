using System;
using System.Collections.Generic;
using System.Linq;

public static class LayerCollection
{
    private static readonly Dictionary<Layers, int> _layers;

    static LayerCollection()
    {
        _layers = Enum.GetValues(typeof(Layers)).Cast<Layers>().ToDictionary(e => e, e => (int)e);
    }

    public static Dictionary<Layers, int> Layers => _layers;
}
