using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoB : MonoBehaviour
{
    private void Awake()
    {
        this.Bind();
    }

    public void Hello()
    {
        Debug.Log("HELLO");
    }
}
