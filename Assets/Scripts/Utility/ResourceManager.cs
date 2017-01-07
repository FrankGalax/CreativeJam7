using System;
using System.Collections.Generic;
using UnityEngine;

public static class ResourceManager
{
    private static Dictionary<string, Texture2D> _textures;
    private static Dictionary<string, GameObject> _prefabs;

    static ResourceManager()
    {
        _textures = new Dictionary<string, Texture2D>();
        _prefabs = new Dictionary<string, GameObject>();
    }

    public static Texture2D GetTexture(string name)
    {
        if (_textures.ContainsKey(name))
            return _textures[name];

        Texture2D texture = Resources.Load(name) as Texture2D;
        _textures.Add(name, texture);
        return texture;
    }

    public static GameObject GetPrefab(string name)
    {
        if (_prefabs.ContainsKey(name))
            return _prefabs[name];

        GameObject prefab = Resources.Load(name) as GameObject;
        _prefabs.Add(name, prefab);
        return prefab;
    }
}