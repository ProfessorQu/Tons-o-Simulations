using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RMShape : MonoBehaviour
{
	public Color color;
	public enum Shape { Sphere, Cube, Plane };
    public enum Operation { Union, Subtraction, Intersection }

    public Shape type;
    public Operation operation;

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
