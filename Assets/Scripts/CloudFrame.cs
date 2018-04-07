using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CloudFrame
{
    [SerializeField]
    private float t;
    [SerializeField]
    private float[] p;//public SimpleVector[] p;
    [SerializeField]
    public int height;
    [SerializeField]
    public int width;
    [SerializeField]
    public float psize;


    /// <summary>
    /// Receives a vector3 array, a color array and a timestamp
    /// Converts this into a serializable cloudframe and initializes the object
    /// </summary>
    public CloudFrame(Vector3[] somepc, Color32[] somecc, float timestamp, int height, int width)
    {
        this.height = height;
        this.width = width;
        p = new float[somepc.Length];//new SimpleVector[somepc.Length];
        t = timestamp;

        for (int i = 0; i < somepc.Length; i++)
            p[i] = somepc[i].z;//new SimpleVector(somepc[i], somecc[i]);
    }

    public CloudFrame(float[] Ps, float timestamp, int height, int width)
    {
        //debug(Ps);
        this.height = height;
        this.width = width;
        p = Ps;
        debug(p);
        t = timestamp;
    }

    public int getVertexCount()
    {
        int count = 0;
        for (int i = 0; i < p.Length; i++)
        {
            if (p[i] >= 0)
                count++;
        }
        return count;
    }

    public void reScale(float pA, float pB)
    {
        List<float> rescaledPs = new List<float>();

        for (int i = 0; i < p.Length; i++)
        {
            if (pA <= p[i] && pB >= p[i])
                rescaledPs.Add(p[i]);
        }

        float[] Ps = rescaledPs.ToArray();
        Ps = normalize(Ps);
    }

    private float[] normalize(float[] Ps)
    {
        float max = Ps[0];
        float min = 1;
        foreach (float point in Ps)
        {
            if (point > max)
                max = point;
            if (point < min && point >= 0)
                min = point;
        }

        for(int i = 0; i<Ps.Length; i++)
        {
            float num = Ps[i] - min;
            float den = max-min;
            Ps[i] = num / den;  //I want this value to be between 0 and 1
            //if (Ps[i] == 0)
              //  Debug.Log("Min at: " + i.ToString());
           // if (Ps[i] == 1)
               // Debug.Log("Max at: " + i.ToString());
        }

        return Ps;
    }

    /// <summary>
    /// Returns a unity vector3 array associated with simplevector array stored
    /// with the object.
    /// </summary>1
    public Vector3[] GetpointCloud(float scale)
    {
        List<Vector3> pointCloud = new List<Vector3>();

        for (int x = 0; x < width; x++) 
        {
            for (int y = 0; y < height; y++)
            {
                int i = x + y * width;
                float xval = (float)x * scale;
                float yval = (float)y * scale;
                Vector3 test = Vector3.zero;
                if (p[i] >= 0)
                    pointCloud.Add(new Vector3(xval, yval, p[i]));
                }
            }
        return pointCloud.ToArray();
    }

    /// <summary>
    /// Returns the points which is float[]
    /// </summary>1
    public float[] GetpointCloud()
    {
        //debug(p);
        return p;
    }

    /// <summary>
    /// returns a low resolution cloud frame for mobile applications. 
    /// </summary>1
    public CloudFrame getLowResCloudFrame(int MaxVertex)
    {  //the point of this function is to create a low poly model for mobile
        int multiplier = (int) Math.Sqrt(getVertexCount() / MaxVertex);

        //Vector3[] pointCloud = new Vector3[p.Length];
        List<float> newP = new List<float>();
        //Todo: store color

        int _height = this.height/multiplier;
        int _width = this.width/multiplier;
        //bool check = false;  //very hacky

        //this will downsample the pointCloud
        for (int y = 0; y < height; y = y+multiplier)
        {
            for (int x = 0; x < width; x=x+multiplier)
            {
                int i = x + y * width;
                //add the current z to the vector.
                newP.Add(p[i]);
            }
        }

        //now we have the new points (these have been down sampled), lets rescale them
        float[] normPs = normalize(newP.ToArray());
        //debug(normPs);

        return new CloudFrame(normPs, t, _height, _width);
    }

    public Vector3[] getNormals(Vector3[] pointCloud)
    {
        int height = this.height;
        int width = this.width;

        Vector3[] Normals = new Vector3[pointCloud.Length];

        for (int i = 0; i < pointCloud.Length; i++)
        {
            if((i < pointCloud.Length-width-1) && (i % (width-1) != 0))
            {
                int right = i + 1;
                int down = width + i + 1;
                Vector3 b = pointCloud[right];
                Vector3 a = pointCloud[i];
                Vector3 c = pointCloud[down];

                Plane tempPlane = new Plane();
                tempPlane.Set3Points(a, b, c);
                Normals[i] = tempPlane.normal;

            } else
            {
                Normals[i] = Vector3.zero;
            }
        }

        return Normals;
    }

    private void debug(float[] points)
    {
        float max = 0;
        float min = 1;
        foreach (float point in points)
        {
            if (point > max)
                max = point;
            if (point < min && point >= 0)
                min = point;
        }
        System.Console.WriteLine("Min is: " + min.ToString());
        System.Console.WriteLine("Max is: " + max.ToString());
    }

    /// <summary>
    /// Returns a unity color array associated with the color array associated 
    /// with the simpleVector stored in the object. 
    /// </summary>
    public Color[] GetColorCloud()
    {  //make sure the colorCloud is the same length as the pointCloud
        Color[] colorCloud = new Color[p.Length];

        for (int i = 0; i < p.Length; i++)
        {
            if (p[i] >= 0)
            {
                Color aColor = new Color(0, 0, 0);
                colorCloud[i] = aColor;
            }
        }

        return colorCloud;
    }
}
