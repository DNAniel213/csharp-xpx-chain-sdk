using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bgcontroller : MonoBehaviour
{
    Material material;
    Vector2 offset;

    public int xvelocity,yvelocity;

    private void Awake()
    {
        material = GetComponent<Renderer>().material;
    }
    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector2(xvelocity,yvelocity);
    }

    // Update is called once per frame
    void Update()
    {
        material.mainTextureOffset += offset * Time.deltaTime;
    }
}
