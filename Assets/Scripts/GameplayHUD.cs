using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameplayHUD : MonoBehaviour
{
    [Header("HUD References")]
    [SerializeField] private TMP_Text p1ScoreText;
    [SerializeField] private TMP_Text p2ScoreText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text coinsText;

    private readonly Dictionary<ulong, int> scoresByClientId = new Dictionary<ulong, int>();

    private void OnEnable()
    {
        PlayerScore.OnScoreChanged += HandleScoreChanged;
    }

    private void OnDisable()
    {
        PlayerScore.OnScoreChanged -= HandleScoreChanged;
    }

    private void Start()
    {
        if (p1ScoreText != null) p1ScoreText.text = "P1: 0";
        if (p2ScoreText != null) p2ScoreText.text = "P2: 0";
        if (timerText != null) timerText.text = "Time: 00:00";
        if (resultText != null) resultText.text = "";
        if (coinsText != null) coinsText.text = "Coins: 0";
    }

    private void Update()
    {
        if (MatchManager.Instance == null) return;

        UpdateTimer();
        UpdateCoins();
        UpdateResult();
    }

    private void HandleScoreChanged(ulong clientId, int newScore)
    {
        scoresByClientId[clientId] = newScore;
        RefreshScoreTexts();
    }

    private void RefreshScoreTexts()
    {
        List<ulong> ids = new List<ulong>(scoresByClientId.Keys);
        ids.Sort();

        if (p1ScoreText != null)
        {
            p1ScoreText.text = ids.Count > 0 ? $"P1: {scoresByClientId[ids[0]]}" : "P1: 0";
        }

        if (p2ScoreText != null)
        {
            p2ScoreText.text = ids.Count > 1 ? $"P2: {scoresByClientId[ids[1]]}" : "P2: 0";
        }
    }

    private void UpdateTimer()
    {
        if (timerText == null) return;

        float time = MatchManager.Instance.TimeRemaining.Value;
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        timerText.text = $"Time: {minutes:00}:{seconds:00}";
    }

    private void UpdateCoins()
    {
        if (coinsText == null) return;

        coinsText.text = $"Coins: {MatchManager.Instance.CoinsRemaining.Value}";
    }

    private void UpdateResult()
    {
        if (resultText == null) return;

        if (!MatchManager.Instance.MatchEnded.Value)
        {
            resultText.text = "";
            return;
        }

        ulong winnerId = MatchManager.Instance.WinnerClientId.Value;

        if (winnerId == ulong.MaxValue)
        {
            resultText.text = "Draw!";
            return;
        }

        if (NetworkManager.Singleton != null &&
            winnerId == NetworkManager.Singleton.LocalClientId)
        {
            resultText.text = "You Win!";
        }
        else
        {
            resultText.text = "You Lose!";
        }
    }
}