using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class device
{
    //this class just stores camera parameters

    private int height;
    private int width;
    private int skip;

    public device(int height, int width, int skip)
    {
        this.height = height;
        this.width = width;
        this.skip = skip;
    }

    public int getHeight()
    {
        return height;
    }
    
    public int getWidth()
    {
        return width;
    } 

    public int getSkip()
    {
        return skip;
    }
}

