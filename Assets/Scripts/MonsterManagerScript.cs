using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public class MonsterManagerScript : MonoBehaviour
{
    public GameObject monsterPrefab;

    private float timeElasped = 0;
    private float timeSpawn = 5;
    private List<GameObject> monsters;
    private GameObject lastMonster;

    // Start is called before the first frame update
    void Start()
    {
        monsters = new List<GameObject>(5);
        List<Vector3> positions = new List<Vector3>(5);
        positions.Add(new Vector3(1.3f, 0, 0.75f));
        positions.Add(new Vector3(0.75f, 0, 1.3f));
        positions.Add(new Vector3(0, 0, 1.5f));
        positions.Add(new Vector3(-0.75f, 0, 1.3f));
        positions.Add(new Vector3(-1.3f, 0, 0.75f));
        foreach(var position in positions)
        {
            GameObject monster = Instantiate(monsterPrefab, position, Quaternion.identity);
            monsters.Add(monster);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(monsters.Count > 0)
        {
            timeElasped += Time.deltaTime;
            if (timeElasped > timeSpawn)
            {
                int randomIndex = UnityEngine.Random.Range(0, monsters.Count);
                GameObject monster = monsters[randomIndex];
                monster.GetComponentInChildren<MonsterBehaviourScript>().Fly();
                if (monsters.Count == 1)
                {
                    lastMonster = monster;
                }
                monsters.RemoveAt(randomIndex);
                timeElasped = 0;
                timeSpawn--;
            }
        }
    }

    public bool IsEnd()
    {
        foreach (GameObject monster in GameObject.FindGameObjectsWithTag("Monster"))
        {
            if (!monster.GetComponentInChildren<MonsterBehaviourScript>().IsEnd())
            {
                return false;
            }
        }
        return true;
    }
}
