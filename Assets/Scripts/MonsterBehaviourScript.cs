using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

public class MonsterBehaviourScript : MonoBehaviour
{
    public GameObject markPrefab;
    public GameObject hitEffectPrefab;
    public AudioClip readySound;
    public AudioClip spawnSound;
    public Material deadMaterial;

    private enum State
    {
        Ready,
        Flying,
        Missed,
        Hitted,
        Landed,
    }

    private State state;
    private Collider monsterCollider;
    private Vector3 hitPosition;
    private Animation parentAnimation;
    private GameObject terrain;
    private Rigidbody monsterRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        state = State.Ready;
        monsterCollider = GetComponentInChildren<Collider>();
        hitPosition = new Vector3(0, 0, 0);
        // Ignore collision with terrain
        terrain = GameObject.Find("Terrain");
        Physics.IgnoreCollision(terrain.GetComponent<Collider>(), monsterCollider);
        parentAnimation = GetComponentInParent<Animation>();
        monsterRigidbody = GetComponent<Rigidbody>();
        IgnoreWeapons();
        // Play ready sound
        AudioSource.PlayClipAtPoint(readySound, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Flying && !parentAnimation.IsPlaying("Create Monster"))
        {
            Destroy(transform.root.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        switch (state)
        {
            case State.Flying:
                if (collision.gameObject.tag == "Weapon")
                {
                    IgnoreWeapons();
                    // Record current position
                    hitPosition = transform.position;
                    hitPosition.y = 0;
                    // Show hit effect
                    GameObject hitEffect = Instantiate(hitEffectPrefab, collision.contacts[0].point, Quaternion.identity);
                    Destroy(hitEffect, 1.0f);
                    // Stop animation
                    parentAnimation.Stop();
                    // Add gravity and explosion force
                    monsterRigidbody.useGravity = true;
                    monsterRigidbody.AddExplosionForce(2.5f, collision.contacts[0].point, 0.1f);
                    // Allow collision with terrain
                    Physics.IgnoreCollision(terrain.GetComponent<Collider>(), monsterCollider, false);
                    // Update to "Hitted" state
                    state = State.Hitted;
                    GetComponentInChildren<SkinnedMeshRenderer>().material = deadMaterial;
                }
                return;
            case State.Hitted:
                if (collision.gameObject.name == "Terrain")
                {
                    // Record distance away upon first landed
                    Vector3 landPosition = collision.contacts[0].point;
                    GameObject mark = Instantiate(markPrefab, landPosition, Quaternion.identity);
                    mark.tag = "Mark";
                    mark.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    landPosition.y = 0;
                    int points = (int) (Vector3.Distance(landPosition, hitPosition) * 100);
                    TMPro.TextMeshProUGUI scoreUI = GameObject.Find("Score").GetComponent<TMPro.TextMeshProUGUI>();
                    scoreUI.text = (Int32.Parse(scoreUI.text) + points).ToString();
                    // Update to "Landed" state
                    state = State.Landed;
                }
                return;
        }
    }

    public void Fly()
    {
        parentAnimation.Play("Create Monster");
        state = State.Flying;
        IgnoreWeapons(false);
        // Play spawn sound
        AudioSource.PlayClipAtPoint(spawnSound, transform.position);
    }


    public bool IsEnd()
    {
        return state == State.Missed || (state == State.Landed && monsterRigidbody.velocity == Vector3.zero);
    }

    public void IgnoreWeapons(bool ignore = true)
    {
        foreach (GameObject weapon in GameObject.FindGameObjectsWithTag("Weapon"))
        {
            Physics.IgnoreCollision(weapon.GetComponent<Collider>(), monsterCollider, ignore);
        }
    }
}
