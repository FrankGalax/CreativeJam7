using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public struct PlanetZone
{
    public string Name;
    public Color Color;
    public List<PlanetFragment> Fragments;
}