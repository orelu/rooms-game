using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;


[System.Serializable]
public class ItemData
{
    public string name;
    public bool isBreakable;
    public Vector3 scale;
    public Vector3 position;
    public bool isDefaultItem;
    public float attackRate;
    public int strengthModifier;
    public int defenceModifier;
    public int speedModifier;
    public int glowModifier;
    public int itemID;
    public Sprite icon;
}

[System.Serializable]
public class ItemDataListWrapper
{
    public List<ItemData> items;
}

public class ExitDoorLogic : MonoBehaviour
{
    [HideInInspector] public GameObject sprite; // now auto-assigned to self
    [HideInInspector] public Sprite tileUnselected;
    [HideInInspector] public Sprite tileSelected;
    [HideInInspector] public GameObject player; // found automatically
    public int radius;
    [HideInInspector] public SpriteRenderer renderer;
    [HideInInspector] public float timer = 0f;
    public float timeToTransition = 5.0f;
    public string sceneToLoad = "";
    
    private Canvas fadeCanvas;
    private Image fadeImage;
    private float fadeAlpha = 0f;
    private GameObject canvasGO;
    private Text deathText;


    [HideInInspector] public GameObject playerStats; // found automatically
    
    void Awake()
    {
        // Auto-assign variables
        sprite = gameObject;
        renderer = GetComponent<SpriteRenderer>();

        // Find player in the hierarchy (tagged "Player" or named "Player")
        GameObject foundPlayer = GameObject.FindWithTag("Player");
        if (foundPlayer == null)
            foundPlayer = GameObject.Find("Player");
        player = foundPlayer;

        // Find StatsManager and its PlayerStats child
        GameObject statsManager = GameObject.Find("StatsManager");
        if (statsManager != null)
            playerStats = statsManager.GetComponentInChildren<PlayerStats>()?.gameObject;

        renderer.sprite = tileUnselected;
        
        // Create a new Canvas for fade effect
        canvasGO = new GameObject("FadeCanvas");
        fadeCanvas = canvasGO.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        
        // Create a full-screen Image
        GameObject imageGO = new GameObject("FadeImage");
        imageGO.transform.SetParent(canvasGO.transform, false);
        fadeImage = imageGO.AddComponent<Image>();
        fadeImage.color = new Color(0f, 0f, 0f, 0f); // start transparent
        RectTransform rt = fadeImage.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // Create "YOU DIED!" text
        GameObject textGO = new GameObject("DeathText");
        textGO.transform.SetParent(canvasGO.transform, false);
        deathText = textGO.AddComponent<Text>();
        deathText.text = "YOU DIED!";
        deathText.alignment = TextAnchor.MiddleCenter;
        // Load custom font from Resources

        deathText.font = Resources.Load<Font>("Fonts/MyFont");

        deathText.fontSize = 80;
        deathText.color = new Color(1f, 0f, 0f, 0f); // Start fully transparent

        RectTransform textRT = deathText.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;

    }

    public static string SerializeItemList(List<Item> itemList)
    {
        List<ItemData> dataList = new List<ItemData>();

        foreach (Item item in itemList)
        {
            ItemData data = new ItemData
            {
                name = item.name,
                isBreakable = item.isBreakable,
                scale = item.scale,
                position = item.position,
                isDefaultItem = item.isDefaultItem,
                attackRate = item.attackRate,
                strengthModifier = item.strengthModifier,
                defenceModifier = item.defenceModifier,
                speedModifier = item.speedModifier,
                glowModifier = item.glowModifier,
                itemID = item.itemID,
                icon = item.icon
            };

            dataList.Add(data);
        }

        ItemDataListWrapper wrapper = new ItemDataListWrapper { items = dataList };
        return JsonUtility.ToJson(wrapper, true);
    }

    void SaveToJson()
    {
        // Player Data
        string filePath = Application.persistentDataPath + "/PlayerStats.json";

        PlayerStats stats = playerStats.GetComponent<PlayerStats>();
        PlayerStatsData data = stats.ToData();
        string playerData = JsonUtility.ToJson(data, true);

        System.IO.File.WriteAllText(filePath, playerData);

        // Inventory Data
        filePath = Application.persistentDataPath + "/PlayerInventory.json";
        List<Item> items = Inventory.instance.items;
        System.IO.File.WriteAllText(filePath, SerializeItemList(items));
        Debug.Log(filePath);
    }
    
    private bool isDying = false;

    void Update()
    {
        if (player == null) return;

        // Door logic (unchanged)
        float distance = Vector3.Distance(sprite.transform.position, player.transform.position);
        if (distance <= radius && !isDying)
        {
            HandleDoorFade();
        }
        else if (!isDying)
        {
            ResetFade();
        }

        // Death logic
        if (PlayerStats.instance.health <= 0 && !isDying)
        {
            PlayerStats.instance.health = 0; // Clamp
            StartCoroutine(HandleDeathFade());
        }
    }

    private void HandleDoorFade()
    {
        canvasGO.SetActive(true);
        renderer.sprite = tileSelected;
        timer += Time.deltaTime;
        fadeAlpha = Mathf.Clamp01(timer / timeToTransition);
        fadeImage.color = new Color(0f, 0f, 0f, fadeAlpha);
        deathText.color = new Color(1f, 0f, 0f, fadeAlpha); 

        if (timer >= timeToTransition)
        {
            SaveToJson();
            if (string.IsNullOrEmpty(sceneToLoad))
                SceneManager.LoadScene(gameObject.GetComponent<LevelPicker>().GetNextScene(false, PlayerStats.instance.level));
            else
                SceneManager.LoadScene(sceneToLoad);
        }
    }

    private void ResetFade()
    {
        canvasGO.SetActive(false);
        renderer.sprite = tileUnselected;
        timer = 0f;
        fadeAlpha = 0f;
        fadeImage.color = new Color(0f, 0f, 0f, fadeAlpha);
    }

    private IEnumerator HandleDeathFade()
    {
        isDying = true;
        Time.timeScale = 0f; // Pause game immediately

        canvasGO.SetActive(true);
        renderer.sprite = tileSelected;

        float fadeTime = 0f;
        while (fadeTime < timeToTransition)
        {
            fadeTime += Time.unscaledDeltaTime; // works while paused
            fadeAlpha = Mathf.Clamp01(fadeTime / timeToTransition);

            // Fade background
            fadeImage.color = new Color(50f / 255f, 0f, 0f, fadeAlpha);

            // Fade in text at same rate
            deathText.color = new Color(1f, 0f, 0f, fadeAlpha);

            yield return null;
        }

        // Example: lose half XP, reload scene
        PlayerStats.instance.gainExperience(-(PlayerStats.instance.experience / 2));
        PlayerStats.instance.health = PlayerStats.instance.maxHealth;

        SaveToJson();

        if (string.IsNullOrEmpty(sceneToLoad))
            SceneManager.LoadScene(gameObject.GetComponent<LevelPicker>().GetNextScene(true, PlayerStats.instance.level));
        else
            SceneManager.LoadScene(sceneToLoad);

        Time.timeScale = 1f; // Reset timescale for new scene
    }

}
