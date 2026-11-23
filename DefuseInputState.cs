namespace CutTheWirePlugin;

internal sealed class DefuseInputState
{
    private readonly Queue<float> defuseTaps = new();
    private bool secondaryAttackHeld;

    public bool RecordDefuseTap(float currentTime)
    {
        while (defuseTaps.TryPeek(out var firstTap) && currentTime - firstTap > 1.0f)
            defuseTaps.Dequeue();

        defuseTaps.Enqueue(currentTime);
        if (defuseTaps.Count < 3) return false;

        defuseTaps.Clear();
        return true;
    }

    public bool UpdateSecondaryAttack(bool isHeld)
    {
        var pressed = isHeld && !secondaryAttackHeld;
        secondaryAttackHeld = isHeld;
        return pressed;
    }
}
