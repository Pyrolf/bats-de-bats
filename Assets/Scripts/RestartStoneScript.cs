using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartStoneScript : MonoBehaviour
{
    public GamerManagerScript gameManagerScript;

    private Animation parentAnimation;

    // Start is called before the first frame update
    void Start()
    {
        parentAnimation = GetComponentInParent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (!parentAnimation.IsPlaying("Hide Stone") && !parentAnimation.IsPlaying("Show Stone") && collision.gameObject.tag == "Weapon")
        {
            parentAnimation.Play("Hide Stone");
            gameManagerScript.Init();
        }
    }
}
