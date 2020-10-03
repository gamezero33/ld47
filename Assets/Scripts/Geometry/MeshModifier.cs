using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class MeshModifier : MonoBehaviour
{
    
    //[HideInInspector]
    [SerializeField]
    private Mesh originalMesh;

    private MeshFilter meshFilter;
    private Mesh meshInstance;

    internal Mesh meshReference;
    internal Vector3[] vertices;

    internal Bounds bounds;


    #region ~ On Enable ~
    internal virtual void OnEnable ()
    {
        Initialize();
        ApplyModifier();
    }
    #endregion

    #region ~ On Destroy ~
    private void OnDestroy ()
    {
        ResetMesh();
    }
    #endregion


    #region - Initialize -
    private void Initialize ()
    {
        meshFilter = GetComponent<MeshFilter>();
        
        if (!meshFilter)
        {
            Debug.LogError("Could not get a reference to a MeshFilter attached to " + this);
            return;
        }

        if (!originalMesh)
        {
            originalMesh = Instantiate(meshFilter.sharedMesh) as Mesh;
            bounds = originalMesh.bounds;
        }

        if (!originalMesh)
        {
            Debug.LogError("Could not instantiate original mesh from MeshFilter attached to " + this);
            return;
        }

        if (!meshInstance)
        {
            meshInstance = Instantiate(originalMesh) as Mesh;
            meshInstance.name = meshInstance.name.Replace("(Clone)", "");
        }

        if (meshInstance)
        {
            meshReference = meshFilter.mesh = meshInstance;
            vertices = meshReference.vertices;
        }
    }
    #endregion


    #region - Apply Modifier -
    internal virtual void ApplyModifier ()
    {
        ResetMesh();
    }
    #endregion

    #region - Reset Mesh -
    internal void ResetMesh ()
    {
        meshInstance = null;
        Initialize();
    }
    #endregion

}
