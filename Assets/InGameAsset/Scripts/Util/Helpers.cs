using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    static readonly Dictionary<float, WaitForSeconds> WaitDictonary = new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds GetWait(float time)
    {
        if (WaitDictonary.TryGetValue(time, out var wait)) return wait;
        WaitDictonary[time] = new WaitForSeconds(time);
        return WaitDictonary[time];
    }

    public static void DeleteChildren(this Transform t)
    {
        foreach (Transform child in t)
        {
            Object.Destroy(child.gameObject);
        }
    }
}
