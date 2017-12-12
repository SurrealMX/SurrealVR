using Assets.scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class depthViewer : MonoBehaviour
{
    Shader myShader;

    private Vector3[] pointCloud;
    private GameObject Structure;
    private Vector3[] normals;
    private Color[] colorCloud;
    private device Camera;

    //variables for rendering
    public float psize;

    private CameraHelper helper;
    public bool restart;
    public bool play;
    public bool loop;
    public float elapsedTime;
    private float startTime;
    public string fileName;

    public int currentFrame;

    public float threshold;
    public int Factor;
    public int numFramesinCamera;

    // Use this for initialization
    void Start()
    {
        myShader = Shader.Find("Unlit/Texture_color");

        fileName = "myMovie.pcm";
        init(fileName);
        //normals = createSurfaceNormals(pointCloud);
        //generateStructure();

        //play, plause, restart
        restart = false;
        play = false;
        loop = false;

        currentFrame = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentFrame == 0)
        {
            CloudFrame frame = helper.getCloudFrame(currentFrame);
            renderFrame(frame);
        }

        if(currentFrame == 0 && play)  //we weren't playing before but we are now playing from the bigenning
        {
            startTime = Time.time;
            currentFrame++;
        }

        //currently playing so we need to render the next frame
        if(currentFrame < numFramesinCamera && play)
        {
            CloudFrame frame = helper.getCloudFrame(currentFrame);
            float test = frame.t;
            elapsedTime = Time.time - startTime;
            if ((elapsedTime >= frame.t))
            {
                renderFrame(frame);
                currentFrame++;
            }
        }

        //at the end
        if(currentFrame == numFramesinCamera && play && loop)
        {
            currentFrame = 0;
        }
        else if (currentFrame == numFramesinCamera && play && !loop)
        {
            play = false;
            currentFrame = 0;
        }
      
    }

    private void init(string fileName)
    {
        helper = new CameraHelper(fileName);
        numFramesinCamera = helper.getNumFrames();
        //Camera = reader.getCamera();
        //pointCloud = reader.getPointCloud();
        //colorCloud = reader.getColorCloud();
    }

    private void renderFrame(CloudFrame _frame)
    {
        pointCloud = _frame.p;
        colorCloud = _frame.c;

        if(Structure == null)
        {
            generateStructure();
            refresh();
            Structure.transform.parent = gameObject.GetComponent<Transform>();
            Structure.transform.Rotate(new Vector3(0, 0, 1), 180);
            Structure.transform.localPosition = new Vector3(0, 0, 0);
        }
        else if(play)
        {
            refresh();
        }

    }

    private void refresh()
    {
        //first find all the children of the structure (these are the gameObjs from the previous pointCloud
        Transform[] childrenT = Structure.GetComponentsInChildren<Transform>();
        Renderer[] childrenR = Structure.GetComponentsInChildren<Renderer>();
        //GameObject[] pointObjArray = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < pointCloud.Length; i++) //cycle through the new pointCloud and update the location of each pointObj
        {
            try
            {
                if (pointCloud[i] != Vector3.zero)
                {
                    childrenT[i].localPosition = pointCloud[i];
                    childrenR[i].enabled = true;
                    //if (renderer != null)
                    //renderer.material.color = colorCloud[i];
                    childrenR[i].material.SetColor("_Color", colorCloud[i]);
                }
                else
                    childrenR[i].enabled = false;
            }
            catch (Exception e)
            {
                Debug.Log("oops");
            }

        }
    }

    void generateStructure()
    {
        //so the idea here is that this method creates a game object of what the camera sees
        if ((pointCloud != null))  //if the point cloud isn't null
        {
            Structure = new GameObject("Depth Photo");

            foreach(Vector3 point in pointCloud)
            {
                GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //Debug.Log(pointCloud[j].x.ToString());
                jointObj.transform.parent = Structure.transform;
                jointObj.transform.localPosition = point;  //move the center point of the sphere to the location of the point
                jointObj.tag = "Player";
                jointObj.transform.localScale = new Vector3(psize, psize, psize);  //make it a really tiny shphere
                Renderer renderer = jointObj.GetComponent<Renderer>();
                renderer.material.shader = myShader;

                Destroy(jointObj.GetComponent<BoxCollider>());
            }

                //Vector3 test = normals[i];
                //jointObj.transform.LookAt(test);
            }
        }
    }

    /*
    private Vector3[] createSurfaceNormals(Vector3[] pointCloud)
    {
        int H = Camera.getHeight();
        int W = Camera.getWidth();
        int skip = Camera.getSkip();

        Vector3[] n = new Vector3[pointCloud.Length];
        //KDTree tree = KDTree.MakeFromPoints(pointCloud);

        for (int i = 0; i < pointCloud.Length-W/skip; i++)
        {
            int N = i + 1;
            int B = W / skip + i;

            if(i!= W)
            {
                Vector3 V0 = pointCloud[i];
                Vector3 V1 = pointCloud[N];
                Vector3 V2 = pointCloud[B];

                if (V0 != new Vector3(0, 0, 0))
                {
                    float D1 = Vector3.Distance(V0, V1);
                    float D2 = Vector3.Distance(V0, V2);
                    Debug.Log("ahhhh");

                    if ((D1 <= threshold) && (D2 <= threshold))
                    {
                        Plane tempPlane = new Plane();
                        tempPlane.Set3Points(V0, V1, V2);
                        n[i] = tempPlane.normal;
                    }
                }
            }
        }
        return n;
    }
    */

