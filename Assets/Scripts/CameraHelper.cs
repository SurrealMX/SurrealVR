using Assets.scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHelper : MonoBehaviour {

    Vector3[][] pointCloud;
    Color[][] colorCloud;
    CloudFrame[] cloudFrames;
    device Camera;
    public int numFrames;
    importfile reader;

    public int getNumFrames()
    {
        return numFrames;
    }

    private int up = 1;  //default value is 1

    string fileName;

    public int getUpSampleFactor()
    {
        return up;
    }

    public CameraHelper(string fileName)
    {
        this.fileName = fileName;

        //if no upsample rate is specificed then upsample rate is 1
        setClouds(fileName, up);
    }

    public CameraHelper(string _fileName, int _up)
    {
        this.fileName = _fileName;

        if (up > 1)
            up = _up;  //get upsample size

        setClouds(fileName, up);
    }

    private void setClouds(string fileName, int up)
    {
        try
        {
            reader = new importfile(fileName);
            numFrames = reader.getNumFrames();

            cloudFrames = reader.getCloudFrames();
            
        } catch (Exception e)
        {
            Debug.Log(e.Source);
        }
    }

    public CloudFrame getCloudFrame(int numFrame)  //numFgrame controls frame to play;
    {
        return cloudFrames[numFrame];
    }

    private void upsample(int _up)
    {
        Vector3[] newPC = new Vector3[up * pointCloud.Length];
        Color[] newCC = new Color[up * pointCloud.Length];

        if (_up > 1)
        {  //we need to upscale every frame
            for (int k = 0; k < reader.getNumFrames(); k++)
            {
                for (int i = 0; i < pointCloud.Length - 1; i++)
                {
                    for (int j = 0; j < up; j++)
                    {
                        newPC[i + j] = Vector3.Lerp(newPC[i], newPC[i + 1], j / up);

                        //interpolate the color
                        newCC[i + j].a = Mathf.Lerp(newCC[i].a, newCC[i + 1].a, j / up);
                        newCC[i + j].r = Mathf.Lerp(newCC[i].r, newCC[i + 1].r, j / up);
                        newCC[i + j].g = Mathf.Lerp(newCC[i].g, newCC[i + 1].g, j / up);
                        newCC[i + j].b = Mathf.Lerp(newCC[i].b, newCC[i + 1].b, j / up);
                    }
                }
                pointCloud[k] = newPC;
                colorCloud[k] = newCC;
            }

        }
    }
}
