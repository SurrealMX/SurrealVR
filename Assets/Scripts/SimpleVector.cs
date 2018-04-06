using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SimpleVector
{

    /* Converts from a unity vector to a point and backagain*/
    [SerializeField]
    public float X;
    [SerializeField]
    public float Y;
    [SerializeField]
    public float Z;

    [SerializeField]
    public float A;
    [SerializeField]
    public float R;
    [SerializeField]
    public float G;
    [SerializeField]
    public float B;

    /// <summary>
    /// Initialize a simple vector object.  xyz coordinates are floats.  colors are bytes.
    /// </summary>
    public SimpleVector(Vector3 aVector, UnityEngine.Color32 aColor)
    {
        X = aVector.x;
        Y = aVector.y;
        Z = aVector.z;

        A = aColor.a;
        R = aColor.r;
        G = aColor.g;
        B = aColor.b;
    }

    /// <summary>
    /// returns a unity vector3 associated with the xyz coordinates
    /// </summary>
    public Vector3 toVector3()
    {
        return new Vector3(X, Y, Z);
    }

    /// <summary>
    /// Return a unity color vector associated with the simple vector
    /// </summary>
    public Color toColor()
    {
        return new Color(R, G, B, A);
    }


}