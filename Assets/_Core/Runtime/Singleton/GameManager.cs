using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public enum GameMode
    {
        GoDown,
        GoUp,
        Space
    }
    [Header("Game Mode")]
    public GameMode gameMode = GameMode.GoDown;
    
    [Header("Game Control")]
    public float gameSpeed = 0.3f;

    [Header("Game Objects")]
    public GameObject backgroundGameObject;
    public GameObject lightBreathingGameObject;
    public GameObject dustParticleGameObject;
}