using UnityEngine;

public static class GameSettings
{
    public static AIController.DifficultyLevel SelectedDifficulty = AIController.DifficultyLevel.Medium;

    public enum MovementMode { Classic, DashJump }
    public static MovementMode SelectedMovementMode = MovementMode.Classic;

    // YENİ: Arcade modu açık mı kapalı mı?
    public static bool IsArcadeMode = false;
}