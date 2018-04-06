using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public static class importfile
{

    public static device openPicture(string FileName)
    {
        //string _Filename = Application.persistentDataPath + "/" + FileName;
        string _Filename = "myMovie2.pcm";
        //string _Filename = "2018-03-22T01_45_52.763+0000.pcm";
        //string _Filename = "2018-03-02T02_45_35.849+0000.pcm";

        if (File.Exists(_Filename))
        {
            using (StreamReader sr = new StreamReader(new FileStream(_Filename, FileMode.Open)))
            {
                string test = "";
                try
                {
                    string picture = sr.ReadToEnd();
                    char delimiter = '\n';
                    string[] frames = picture.Split(delimiter);
                    int numFrames = chk(frames); 

                    if (numFrames < 1)  ////make sure numFrames is not 0
                        throw new Exception("No Data in File...");

                    CloudFrame[] cloudFrames = new CloudFrame[numFrames];
                    //delimiter = ' ';
                    //string[] deviceParams = frames[0].Split(delimiter);

                    //cycle through frames  -- REMEMBER THE FIRST FRAME IS JUST CAMERA PARAMS
                    for (int j = 0; j < numFrames; j++) //j indexes the frames array
                    {
                        test = frames[j];

                        cloudFrames[j] = JsonUtility.FromJson<CloudFrame>(test);
                        //cloudFrames[j-1] = parseJSONofJSON(test, cloudFrames[j - 1]);
                    }
                    device attachedCamera = new device();
                    attachedCamera.cloudFrames = cloudFrames;
                    return attachedCamera;
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
            throw new Exception("File Not Found");
        }
    }

    /*
    private static CloudFrame parseJSONofJSON(string JSONstr, CloudFrame aframe)
    {  //only deals with the frame at the index given by the index
        char[] delimiter = new char[2];
        delimiter[0] = '[';
        delimiter[1] = ']';
        string[] pieces = JSONstr.Split(delimiter);

        string[] str_parsed = pieces[1].Split('}');
        string[] SVs = new string[str_parsed.Length - 1];
        for(int i = 0; i < str_parsed.Length-1; i++)
        {
            if (str_parsed[i].StartsWith(","))
            {
                str_parsed[i] = str_parsed[i].Remove(0, 1);
            }
            SVs[i] = str_parsed[i] + "}";
        }

        SimpleVector test;
        for(int i = 0; i<SVs.Length; i++)
        {
            test = JsonUtility.FromJson<SimpleVector>(SVs[i]);
        }

        return aframe;
    }
    */

    private static int chk(string[] _frames)
    {
        int chkFrames = 0;
        foreach (string test in _frames)
        {
            if (test.ToCharArray().Length != 0)
                chkFrames++;
        }
        return chkFrames;
    }
}
