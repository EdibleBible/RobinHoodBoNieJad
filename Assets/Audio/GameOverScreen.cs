using System;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class GameOverScreen : MonoBehaviour
{
    public EventReference gameOverEvent;
    public LevelExit levelExit;
    public GameoverController gameoverController;

    private void Awake()
    {
        levelExit = FindObjectOfType<LevelExit>();
        gameoverController = FindObjectOfType<GameoverController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (levelExit == null)
                levelExit = FindObjectOfType<LevelExit>();

            if (levelExit != null)
                levelExit.Interact(gameoverController.transform);
        }
    }

    public void PlayGameOverSound()
    {
        RuntimeManager.PlayOneShot(gameOverEvent);
    }
}