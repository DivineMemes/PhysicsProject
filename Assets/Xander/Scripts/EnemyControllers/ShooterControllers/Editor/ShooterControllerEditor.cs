using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShooterController))]
public class ShooterControllerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ShooterController script = (ShooterController)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Kill"))
        {
            script.Kill();
        }
    }

}
