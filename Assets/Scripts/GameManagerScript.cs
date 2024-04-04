using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public class GamerManagerScript : MonoBehaviour
{
    public GameObject monsterManagerPrefab;
    public GameObject gamePanel;
    public Animation resultAnimation;
    public TMPro.TextMeshProUGUI countdownUI;
    public TMPro.TextMeshProUGUI warningUI;
    public TMPro.TextMeshProUGUI scoreUI;
    public TMPro.TextMeshPro finalScoreUI;
    public TMPro.TextMeshPro highScoreUI;
    public AudioClip endMusic;
    public AudioClip playingMusic;
    public GameObject smokeEffectPrefab;

    private enum State
    {
        Starting,
        Playing,
        End,
    }

    private State state;
    private float countdownTime;
    private GameObject monsterManager;
    private AudioSource audioSource;
    private int highScore = 0;

    public void Init()
    {
        scoreUI.text = "";
        warningUI.text = "Stay in Blue Region!";
        gamePanel.SetActive(true);
        countdownTime = 3;
        state = State.Starting;
        toggleWeaponsEmit();
        // Play playing music
        audioSource.clip = playingMusic;
        audioSource.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        gamePanel.SetActive(false);
        state = State.End;
        audioSource = GetComponent<AudioSource>();
        // Play end music
        audioSource.clip = endMusic;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * 0.4f);

        switch (state)
        {
            case State.Starting:
                // Countdown and spawn monsters
                countdownTime -= Time.deltaTime;
                countdownUI.text = ((int) Math.Ceiling(countdownTime)).ToString();
                if (countdownTime < 0)
                {
                    monsterManager = Instantiate(monsterManagerPrefab);
                    countdownUI.text = "";
                    warningUI.text = "";
                    scoreUI.text = "0";
                    state = State.Playing;
                }
                return;
            case State.Playing:
                if (monsterManager.GetComponentInChildren<MonsterManagerScript>().IsEnd())
                {
                    // Destrpy all monsters related objects and display result
                    Destroy(monsterManager);
                    foreach (GameObject go in GameObject.FindGameObjectsWithTag("Monster"))
                    {
                        removeGameObject(go);
                    }
                    foreach (GameObject go in GameObject.FindGameObjectsWithTag("Mark"))
                    {
                        removeGameObject(go);
                    }
                    resultAnimation.Play("Show Stone");
                    finalScoreUI.text = scoreUI.text;
                    int score = Int32.Parse(scoreUI.text);
                    if (score > highScore)
                    {
                        highScore = score;
                        highScoreUI.text = "*New Highest*";
                    }
                    else
                    {
                        highScoreUI.text = "Highest: " + highScore.ToString();
                    }
                    gamePanel.SetActive(false);
                    state = State.End;
                    toggleWeaponsEmit();
                    // Play end music
                    audioSource.clip = endMusic;
                    audioSource.Play();
                }
                return;
        }
    }

    private void toggleWeaponsEmit()
    {
        foreach (GameObject weapon in GameObject.FindGameObjectsWithTag("Weapon"))
        {
            weapon.GetComponent<BatScript>().toggleEmit();
        }
    }

    private void removeGameObject(GameObject go)
    {
        GameObject smokeEffect = Instantiate(smokeEffectPrefab, go.transform.position, Quaternion.identity);
        float size = 0.3f;
        smokeEffect.transform.localScale = new Vector3(size, size, size);
        Destroy(smokeEffect, 1.0f);
        Destroy(go);
    }
}
