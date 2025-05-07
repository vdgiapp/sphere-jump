using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public enum GameMode
    {
        Descent,
        Ascent,
        ZeroGravity
    }
    
    [Header("Game Mode")]
    public GameMode currentGameMode = GameMode.Descent;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }
}