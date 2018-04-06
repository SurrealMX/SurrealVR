using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class device
{
    //this class just stores camera parameters and all the frames loaded from the camera

    public int height { get; set; }
    public int width { get; set; }
    public bool type { get; set; }  //type 0 = regular picture and type 1 = picture with depth information
    public int numFrames { get; set; }
    public CloudFrame[] cloudFrames { get; set; }

    public device()
    {

    }

    public device(int height, int width, bool type, int numFrames)
    {
        this.height = height;
        this.width = width;
        this.type = type;
        this.numFrames = numFrames;
    }

    public CloudFrame GetCloudFrame(int index)
    {  //gets the cloud frame at a particular index
        return cloudFrames[index];
    }
}

