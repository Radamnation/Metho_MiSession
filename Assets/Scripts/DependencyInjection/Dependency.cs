using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Dependency
{

    private static Dictionary<string, MonoBehaviour> m_dependencies = new Dictionary<string, MonoBehaviour>();

    public static void Bind(this MonoBehaviour _script)
    {
        var key = _script.GetType().Name;
        if (!m_dependencies.ContainsKey(key))
        {
            m_dependencies.Add(key, _script);
        }
    }

    public static T Get<T>() where T : MonoBehaviour
    {
        var key = typeof(T).Name;
        return m_dependencies.ContainsKey(key) ? (T) m_dependencies[key] : null;
    }

    public static void Clear()
    {
        m_dependencies.Clear();
    }
}
