using System;

public static class PlayerInteractionState
{
    public static bool IsLocked { get; private set; }

    public static event Action<bool> LockChanged;

    public static void SetLocked(bool locked)
    {
        if (IsLocked == locked)
        {
            return;
        }

        IsLocked = locked;
        LockChanged?.Invoke(IsLocked);
    }
}