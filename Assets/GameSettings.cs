using UnityEngine;

public static class GameSettings
{
    // AIController içindeki DifficultyLevel enum’ını kullanıyoruz:
    public static AIController.DifficultyLevel SelectedDifficulty 
        = AIController.DifficultyLevel.Medium;
}