using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FuzzyOperations : MonoBehaviour
{
    //Auxiliary function to set the membership curves inflexion points
    public void SetFuzzify(Vector2[] keyframes, AnimationCurve Curve)
    {
        int frame = 0;
        foreach (var key in keyframes)
        {
            Curve.AddKey(key.x, key.y);
        }

        foreach (var key in keyframes)
        {
            AnimationUtility.SetKeyLeftTangentMode(Curve, frame, AnimationUtility.TangentMode.Linear);
            AnimationUtility.SetKeyRightTangentMode(Curve, frame, AnimationUtility.TangentMode.Linear);
            frame += 1;
        }
    }

    //Membership values are defined in isoceles triangles. This function calcuates the are of half of this triangle (a rectangle triangle)
    public float CalculateHalfArea(float x0, float y0, float x1, float y1, float u)
    {
        // Function is of shape y = mx + b
        if (y0 == y1)
            return 0;

        float m = (y0 - y1) / (x0 - x1);
        float b = y1 - m * x1;
        float xu = (u - b) / m;
        float area = 0;
        if (m < 0)
            area = u * (x1 - x0 + xu - x0) / 2;
        else
            area = u * (x1 - x0 + x1 - xu) / 2;

        return area;

    }

    //Divide the isoceles membership into two rectangle triangles and sum up their areas
    public float CalculateTrapezoidArea(AnimationCurve function, float U)
    {
        //Split in two segments, first half and second half
        //Split in two segments, first half and second half

        float areaA = CalculateHalfArea(function.keys[0].time, function.keys[0].value, function.keys[1].time, function.keys[1].value, U);
        float areaB = CalculateHalfArea(function.keys[1].time, function.keys[1].value, function.keys[2].time, function.keys[2].value, U);

        return areaA + areaB;
    }

    //Return the maximum point of the Curve
    public float CalculateCenter(AnimationCurve function, float U)
    {
        return (function.keys[1].time);
    }
}
