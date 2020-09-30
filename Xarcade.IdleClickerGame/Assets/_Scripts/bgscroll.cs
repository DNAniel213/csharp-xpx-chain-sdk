using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bgscroll : MonoBehaviour
{
    Material material;
    public Vector2 offset;
static float t = 0.0f;
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

        offset.y = Mathf.Lerp(offset.y, -0.05f, t);
        t += 0.01f * Time.deltaTime;
    }

    public void TriggerSpeedIncrease()
    {
        offset.y -= 0.5f;
        t = 0;
    }


}
