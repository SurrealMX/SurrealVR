using System;
using UnityEngine;

namespace Assets.scripts
{
    [Serializable]
    public class CloudFrame
    {
        public float t;
        public Vector3[] p;
        public Color[] c;

        public CloudFrame(Vector3[] somepc, Color[] someCC, float timestamp)
        {
            t = timestamp;
            p = somepc;
            c = someCC;
        }
    }
}
