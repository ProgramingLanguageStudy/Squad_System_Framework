using System.Collections.Generic;
using UnityEngine;

public class FlagManager : MonoBehaviour
{
    private Dictionary<string, int> _flags = new Dictionary<string, int>();

    public void SetFlag(string key, int value) => _flags[key] = value;
    public int GetFlag(string key) => _flags.ContainsKey(key) ? _flags[key] : 0;
}