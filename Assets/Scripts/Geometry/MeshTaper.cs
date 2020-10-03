#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshTaper : MeshModifier
{

    public Vector2 taperX = Vector2.one;
    public Vector2 taperY = Vector2.one;
    public Vector2 taperZ = Vector2.one;

    
    #region - Apply Modifier -
    internal override void ApplyModifier ()
    {
        base.ApplyModifier();

        Vector3 t = Vector3.one;

        for (int i = 0; i < vertices.Length; i++)
        {
            t = vertices[i] + (bounds.size * 0.5f);
            if (bounds.size.x > 1)
                t.x = 1f / t.x;
            if (bounds.size.y > 1)
                t.y = 1f / t.y;
            if (bounds.size.z > 1)
                t.z = 1f / t.z;

            vertices[i].z = Mathf.Lerp(vertices[i].z, vertices[i].z * taperX.x, t.x);
            vertices[i].y = Mathf.Lerp(vertices[i].y, vertices[i].y * taperX.y, t.x);

            vertices[i].x = Mathf.Lerp(vertices[i].x, vertices[i].x * taperY.x, t.y);
            vertices[i].z = Mathf.Lerp(vertices[i].z, vertices[i].z * taperY.y, t.y);

            vertices[i].x = Mathf.Lerp(vertices[i].x, vertices[i].x * taperZ.x, t.z);
            vertices[i].y = Mathf.Lerp(vertices[i].y, vertices[i].y * taperZ.y, t.z);
        }

        meshReference.vertices = vertices;
        meshReference.RecalculateBounds();

    }
    #endregion

}

#region - C: Mesh Taper Editor -
#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(MeshTaper))]
public class MeshTaperEditor : Editor
{

    #region - On Enable / Disable -
    void OnEnable ()
    {
        Undo.undoRedoPerformed += ApplyModifier;
    }

    void OnDisable ()
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
    private void ApplyModifier ()
    {
        (target as MeshTaper).ApplyModifier();
    }
    #endregion

}
#endif
#endregion
