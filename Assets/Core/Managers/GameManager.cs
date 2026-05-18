using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        DayEnd,
        GameOver
    }

    public static GameManager Instance { get; private set; }

    [SerializeField] private int maxDays = 7;
    [SerializeField] private int startingDay = 1;
    [SerializeField] private int startingReviewPoints = 0;
    [SerializeField] private int startingSuspicionPoints = 0;
    [SerializeField] private int maxSuspicionPoints = 100;

    public int CurrentDay { get; private set; }
    public int ReviewPoints { get; private set; }
    public int SuspicionPoints { get; private set; }
    public GameState State { get; private set; } = GameState.MainMenu;
    public bool IsPaused => State == GameState.Paused;

    public event Action<GameState> OnGameStateChanged;
    public event Action<int> OnDayChanged;
    public event Action<int> OnReviewPointsChanged;
    public event Action<int> OnSuspicionPointsChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (CurrentDay == 0)
        {
            ResetRunData();
        }
    }

    public void StartGame()
    {
        ResetRunData();
        SetState(GameState.Playing);
    }

    public void LoadGame()
    {
        if (!SaveSystem.HasSave())
        {
            ResetRunData();
            return;
        }

        var saveData = SaveSystem.Load();
        ApplySaveData(saveData);
    }

    public void SaveGame()
    {
        SaveSystem.Save(CreateSaveData());
    }

    public void PauseGame()
    {
        if (State != GameState.Playing)
        {
            return;
        }

        Time.timeScale = 0f;
        SetState(GameState.Paused);
    }

    public void ResumeGame()
    {
        if (State != GameState.Paused)
        {
            return;
        }

        Time.timeScale = 1f;
        SetState(GameState.Playing);
    }

    public void TogglePause()
    {
        if (IsPaused)
        {
            ResumeGame();
        }
        else if (State == GameState.Playing)
        {
            PauseGame();
        }
    }

    public void EndDay()
    {
        SetState(GameState.DayEnd);
    }

    public void NextDay()
    {
        CurrentDay++;
        OnDayChanged?.Invoke(CurrentDay);

        if (CurrentDay > maxDays)
        {
            GameOver();
            return;
        }

        SetState(GameState.Playing);
    }

    public void GameOver()
    {
        Time.timeScale = 1f;
        SetState(GameState.GameOver);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SetState(GameState.MainMenu);
    }

    public void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetDay(int day)
    {
        CurrentDay = Mathf.Clamp(day, 1, maxDays);
        OnDayChanged?.Invoke(CurrentDay);
    }

    public void SetReviewPoints(int reviewPoints)
    {
        ReviewPoints = reviewPoints;
        OnReviewPointsChanged?.Invoke(ReviewPoints);
    }

    public void AddReviewPoints(int amount)
    {
        SetReviewPoints(ReviewPoints + amount);
    }

    public void SetSuspicionPoints(int suspicionPoints)
    {
        SuspicionPoints = Mathf.Clamp(suspicionPoints, 0, maxSuspicionPoints);
        OnSuspicionPointsChanged?.Invoke(SuspicionPoints);
    }

    public void AddSuspicionPoints(int amount)
    {
        SetSuspicionPoints(SuspicionPoints + amount);
    }

    private void ResetRunData()
    {
        CurrentDay = startingDay;
        ReviewPoints = startingReviewPoints;
        SuspicionPoints = startingSuspicionPoints;

        OnDayChanged?.Invoke(CurrentDay);
        OnReviewPointsChanged?.Invoke(ReviewPoints);
        OnSuspicionPointsChanged?.Invoke(SuspicionPoints);
    }

    private SaveData CreateSaveData()
    {
        return new SaveData
        {
            CurrentDay = CurrentDay,
            ReviewPoints = ReviewPoints,
            SuspicionPoints = SuspicionPoints
        };
    }

    private void ApplySaveData(SaveData saveData)
    {
        if (saveData == null)
        {
            ResetRunData();
            return;
        }

        SetDay(saveData.CurrentDay);
        SetReviewPoints(saveData.ReviewPoints);
        SetSuspicionPoints(saveData.SuspicionPoints);
    }

    private void SetState(GameState newState)
    {
        if (State == newState)
        {
            return;
        }

        State = newState;
        OnGameStateChanged?.Invoke(State);
    }
}
