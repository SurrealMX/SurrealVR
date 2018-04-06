using MarchingCubesProject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MARCHING_MODE { CUBES, TETRAHEDRON };

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class generateStructure : MonoBehaviour
{

    public Material m_material;
    public float surface;

    public MARCHING_MODE mode = MARCHING_MODE.CUBES;

    List<GameObject> meshes = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        Generate();
    }

    void Update()
    {
        transform.Rotate(Vector3.right, 10.0f * Time.deltaTime);
    }

    public CloudFrame LoadPointCloud()
    {
        string filePath = "testc.pcm"; //System.IO.Path.Combine(Application.streamingAssetsPath, fileName) + ".ply";

        return importfile.openPicture("test.pcm").GetCloudFrame(0);

        //return SimpleImporter.Instance.Load(filePath, maximumVertices);
    }

    public void Generate()
    {
        CloudFrame frame = LoadPointCloud();
        createStructure(frame);
    }

    private float[] setupVoxelArray(float[] voxelArray)
    {
        for (int i = 0; i < voxelArray.Length; i++)
            voxelArray[i] = 1;

        return voxelArray;
    }

    private void testVoxel(float[] Voxel, int width, int height, int length)
    {
        List<Vector3> verts = new List<Vector3>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    int idx = x + y * width + z * width * height;
                    if(Voxel[idx] != 0)
                        verts.Add(new Vector3(x, y, z));
                }
            }
        }

        Vector3[] points = verts.ToArray();

        int[] indecies = new int[points.Length];
        for (int i = 0; i < points.Length; i++)
            indecies[i] = i;

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = points;
        mesh.SetIndices(indecies, MeshTopology.Points, 0);
    }

    private void createStructure(CloudFrame frame)
    {
        //Set the mode used to create the mesh.
        //Cubes is faster and creates less verts, tetrahedrons is slower and creates more verts but better represents the mesh surface.
        Marching marching = null;
        if (mode == MARCHING_MODE.TETRAHEDRON)
            marching = new MarchingTertrahedron();
        else
            marching = new MarchingCubes();

        //Surface is the value that represents the surface of mesh
        //For example the perlin noise has a range of -1 to 1 so the mid point is where we want the surface to cut through.
        //The target value does not have to be the mid point it can be any value with in the range.
 
        marching.Surface = surface;  //nothing at 1

        CloudFrame lowResFrame = frame.getLowResCloudFrame(30000);

        //The size of voxel array.
        int width = lowResFrame.width;
        int height = lowResFrame.height;
        int length = 500;

        float[] voxels = setupVoxelArray(new float[width * height * length]);
        float[] points = lowResFrame.GetpointCloud(); ;

        //Fill voxels with values. I take the pointcloud and populate the voxel with it.
        List<float> t = new List<float>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float _z = points[x + y * width];
                int z = 0;
                if (_z >= 0)  //we are removing garbage value(s)... 
                {
                    z = (int) Math.Floor(_z * length);
                    if(z < 1) t.Add(z);

                    //we know that _z is normalized to 1 so we multiply
                    //by the length to get the length in voxels we subtract 1 because index is 0-9 but
                    //the index could be 0-10 so we subtract 1 and clamp to 0 

                    //clamp z at 9
                    if (z>=length-1)
                    {
                        z = length -1;
                    }

                    int idx = x + y * width + z * width * height;
                    try
                    {
                        voxels[idx] = 0;  //put a 1 where the point is because thats where the surface is
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Opps");
                        return;
                    }
                }
                else
                {
                    continue;
                } 
            }
        }

        //test the voxel
        //testVoxel(voxels, width, height, length);


        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();

        //The mesh produced is not optimal. There is one vert for each index.
        //Would need to weld vertices for better quality mesh.
        marching.Generate(voxels, width, height, length, verts, indices);

        //A mesh in unity can only be made up of 65000 verts.
        //Need to split the verts between multiple meshes.

        int maxVertsPerMesh = 30000; //must be divisible by 3, ie 3 verts == 1 triangle
        int numMeshes = verts.Count / maxVertsPerMesh + 1;

        for (int i = 0; i < numMeshes; i++)
        {

            List<Vector3> splitVerts = new List<Vector3>();
            List<int> splitIndices = new List<int>();

            for (int j = 0; j < maxVertsPerMesh; j++)
            {
                int idx = i * maxVertsPerMesh + j;

                if (idx < verts.Count)
                {
                    splitVerts.Add(verts[idx]);
                    splitIndices.Add(j);
                }
            }

            if (splitVerts.Count == 0) continue;

            Mesh mesh = new Mesh();
            mesh.SetVertices(splitVerts);
            mesh.SetTriangles(splitIndices, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            GameObject go = new GameObject("Mesh");
            go.transform.parent = transform;
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            go.GetComponent<Renderer>().material = m_material;
            go.GetComponent<MeshFilter>().mesh = mesh;
            go.transform.localPosition = new Vector3(-width / 2, -height / 2, -length / 2);

            meshes.Add(go);
        }
        transform.Rotate(new Vector3(0, 0, -90));
    }
}
	
