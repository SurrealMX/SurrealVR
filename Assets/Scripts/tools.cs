using System;
using UnityEngine;

public static class tools
{
    public static void DestroyChildren(this Transform root)
    {
        int childCount = root.childCount;
        for(int i = 0; i<childCount; i++)
        {
            GameObject.Destroy(root.GetChild(0).gameObject);
        }
    }
}
