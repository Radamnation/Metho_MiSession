using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoA : MonoBehaviour
{
    private DemoB demoB;
    
    private void Awake()
    {
        demoB = Dependency.Get<DemoB>();
    }

    private void Start()
    {
        demoB.Hello();
    }
}
