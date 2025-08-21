using UnityEngine;

public static class ScoreManager
{
    public static int Score
    {
        get;
        private set;
    }

    public static void IncreaseScore(int amount)
    {
        Score += amount;
    }

    public static void DecreaseScore(int amount)
    {
        Score -= amount;

        if (Score < 0)
            ResetScore();
    }

    public static void ResetScore()
    {
        Score = 0;
    }
}
