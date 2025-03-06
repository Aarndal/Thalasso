using System;

[Flags]
public enum LayerMaskFlags
{
    Default = 1 << Layers.Default,
    TransparentFX = 1 << Layers.TransparentFX,
    IgnoreRaycast = 1 << Layers.IgnoreRaycast,
    Water = 1 << Layers.Water,
    UI = 1 << Layers.UI,
    Ground = 1 << Layers.Ground,
    Player = 1 << Layers.Player,
    InteractiveObject = 1 << Layers.InteractiveObject,
}
