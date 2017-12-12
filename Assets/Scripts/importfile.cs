using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Assets.scripts;

public class importfile {

    public Boolean ready = false;
    private int numFrames;  //counts the number of frames minus the param frame
    private device Camera;
    private string fileName;
    private CloudFrame[] cloudFrames;

    public importfile(string _fileName)
    {
        this.fileName = _fileName;
        openPicture(this.fileName);
    }

    public int getNumFrames()
    {
        return numFrames;
    }

    public CloudFrame[] getCloudFrames()
    {
        return cloudFrames;
    }

    public device getCamera()
    {
        return Camera;
    }

    public void openPicture(string FileName)
    {
        if (File.Exists(FileName))
        {
            using (StreamReader sr = new StreamReader(FileName))
            {
                string test = "";
                try
                {
                    string picture = sr.ReadToEnd();
                    char delimiter = '\n';
                    string[] frames = picture.Split(delimiter);
                    numFrames = chk(frames) - 1; //the first frame is always just the camera params

                    if(numFrames < 1)  ////make sure numFrames is not 0
                        throw new Exception("No Data in File...");

                    cloudFrames = new CloudFrame[numFrames];
                    delimiter = ' ';
                    string[] deviceParams = frames[0].Split(delimiter);
                    Camera = new device(int.Parse(deviceParams[0]), int.Parse(deviceParams[1]), int.Parse(deviceParams[2]));

                    //cycle through frames  -- REMEMBER THE FIRST FRAME IS JUST CAMERA PARAMS
                    for (int j = 1; j<=numFrames; j++) //j indexes the frames array
                    {
                        test = frames[j];
              
                        cloudFrames[j - 1] = JsonUtility.FromJson<CloudFrame>(test);
                    }
                }
                catch (Exception e)
                {
                    e.Source = test;
                    throw e;
                }
            }
        }
        else
        {
            Exception e = new Exception();
            throw e;
        }
    }

    private int chk(string[] _frames)
    {
        int chkFrames = 0;
        foreach(string test in _frames)
        {
            if (test.ToCharArray().Length != 0)
                chkFrames++;
        }
        return chkFrames;
    }
}
