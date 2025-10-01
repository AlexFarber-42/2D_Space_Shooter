using TMPro;
using UnityEngine;

public class LevelCompletionStats : MonoBehaviour
{
    [SerializeField] private GameObject levelNotification;
    [SerializeField] private TextMeshProUGUI killedText;
    [SerializeField] private TextMeshProUGUI shotsText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private TextMeshProUGUI numberLeftText;

    private readonly int ShopScene = 2;

    public void SignalLevelNotifActive(bool state)
        => levelNotification.SetActive(state);

    public void InjectStats()
    {
        Player player = Player.Instance;

        int shotsFired      = player.ShotsFired;
        int accuracy        = player.Accuracy;
        int numberLeft      = WaveManager.Instance.EnemiesLeft;
        int killedAmount    = WaveManager.Instance.EnemiesKilled;

        killedText.text         = killedAmount.ToString("Number Killed\n000000");
        shotsText.text          = shotsFired.ToString("Shots Fired\n000000");
        accuracyText.text       = accuracy.ToString("Accuracy\n000");
        numberLeftText.text     = numberLeft.ToString("Number Left\n000000");
    }

    public void SignalLevelChange()
        => GameManager.Instance.ChangeScene(ShopScene);
}
