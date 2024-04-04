using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;

public class BatScript : MonoBehaviour
{
    public AudioClip hitSound;
    public AudioClip emitSound;
    public Material emitMaterial;
    public GameObject emitEffectPrefab;

    private Material originalMaterial;
    private MeshRenderer[] childenMeshRenderers;

    // Start is called before the first frame update
    void Start()
    {
        childenMeshRenderers = GetComponentsInChildren<MeshRenderer>();
        originalMaterial = childenMeshRenderers[0].material;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Stone" || collision.gameObject.tag == "Monster")
        {
            // Play hit sound
            AudioSource.PlayClipAtPoint(hitSound, collision.contacts[0].point);
        }
    }

    public void toggleEmit()
    {
        // Show emit effect
        GameObject emitEffect = Instantiate(emitEffectPrefab, transform.position, Quaternion.identity);
        emitEffect.transform.LookAt(Camera.main.transform.position, Camera.main.transform.up);
        Destroy(emitEffect, 1.0f);
        foreach (MeshRenderer meshRenderer in childenMeshRenderers)
        {
            meshRenderer.material = meshRenderer.material == originalMaterial ? emitMaterial : originalMaterial;
        }
        // Play emit sound
        AudioSource.PlayClipAtPoint(emitSound, transform.position);
    }
}
