using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Simulations.Raymarching
{
    public class ShapeEditor : Editor
    {
        static Shape CreateShape(Shape.ShapeType shapeType)
        {
            List<Shape> allShapes = new List<Shape>(FindObjectsOfType<Shape>());
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

            Shape shape = obj.AddComponent<Shape>();
            shape.type = shapeType;

            return shape;
        }

        [MenuItem("GameObject/Raymarching/Sphere", priority = 1)]
        static void CreateSphere()
        {
            Shape shape = CreateShape(Shape.ShapeType.Sphere);
        }

        [MenuItem("GameObject/Raymarching/Cube", priority = 2)]
        static void CreateCube()
        {
            Shape shape = CreateShape(Shape.ShapeType.Cube);
        }

        [MenuItem("GameObject/Raymarching/Torus", priority = 3)]
        static void CreateTorus()
        {
            Shape shape = CreateShape(Shape.ShapeType.Torus);
        }

        [MenuItem("GameObject/Raymarching/Plane", priority = 4)]
        static void CreatePlane()
        {
            Shape shape = CreateShape(Shape.ShapeType.Plane);
        }
    }
}