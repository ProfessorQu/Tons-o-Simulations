using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RMShapeEditor : Editor
{
    static RMShape CreateShape(RMShape.Shape shapeType)
    {
        List<RMShape> allShapes = new List<RMShape>(FindObjectsOfType<RMShape>());
        int shapesOfType = 0;

        foreach (var s in allShapes)
        {
            if (s.type == shapeType)
            {
                shapesOfType++;
            }
        }

        GameObject obj;
        if (shapesOfType > 0)
        {
            obj = new GameObject(shapeType.ToString() + "(" + shapesOfType.ToString() + ")");
        }
        else
        {
            obj = new GameObject(shapeType.ToString());
        }

        RMShape shape = obj.AddComponent<RMShape>();
        shape.type = shapeType;

        return shape;
    }

    [MenuItem("GameObject/Raymarching/Sphere", priority = 1)]
    static void CreateSphere()
    {
        RMShape shape = CreateShape(RMShape.Shape.Sphere);
    }

    [MenuItem("GameObject/Raymarching/Cube", priority = 2)]
    static void CreateCube()
    {
        RMShape shape = CreateShape(RMShape.Shape.Cube);
    }

    [MenuItem("GameObject/Raymarching/Torus", priority = 3)]
    static void CreateTorus()
    {
        RMShape shape = CreateShape(RMShape.Shape.Torus);
    }

    [MenuItem("GameObject/Raymarching/Plane", priority = 4)]
    static void CreatePlane()
    {
        RMShape shape = CreateShape(RMShape.Shape.Plane);
    }
}
