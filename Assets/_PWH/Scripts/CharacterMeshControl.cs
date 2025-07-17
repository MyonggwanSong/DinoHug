using System.Collections.Generic;
using UnityEngine;

public class CharacterMeshControl : MonoBehaviour
{
    [SerializeField] List<GameObject> bodyMesh;

    public void SetMaterial(Material mat)
    {
        Debug.Log("Material Setting");

        if (bodyMesh.Count <= 0)
        {
            Debug.Log("mesh Count is 0");
            return;
        }

        foreach (var mesh in bodyMesh)
        {
            mesh.GetComponent<SkinnedMeshRenderer>().material = mat;
        }
    }
}