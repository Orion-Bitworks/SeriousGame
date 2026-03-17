using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Este script es para calcular el area irregular del albeolo, NO SE COMO VA
/// !!!!NO TOCAR!!!!
/// </summary>
public class SpawnAreaAlbeolo : MonoBehaviour
{

    public List<Transform> vertexPoints;
    private float heigh;

    List<Vector2> vertex = new List<Vector2>();
    
    void Start()
    {
        foreach (var t in vertexPoints)
            vertex.Add(new Vector2(t.position.x, t.position.z));

        heigh = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public Vector3 GetRandomPoint()
    {

            float minX = float.MaxValue, maxX = float.MinValue;
            float minZ = float.MaxValue, maxZ = float.MinValue;


            foreach (var v in vertex)
            {
                if (v.x < minX) minX = v.x;
                if (v.x > maxX) maxX = v.x;
                if (v.y < minZ) minZ = v.y;
                if (v.y > maxZ) maxZ = v.y;

            }

            //50 es las veces que intentamos encontrar un punto valido CAMBIAR
            for (int i = 0; i < 50; i++)
            {
                float x = Random.Range(minX, maxX);
                float z = Random.Range(minZ, maxZ);

                Vector2 point = new Vector2(x, z);

                if (IsPointInPoligon(point)) return new Vector3(x, heigh, z);
            }

            return transform.position;

    }

    private bool IsPointInPoligon(Vector2 point)
    {
        bool inside = false;

        for(int i = 0, j = vertex.Count - 1; i < vertex.Count; j = i++)
        {
            bool intersect = ((vertex[i].y > point.y) != (vertex[j].y > point.y))&&
                (point.x < (vertex[j].x - vertex[i].x) * (point.y- vertex[i].y) / 
                (vertex[j].y - vertex[i].y) + vertex[i].x);

            if(intersect) inside = !inside;
        }

        return inside;
    }
}
