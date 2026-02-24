using System.Collections.Generic;
using UnityEngine;

public class FlagManager : MonoBehaviour
{
    private Dictionary<string, int> _flags = new Dictionary<string, int>();

    public void SetFlag(string key, int value) => _flags[key] = value;

    /// <summary>현재 값에 value를 더함. 없으면 0으로 간주.</summary>
    public void AddFlag(string key, int value) => SetFlag(key, GetFlag(key) + value);

    public int GetFlag(string key) => _flags.ContainsKey(key) ? _flags[key] : 0;
}