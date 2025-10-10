/// <summary>
/// Money handler for the player's savings for usage in the store
/// </summary>
public static class BankSystem
{
    public static int Savings { get; private set; }
    public static int Core_Savings { get; private set; }
    public static int Metal_Savings { get; private set; }

    public static void AddMoney(int value)
        => Savings += value;
    public static bool TryPay(int value)
        => Savings >= value;
    public static void Pay(int value)
        => Savings -= value;
    public static void AddCore(int value)
        => Core_Savings += value;
    public static bool TryPayCore(int value)
        => Core_Savings >= value;
    public static void PayCore(int value)
        => Core_Savings -= value;
    public static void AddMetal(int value)
        => Metal_Savings += value;
    public static bool TryPayMetal(int value)
        => Metal_Savings >= value;
    public static void PayMetal(int value)
        => Metal_Savings -= value;

}
