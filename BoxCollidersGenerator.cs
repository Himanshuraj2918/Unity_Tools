using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BoxCollidersGenerator : EditorWindow
{
    private List<Vector3> points = new List<Vector3>();
    private List<string> colliderNames = new List<string>(); 
    private string objectName = "Box_Collider";

    [MenuItem("Tools/Box Collider Generator")]
    public static void ShowWindow()
    {
        GetWindow<BoxCollidersGenerator>("Box Collider Generator");
    }

private void OnGUI()
{
    GUILayout.Label("Parent Object", EditorStyles.boldLabel);
    objectName = EditorGUILayout.TextField("Parent Name", objectName); 
    GUILayout.Space(10);
    GUILayout.Label("Selected Points & Collider Names", EditorStyles.boldLabel);

    for (int i = 0; i < points.Count; i++)
    {
        EditorGUILayout.BeginHorizontal();
        
        points[i] = EditorGUILayout.Vector3Field($"Point {i + 1}", points[i]);
        colliderNames[i] = EditorGUILayout.TextField(colliderNames[i]); 

        EditorGUILayout.EndHorizontal();
    }

    if (GUILayout.Button("Select Points in Scene"))
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    if (points.Count > 1 && GUILayout.Button("Generate Collider"))
    {
        GenerateColliders();
    }

    if (GUILayout.Button("Clear Points"))
    {
        points.Clear();
        colliderNames.Clear();
    }
}


    private void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 clickedPoint = hit.point;
                clickedPoint.y = 1f; // Keep Y consistent

                points.Add(clickedPoint);
                colliderNames.Add($"Collider_{points.Count}"); 

                Debug.Log($"Point {points.Count} Selected: {clickedPoint}");

                e.Use();
                Repaint();
                SceneView.RepaintAll();
            }
        }
    }

    private void GenerateColliders()
    {
        if (points.Count < 2) return;

        GameObject parentObject = new GameObject(objectName);

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 start = points[i];
            Vector3 end = points[i + 1];
            Vector3 center = (start + end) / 2;

            float length = Vector3.Distance(start, end);
            Vector3 direction = (end - start).normalized;

            Quaternion rotation = Quaternion.LookRotation(direction);
            rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0); 
            GameObject box = new GameObject(colliderNames[i]); 
            box.transform.position = center;
            box.transform.rotation = rotation;
            box.transform.parent = parentObject.transform;

            BoxCollider boxCollider = box.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(0.5f, 0.5f, length);

            Debug.Log($"{box.name} created at {center}");
        }

        Debug.Log($"Curved Collider Created with {points.Count - 1} BoxColliders!");
        Repaint();
    }
}
