using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public enum GameState { START, PLAYING, WON, LOST }

    [Header("Game Settings")]
    public float totalTime   = 60f;
    public float wave2Time   = 40f;
    public float wave3Time   = 20f;

    public GameState State       { get; private set; } = GameState.START;
    public float     TimeLeft    { get; private set; }
    public int       Wave        { get; private set; } = 1;
    public float     Load        { get; private set; } = 0f;
    public float     Frustration { get; private set; } = 0f;
    public string    LostReason  { get; private set; } = "";

    public event System.Action<GameState> OnStateChanged;
    public event System.Action            OnTick;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        SetState(GameState.START);
    }

    public void StartGame()
    {
        TimeLeft    = totalTime;
        Wave        = 1;
        Load        = 0f;
        Frustration = 0f;
        LostReason  = "";

        StopAllCoroutines();
        SetState(GameState.PLAYING);
        StartCoroutine(CountdownLoop());
    }

    IEnumerator CountdownLoop()
    {
        while (State == GameState.PLAYING)
        {
            yield return new WaitForSeconds(1f);

            TimeLeft--;

            if (TimeLeft <= wave2Time && Wave < 2) Wave = 2;
            if (TimeLeft <= wave3Time && Wave < 3) Wave = 3;

            OnTick?.Invoke();

            if (TimeLeft <= 0f)
            {
                SetState(GameState.WON);
                yield break;
            }
        }
    }

    public void BotReachedServer()
    {
        if (State != GameState.PLAYING) return;
        Load = Mathf.Min(100f, Load + 12f);
        OnTick?.Invoke();
        CheckLoss();
    }

    public void LegitReachedServer()
    {
        if (State != GameState.PLAYING) return;
        Load = Mathf.Max(0f, Load - 2f);
        OnTick?.Invoke();
    }

    public void LegitTimedOut()
    {
        if (State != GameState.PLAYING) return;
        // Reduced frustration penalty to 5% to make the game more forgiving early on
        Frustration = Mathf.Min(100f, Frustration + 5f);
        OnTick?.Invoke();
        CheckLoss();
    }

    void CheckLoss()
    {
        if (State != GameState.PLAYING) return;

        if (Load >= 100f)
        {
            LostReason = "SERVER_CRASH";
            SetState(GameState.LOST);
        }
        else if (Frustration >= 100f)
        {
            LostReason = "USER_REVOLT";
            SetState(GameState.LOST);
        }
    }

    void SetState(GameState newState)
    {
        State = newState;
        OnStateChanged?.Invoke(newState);
    }
}