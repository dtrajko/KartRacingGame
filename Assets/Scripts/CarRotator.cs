﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRotator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(this.transform.up, 1f);
    }
}
