using UnityEngine;
using System.Collections;
using MarchingCubesProject;

public class ProceduralGeneration : MonoBehaviour
{
    [SerializeField]
    protected GameObject meshPrefab;

    private const int _width = 32;
    private const int _height = 32;
    private const int _length = 32;
    public const int k = 1;
    private const float _scale = 5f;
    public const int numSeeds = 12;

    public static float width { get { return _width * _scale; } }
    public static float height { get { return _height * _scale; } }
    public static float length { get { return _length * _scale; } }

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
        if (diff > _width / 2)
        {
            seed.x -= _width;
        }
        else if (diff < -_width / 2)
        {
            seed.x += _width;
        }

        diff = seed.y - testPoint.y;
        if (diff > _height / 2)
        {
            seed.y -= _height;
        }
        else if (diff < -_height / 2)
        {
            seed.y += _height;
        }

        diff = seed.z - testPoint.z;
        if (diff > _length / 2)
        {
            seed.z -= _length;
        }
        else if (diff < -_length / 2)
        {
            seed.z += _length;
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
        int[, ,] voxelVoronoi = new int[_width + 1, _height + 1, _length + 1];
        //smoothmin distances
        float[, ,] voxelSmoothMin = new float[_width + 1, _height + 1, _length + 1];
        //final values
        float[, ,] voxels = new float[_width + 1, _height + 1, _length + 1];
        int x, y, z;

        float start = Time.realtimeSinceStartup;

        Vector3[] seeds = new Vector3[numSeeds];

        for (int i = 0; i < seeds.Length; i++)
        {
            seeds[i] = new Vector3(Random.value * (_width + 1), Random.value * (_height + 1), Random.value * (_length + 1));
        }

        //populate the voronoi/smoothmin arrays
        for (x = 0; x < _width + 1; x++)
        {
            for (y = 0; y < _height + 1; y++)
            {
                for (z = 0; z < _length + 1; z++)
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
            for (x = 0; x < _width + 1; x++)
            {
                for (y = 0; y < _height + 1; y++)
                {
                    for (z = 0; z < _length + 1; z++)
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

            Mesh mesh = MarchingCubes.CreateMesh(voxels, _scale);

            //The diffuse shader wants uvs so just fill with a empty array, there not actually used
            mesh.uv = new Vector2[mesh.vertices.Length];
            mesh.RecalculateNormals();

            m_mesh = Instantiate(meshPrefab);
            m_mesh.GetComponent<MeshFilter>().mesh = mesh;
            
            //Center mesh
            //m_mesh.transform.localPosition = scale * new Vector3(-width / 2, -height / 2, -length / 2);

            //meshcolliders need to be added as components to properly initialize
            //so I can't put it in the prefab
            m_mesh.AddComponent<MeshCollider>(); 

            ProceduralDuplication.AddToDuplicate(m_mesh);

        }
        Debug.Log("Time take = " + (Time.realtimeSinceStartup - start) * 1000.0f);
    }
}