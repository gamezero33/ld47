#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshSquash : MeshModifier
{

    public Vector3 squash;


    internal override void ApplyModifier()
    {
        base.ApplyModifier();

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].x = vertices[i].x < squash.x - 0.5f ? squash.x - 0.5f : vertices[i].x;
            vertices[i].y = vertices[i].y < squash.y - 0.5f ? squash.y - 0.5f : vertices[i].y;
            vertices[i].z = vertices[i].z < squash.z - 0.5f ? squash.z - 0.5f : vertices[i].z;
        }

        meshReference.vertices = vertices;
        meshReference.RecalculateBounds();

    }

}

#region - C: Mesh Taper Editor -
#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(MeshSquash))]
public class MeshSquashEditor : Editor
{

    #region - On Enable / Disable -
    void OnEnable()
    {
        Undo.undoRedoPerformed += ApplyModifier;
    }

    void OnDisable()
    {
        Undo.undoRedoPerformed -= ApplyModifier;
    }
    #endregion

    #region ~ On Inspector GUI ~
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUI.changed)
        {
            ApplyModifier();
            EditorUtility.SetDirty(target);
        }
    }
    #endregion

    #region - Apply Modifier -
    private void ApplyModifier()
    {
        (target as MeshSquash).ApplyModifier();
    }
    #endregion

}
#endif
#endregion
