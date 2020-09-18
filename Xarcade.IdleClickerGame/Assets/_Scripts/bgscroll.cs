using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bgscroll : MonoBehaviour
{
    Material material;
    Vector2 offset;

    public float xVelocity,yVelocity;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
    }
    
    void Start()
    {
        offset = new Vector2(xVelocity,yVelocity);
    }

    void Update()
    {
        material.mainTextureOffset += offset * Time.deltaTime;
    }
}
