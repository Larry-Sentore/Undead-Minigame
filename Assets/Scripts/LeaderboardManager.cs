using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

/// <summary>
/// Stores scores and sorts them from highest to lowest using a recursive merge sort. Algorithm: Merge Sort in O(n log n) time.
/// </summary>
public sealed class LeaderboardManager : MonoBehaviour
{
    private const string ScoresPreferenceKey = "UndeadMinigame.Leaderboard";

    // Loads previous scores, adds a new run, sorts all scores, and persists the result. Algorithm: Merge Sort.
    public void SubmitScore(int score)
    {
        List<int> scores = LoadScores();
        scores.Add(Mathf.Max(0, score));
        SortDescending(scores);

        string[] serializedScores = new string[scores.Count];
        for (int i = 0; i < scores.Count; i++)
        {
            serializedScores[i] = scores[i].ToString(CultureInfo.InvariantCulture);
        }

        PlayerPrefs.SetString(ScoresPreferenceKey, string.Join(",", serializedScores));
        PlayerPrefs.Save();
    }

    // Returns up to the requested number of highest scores. Algorithm: reads the already sorted leaderboard.
    public List<int> GetTopScores(int count)
    {
        List<int> scores = LoadScores();
        int resultCount = Mathf.Clamp(count, 0, scores.Count);
        return scores.GetRange(0, resultCount);
    }

    // Loads and validates the comma-separated scores saved in PlayerPrefs. OOP principle: persistence is encapsulated.
    private List<int> LoadScores()
    {
        List<int> scores = new List<int>();
        string serializedScores = PlayerPrefs.GetString(ScoresPreferenceKey, string.Empty);
        if (string.IsNullOrEmpty(serializedScores))
        {
            return scores;
        }

        string[] entries = serializedScores.Split(',');
        for (int i = 0; i < entries.Length; i++)
        {
            int score;
            if (int.TryParse(entries[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out score) && score >= 0)
            {
                scores.Add(score);
            }
            else
            {
                Debug.LogWarning("A malformed leaderboard score was ignored: " + entries[i]);
            }
        }

        SortDescending(scores);
        return scores;
    }

    // Sorts scores in descending order without using List.Sort. Algorithm: top-level Merge Sort operation.
    private static void SortDescending(List<int> scores)
    {
        if (scores.Count < 2)
        {
            return;
        }

        int[] values = scores.ToArray();
        int[] scratch = new int[values.Length];
        MergeSort(values, scratch, 0, values.Length - 1);
        scores.Clear();
        scores.AddRange(values);
    }

    // Recursively sorts each half of the score array. Algorithm: divide-and-conquer Merge Sort.
    private static void MergeSort(int[] values, int[] scratch, int left, int right)
    {
        if (left >= right)
        {
            return;
        }

        int middle = left + (right - left) / 2;
        MergeSort(values, scratch, left, middle);
        MergeSort(values, scratch, middle + 1, right);
        Merge(values, scratch, left, middle, right);
    }

    // Merges two sorted halves in descending order. Algorithm: linear merge step of Merge Sort.
    private static void Merge(int[] values, int[] scratch, int left, int middle, int right)
    {
        int leftIndex = left;
        int rightIndex = middle + 1;
        int outputIndex = left;

        while (leftIndex <= middle && rightIndex <= right)
        {
            if (values[leftIndex] >= values[rightIndex])
            {
                scratch[outputIndex++] = values[leftIndex++];
            }
            else
            {
                scratch[outputIndex++] = values[rightIndex++];
            }
        }

        while (leftIndex <= middle)
        {
            scratch[outputIndex++] = values[leftIndex++];
        }

        while (rightIndex <= right)
        {
            scratch[outputIndex++] = values[rightIndex++];
        }

        for (int i = left; i <= right; i++)
        {
            values[i] = scratch[i];
        }
    }
}
