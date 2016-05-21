﻿using UnityEngine;
using System.Collections;
using MarchingCubesProject;

public class ProceduralGeneration : MonoBehaviour
{
    public const int width = 32;
    public const int height = 32;
    public const int length = 32;
    public const int k = 1;
    public const float scale = 5f;
    public const int numSeeds = 10;

    public Material m_material;
    GameObject m_mesh;

    /// <summary>
    /// Returns the closest wrapped seed point closest to the test point.
    /// </summary>
    /// <param name="testPoint"></param>
    /// <param name="seed"></param>
    /// <returns></returns>
    Vector3 minOfMirrors(Vector3 testPoint, Vector3 seed)
    {
        float diff = seed.x - testPoint.x;
        if (diff > width / 2)
        {
            seed.x -= width;
        }
        else if (diff < -width / 2)
        {
            seed.x += width;
        }

        diff = seed.y - testPoint.y;
        if (diff > height / 2)
        {
            seed.y -= height;
        }
        else if (diff < -height / 2)
        {
            seed.y += height;
        }

        diff = seed.z - testPoint.z;
        if (diff > length / 2)
        {
            seed.z -= length;
        }
        else if (diff < -length / 2)
        {
            seed.z += length;
        }

        return seed;
    }

    float smin(float[] values, float k = 1)
    {
        float result = 0;
        for (int i = 0; i < values.Length; i++)
        {
            result += Mathf.Exp(-k * values[i]);
        }
        return -Mathf.Log(result) / k;
    }

    float sminDistance(Vector3 queryPoint, Vector3[] seeds, float k = 1)
    {
        float[] distances = new float[seeds.Length];
        for (int i = 0; i < seeds.Length; i++)
        {
            distances[i] = distanceOfMirrors(queryPoint, seeds[i]);
        }
        return smin(distances, k);
    }

    float distanceOfMirrors(Vector3 testPoint, Vector3 seed)
    {
        return Vector3.Distance(testPoint, minOfMirrors(testPoint, seed));
    }

    // Use this for initialization
    void Awake()
    {
        //Random.seed = 2;

        //Target is the value that represents the surface of mesh
        //For example the perlin noise has a range of -1 to 1 so the mid point is were we want the surface to cut through
        //The target value does not have to be the mid point it can be any value with in the range
        MarchingCubes.SetTarget(-0.925f);

        //Winding order of triangles use 2,1,0 or 0,1,2
        MarchingCubes.SetWindingOrder(2, 1, 0);

        //Set the mode used to create the mesh
        //Cubes is faster and creates less verts, tetrahedrons is slower and creates more verts but better represents the mesh surface
        MarchingCubes.SetModeToCubes();
        //MarchingCubes.SetModeToTetrahedrons();

        //the index of the closest voronoi seed
        int[, ,] voxelVoronoi = new int[width + 1, height + 1, length + 1];
        //smoothmin distances
        float[, ,] voxelSmoothMin = new float[width + 1, height + 1, length + 1];
        //final values
        float[, ,] voxels = new float[width + 1, height + 1, length + 1];
        int x, y, z;

        float start = Time.realtimeSinceStartup;

        Vector3[] seeds = new Vector3[numSeeds];

        for (int i = 0; i < seeds.Length; i++)
        {
            seeds[i] = new Vector3(Random.value * (width + 1), Random.value * (height + 1), Random.value * (length + 1));
        }

        //populate the voronoi/smoothmin arrays
        for (x = 0; x < width + 1; x++)
        {
            for (y = 0; y < height + 1; y++)
            {
                for (z = 0; z < length + 1; z++)
                {
                    Vector3 testPoint = new Vector3(x, y, z);

                    int closestSeedIndex = 0;
                    float distance = distanceOfMirrors(testPoint, seeds[0]);

                    for (int i = 1; i < seeds.Length; i++)
                    {
                        float queryDistance = distanceOfMirrors(testPoint, seeds[i]);
                        if (queryDistance < distance)
                        {
                            distance = queryDistance;
                            closestSeedIndex = i;
                        }
                    }

                    voxelVoronoi[x, y, z] = closestSeedIndex;

                    voxelSmoothMin[x, y, z] = sminDistance(testPoint, seeds, k);
                }
            }
        }
        int yesCount = 0;
        int noCount = 0;

        //Create a mesh for each seed
        for (int i = 0; i < seeds.Length; i++)
        {
            //Fill voxels with values. Im using perlin noise but any method to create voxels will work
            for (x = 0; x < width + 1; x++)
            {
                for (y = 0; y < height + 1; y++)
                {
                    for (z = 0; z < length + 1; z++)
                    {
                        if (voxelVoronoi[x, y, z] != i) //we are not in the correct voronoi cell
                        {
                            voxels[x, y, z] = 1; //outside
                            noCount++;
                        }
                        else
                        {
                            Vector3 queryPoint = new Vector3(x, y, z);
                            float distance = distanceOfMirrors(queryPoint, seeds[i]);
                            distance = voxelSmoothMin[x, y, z] / distance;
                            voxels[x, y, z] = 1 - (distance * 2); //transform from 0..1 to 1..-1
                            yesCount++;
                        }
                    }
                }
            }

            Mesh mesh = MarchingCubes.CreateMesh(voxels, scale);

            //The diffuse shader wants uvs so just fill with a empty array, there not actually used
            mesh.uv = new Vector2[mesh.vertices.Length];
            mesh.RecalculateNormals();

            m_mesh = new GameObject("Mesh");
            m_mesh.AddComponent<MeshFilter>();
            m_mesh.AddComponent<MeshRenderer>();
            m_mesh.GetComponent<Renderer>().material = m_material;
            m_mesh.GetComponent<MeshFilter>().mesh = mesh;
            
            //Center mesh
            //m_mesh.transform.localPosition = scale * new Vector3(-width / 2, -height / 2, -length / 2);

            m_mesh.AddComponent<MeshCollider>();

            ProceduralDuplication.AddToDuplicate(m_mesh);

        }
        Debug.Log("Time take = " + (Time.realtimeSinceStartup - start) * 1000.0f);
    }
}