using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType { Hallway, Combat, Treasure, Prison, Puzzle }

[System.Serializable]
public class WeightedScene
{
    public string sceneName;
    public int sceneDifficulty; // This is now matched directly to player difficulty
}

[System.Serializable]
public class SceneListForType
{
    public SceneType type;  // Combat, Puzzle, etc.
    public List<WeightedScene> scenes;  // All scenes of this type
}

public class LevelPicker : MonoBehaviour
{
    // The only inspector-editable field for LevelPicker
    public SceneType currentSceneType;
    

    // The ScriptableObject reference will be loaded automatically at runtime
    [SerializeField] private SceneDatabaseSO sceneDatabaseSO;

    void Awake()
    {
        // Try to auto-load the SceneDatabase from Resources/SceneDatabase.asset
        if (sceneDatabaseSO == null)
        {
            Debug.LogError("SceneDatabase SO not found. Create it via Create -> Level Picker -> Scene Database and place it in Assets/Resources named 'SceneDatabase'.");
        }
    }

    /// <summary>
    /// Returns the name of the next scene to load
    /// </summary>
    public string GetNextScene(bool playerDied, int playerDifficulty)
    {
        if (playerDied)
        {
            // Always send the player to a hallway when they die
            return GetRandomScene(SceneType.Hallway, playerDifficulty);
        }

        // Normal weighted scene picking
        Dictionary<SceneType, float> weights = GetWeights(currentSceneType, playerDifficulty);
        SceneType nextType = ChooseNextSceneType(weights);
        return GetRandomScene(nextType, playerDifficulty);
    }


    private Dictionary<SceneType, float> GetWeights(SceneType currentType, int difficulty)
    {
        var weights = new Dictionary<SceneType, float>();

        switch (currentType)
        {
            case SceneType.Hallway:
                weights.Add(SceneType.Hallway, 10f + difficulty);
                weights.Add(SceneType.Combat, 15f + (2f * difficulty));
                weights.Add(SceneType.Prison, difficulty);
                weights.Add(SceneType.Puzzle, difficulty);
                weights.Add(SceneType.Treasure, 0.25f * difficulty);
                break;
            case SceneType.Combat:
                weights.Add(SceneType.Hallway, 0.3f * difficulty);
                weights.Add(SceneType.Combat, 0.1f * difficulty);
                weights.Add(SceneType.Treasure, 0.3f * difficulty);
                weights.Add(SceneType.Prison, 0.2f * difficulty);
                weights.Add(SceneType.Puzzle, 0.1f * difficulty);
                break;
            case SceneType.Treasure:
                weights.Add(SceneType.Hallway, 0.2f * difficulty);
                weights.Add(SceneType.Combat, 0.1f * difficulty);
                weights.Add(SceneType.Treasure, 0.05f * difficulty);
                weights.Add(SceneType.Prison, 0.3f * difficulty);
                weights.Add(SceneType.Puzzle, 0.35f * difficulty);
                break;
            case SceneType.Prison:
            case SceneType.Puzzle:
                weights.Add(SceneType.Hallway, 0.6f * difficulty);
                weights.Add(SceneType.Combat, 0.2f * difficulty);
                weights.Add(SceneType.Treasure, 0.1f * difficulty);
                weights.Add(SceneType.Prison, 0.05f * difficulty);
                weights.Add(SceneType.Puzzle, 0.05f * difficulty);
                break;
            default:
                Debug.LogWarning($"Unknown scene type: {currentType}. Using equal weights.");
                foreach (SceneType type in System.Enum.GetValues(typeof(SceneType)))
                    weights.Add(type, 1f);
                break;
        }

        return weights;
    }

    private SceneType ChooseNextSceneType(Dictionary<SceneType, float> weights)
    {
        float totalWeight = 0;
        foreach (var pair in weights)
            totalWeight += pair.Value;

        if (Mathf.Approximately(totalWeight, 0))
        {
            Debug.LogWarning("All scene weights are zero! Defaulting to Hallway.");
            return SceneType.Hallway;
        }

        float randomPoint = Random.Range(0, totalWeight);
        float cumulative = 0;

        foreach (var pair in weights)
        {
            cumulative += pair.Value;
            if (randomPoint < cumulative)
                return pair.Key;
        }

        Debug.LogError("Weight selection failed! Defaulting to Hallway.");
        return SceneType.Hallway;
    }

    private string GetRandomScene(SceneType type, int difficulty)
    {
        if (sceneDatabaseSO == null)
        {
            Debug.LogError("Scene database SO is null. Returning fallback scene 'MainMenu'.");
            return "MainMenu";
        }

        SceneListForType typeEntry = sceneDatabaseSO.sceneLists.Find(e => e.type == type);
        if (typeEntry == null || typeEntry.scenes == null || typeEntry.scenes.Count == 0)
        {
            Debug.LogError($"No scenes configured for type: {type}");
            return "MainMenu";
        }

        string currentSceneName = SceneManager.GetActiveScene().name;

        // Filter out the current scene
        List<WeightedScene> availableScenes = typeEntry.scenes.FindAll(s => s.sceneName != currentSceneName);

        if (availableScenes.Count == 0)
        {
            Debug.LogWarning($"No alternative scenes found for type {type}. Returning current scene.");
            return currentSceneName; // fallback if all options are the same scene
        }

        // Calculate weights based on sceneDifficulty matching the difficulty
        List<float> sceneWeights = new List<float>();
        foreach (WeightedScene scene in availableScenes)
        {
            int diff = Mathf.Abs(scene.sceneDifficulty - difficulty);
            float weight = 25f / (1 + diff); // Higher weight for closer matches
            sceneWeights.Add(weight);
        }

        float totalWeight = 0;
        foreach (float w in sceneWeights) totalWeight += w;

        if (Mathf.Approximately(totalWeight, 0))
        {
            Debug.LogWarning("All scene weights are zero! Selecting randomly.");
            return availableScenes[Random.Range(0, availableScenes.Count)].sceneName;
        }

        float randomPoint = Random.Range(0, totalWeight);
        float cumulative = 0;

        for (int i = 0; i < availableScenes.Count; i++)
        {
            cumulative += sceneWeights[i];
            if (randomPoint < cumulative)
                return availableScenes[i].sceneName;
        }

        return availableScenes[0].sceneName;
    }

}
