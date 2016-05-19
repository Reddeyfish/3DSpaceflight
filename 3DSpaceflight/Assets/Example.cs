using UnityEngine;
using System.Collections;

using PerlinNoiseProject;

namespace MarchingCubesProject
{
    public class Example : MonoBehaviour
    {
        int width = 32;
        int height = 32;
        int length = 32;

        public Material m_material;

        PerlinNoise m_perlin;
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
            else if(diff < width / 2)
            {
                seed.x -= width;
            }

            diff = seed.y - testPoint.y;
            if (diff > height / 2)
            {
                seed.y -= height;
            }
            else if (diff < height / 2)
            {
                seed.y -= height;
            }

            diff = seed.z - testPoint.z;
            if (diff > length / 2)
            {
                seed.z -= length;
            }
            else if (diff < length / 2)
            {
                seed.z -= length;
            }

            return seed;
        }

        float distanceOfMirrors(Vector3 testPoint, Vector3 seed)
        {
            return Vector3.Distance(testPoint, minOfMirrors(testPoint, seed));
        }

        int value(Vector3 testPoint, Vector3[] seeds)
        {
            float minDist = distanceOfMirrors(testPoint, seeds[0]);

            for (int i = 1; i < seeds.Length; i++)
            {
                if (distanceOfMirrors(testPoint, seeds[i]) < minDist)
                {
                    return 1; //outside
                }
            }

            return 0;
        }

        // Use this for initialization
        void Start()
        {

            m_perlin = new PerlinNoise(2);

            //Target is the value that represents the surface of mesh
            //For example the perlin noise has a range of -1 to 1 so the mid point is were we want the surface to cut through
            //The target value does not have to be the mid point it can be any value with in the range
            MarchingCubes.SetTarget(0.0f);

            //Winding order of triangles use 2,1,0 or 0,1,2
            MarchingCubes.SetWindingOrder(2, 1, 0);

            //Set the mode used to create the mesh
            //Cubes is faster and creates less verts, tetrahedrons is slower and creates more verts but better represents the mesh surface
            MarchingCubes.SetModeToCubes();
            //MarchingCubes.SetModeToTetrahedrons();

            float[,,] voxels = new float[width, height, length];

            int x, y, z;

            float start = Time.realtimeSinceStartup;

            Vector3[] seeds = new Vector3[10]; //0 is displayed seed for now

            for (int i = 0; i < seeds.Length; i++)
            {
                seeds[i] = new Vector3(Random.value * width, Random.value * height, Random.value * length);
            }

            //Fill voxels with values. Im using perlin noise but any method to create voxels will work
            for (x = 0; x < width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    for (z = 0; z < length; z++)
                    {
                        Vector3 testPoint = new Vector3(x, y, z);

                        //if we haven't exited, we are inside
                        voxels[x, y, z] = value(testPoint, seeds);

                        //voxels[x, y, z] = m_perlin.FractalNoise3D(x, y, z, 3, 40.0f, 1.0f);
                    }
                }
            }

            Mesh mesh = MarchingCubes.CreateMesh(voxels);

            //The diffuse shader wants uvs so just fill with a empty array, there not actually used
            mesh.uv = new Vector2[mesh.vertices.Length];
            mesh.RecalculateNormals();

            m_mesh = new GameObject("Mesh");
            m_mesh.AddComponent<MeshFilter>();
            m_mesh.AddComponent<MeshRenderer>();
            m_mesh.GetComponent<Renderer>().material = m_material;
            m_mesh.GetComponent<MeshFilter>().mesh = mesh;
            //Center mesh
            m_mesh.transform.localPosition = new Vector3(-width / 2, -height / 2, -length / 2);

            Debug.Log("Time take = " + (Time.realtimeSinceStartup - start) * 1000.0f);

        }
    }
}
