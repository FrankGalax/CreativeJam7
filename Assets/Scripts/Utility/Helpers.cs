using UnityEngine;
using System.Collections;

public static class Helpers
{
    public static bool IsDebug = true;

    /////////////////////////////////////////////////////////////////////////////////////////
    /// TAG HELPER FUNCTIONS
    /////////////////////////////////////////////////////////////////////////////////////////
    public static bool CheckObjectTag(GameObject obj, string tag)
    {
        Transform[] children = obj.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            Transform parent = child.transform.parent;
            if (parent != null && parent.gameObject == obj && child.gameObject.tag == tag)
            {
                return true;
            }
        }

        return false;
    }

    public static string[] GetObjectTags(GameObject obj)
    {
        Transform[] children = obj.GetComponentsInChildren<Transform>();
        string[] tags = new string[children.Length];

        for (int i = 0; i < children.Length; i++)
        {
            tags[i] = children[i].gameObject.tag;
        }

        return tags;
    }

    public static GameObject[] GetObjectsWithTag(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

        for (int i = 0; i < objects.Length; i++)
        {
            objects[i] = objects[i].transform.parent.gameObject;
        }

        return objects;
    }

    public static GameObject GetObjectWithTag(string tag)
    {
        return GameObject.FindGameObjectWithTag(tag).transform.parent.gameObject;
    }

    public static GameObject GetChildObjectWithTag(GameObject parent, string tag)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>();

        foreach (Transform child in children)
        {
            if (child.tag == tag)
            {
                return child.parent.gameObject;
            }
        }

        return null;
    }

    public static void SetCollidersEnabled(GameObject obj, bool state)
    {
        Collider[] colliders = obj.GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = state;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////
    /// MATH HELPER FUNCTIONS
    /////////////////////////////////////////////////////////////////////////////////////////
    public static float ClampAngleDegrees(float angle, float min, float max)
    {
        if (angle < -360.0f)
        {
            angle += 360.0f;
        }
        else if (angle > 360.0f)
        {
            angle -= 360.0f;
        }

        return Mathf.Clamp(angle, min, max);
    }

    public static float ClampAngleRadians(float angle)
    {
        float twoPi = 2 * Mathf.PI;
        if (angle < 0)
        {
            angle += twoPi;
        }
        else if (angle > twoPi)
        {
            angle -= twoPi;
        }

        return angle;
    }
}