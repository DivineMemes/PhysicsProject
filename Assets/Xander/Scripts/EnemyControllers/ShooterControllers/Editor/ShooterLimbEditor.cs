using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShooterLimb))]
public class ShooterLimbEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ShooterLimb script = (ShooterLimb)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Kill"))
        {
            script.Kill();
        }
    }

}
