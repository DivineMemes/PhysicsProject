using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShooterGun))]
public class ShooterGunEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ShooterGun script = (ShooterGun)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Kill"))
        {
            script.Kill();
        }
    }

}
