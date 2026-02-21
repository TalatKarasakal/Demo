using UnityEngine;

public static class OyunAyarlari
{
    public static YapayZekaKontrol.DifficultyLevel SelectedDifficulty = YapayZekaKontrol.DifficultyLevel.Medium;

    public enum MovementMode { Classic, DashJump }
    public static MovementMode SelectedMovementMode = MovementMode.Classic;

    
    public static bool IsArcadeMode = false;
}