/// <summary>
/// Money handler for the player's savings for usage in the store
/// </summary>
public static class BankSystem
{
    public static int Savings { get; private set; }

    public static void AddMoney(int value)
        => Savings += value;

    public static bool TryPay(int value)
        => Savings >= value;

    public static void Pay(int value)
        => Savings -= value;
}
