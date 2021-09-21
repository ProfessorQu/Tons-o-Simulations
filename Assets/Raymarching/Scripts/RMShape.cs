using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RMShape : MonoBehaviour
{
	public enum Shape { Sphere, Cube, Torus, Plane };
    public enum Operation { Union, Subtraction, Intersection, SmoothUnion, SmoothSubtraction, SmoothIntersection }

    [Header("Characteristics")]
    public Shape type;
    public Operation operation;

    [Header("Repeating")]
    public Vector3 repeat;
    public Vector3 repeatInterval;

    [Header("Color")]
    public Color color = Color.white;

    public Vector3 position
    {
        get
        {
            return transform.position;
        }
    }

    public Vector3 scale
    {
        get
        {
            return transform.localScale;
        }
    }
}
