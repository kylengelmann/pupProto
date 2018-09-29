using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class gameobjectExtension {


    public static T GetComponentInHierarchy<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if(component != null) return component;
        component = gameObject.GetComponentInParent<T>();
        if (component != null) return component;
        return gameObject.GetComponentInChildren<T>();
    }
}
