using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI - Общий счёт")]
    public TextMeshProUGUI totalScoreText;

    [Header("UI - Счётчики блоков")]
    public TextMeshProUGUI brainrotCountText;
    public TextMeshProUGUI coalCountText;
    public TextMeshProUGUI copperCountText;
    public TextMeshProUGUI ironCountText;
    public TextMeshProUGUI goldCountText;
    public TextMeshProUGUI lapisCountText;
    public TextMeshProUGUI redstoneCountText;
    public TextMeshProUGUI diamondCountText;
    public TextMeshProUGUI emeraldCountText;

    [Header("Настройки очков")]
    public int scorePerBlock = 1;

    // Общий счёт (все блоки кроме брейнрота)
    private int totalScore = 0;

    // Индивидуальные счётчики для каждого типа
    private int brainrotCount = 0;
    private int coalCount = 0;
    private int copperCount = 0;
    private int ironCount = 0;
    private int goldCount = 0;
    private int lapisCount = 0;
    private int redstoneCount = 0;
    private int diamondCount = 0;
    private int emeraldCount = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateAllUI();
    }

    // Брейнрот - ТОЛЬКО в свою категорию, БЕЗ общего скора
    public void AddBrainrot()
    {
        brainrotCount++;
        UpdateAllUI();
    }

    // Уголь - в общий скор + в свою категорию
    public void AddCoal()
    {
        coalCount++;
        totalScore += scorePerBlock;
        UpdateAllUI();
    }

    // Медь - в общий скор + в свою категорию
    public void AddCopper()
    {
        copperCount++;
        totalScore += scorePerBlock;
        UpdateAllUI();
    }

    // Железо - в общий скор + в свою категорию
    public void AddIron()
    {
        ironCount++;
        totalScore += scorePerBlock;
        UpdateAllUI();
    }

    // Золото - в общий скор + в свою категорию
    public void AddGold()
    {
        goldCount++;
        totalScore += scorePerBlock;
        UpdateAllUI();
    }

    // Лазурит - в общий скор + в свою категорию
    public void AddLapis()
    {
        lapisCount++;
        totalScore += scorePerBlock;
        UpdateAllUI();
    }

    // Редстоун - в общий скор + в свою категорию
    public void AddRedstone()
    {
        redstoneCount++;
        totalScore += scorePerBlock;
        UpdateAllUI();
    }

    // Алмаз - в общий скор + в свою категорию
    public void AddDiamond()
    {
        diamondCount++;
        totalScore += scorePerBlock;
        UpdateAllUI();
    }

    // Изумруд - в общий скор + в свою категорию
    public void AddEmerald()
    {
        emeraldCount++;
        totalScore += scorePerBlock;
        UpdateAllUI();
    }

    void UpdateAllUI()
    {
        // Общий счёт
        if (totalScoreText != null)
        {
            totalScoreText.text = "SCORE: " + totalScore.ToString();
        }

        // Счётчики блоков
        if (brainrotCountText != null)
        {
            brainrotCountText.text = "Brainrot: " + brainrotCount.ToString();
        }

        if (coalCountText != null)
        {
            coalCountText.text = "Coal: " + coalCount.ToString();
        }

        if (copperCountText != null)
        {
            copperCountText.text = "Copper: " + copperCount.ToString();
        }

        if (ironCountText != null)
        {
            ironCountText.text = "Iron: " + ironCount.ToString();
        }

        if (goldCountText != null)
        {
            goldCountText.text = "Gold: " + goldCount.ToString();
        }

        if (lapisCountText != null)
        {
            lapisCountText.text = "Lapis: " + lapisCount.ToString();
        }

        if (redstoneCountText != null)
        {
            redstoneCountText.text = "Redstone: " + redstoneCount.ToString();
        }

        if (diamondCountText != null)
        {
            diamondCountText.text = "Diamond: " + diamondCount.ToString();
        }

        if (emeraldCountText != null)
        {
            emeraldCountText.text = "Emerald: " + emeraldCount.ToString();
        }
    }

    // Геттеры для получения значений
    public int GetTotalScore() => totalScore;
    public int GetBrainrotCount() => brainrotCount;
    public int GetCoalCount() => coalCount;
    public int GetCopperCount() => copperCount;
    public int GetIronCount() => ironCount;
    public int GetGoldCount() => goldCount;
    public int GetLapisCount() => lapisCount;
    public int GetRedstoneCount() => redstoneCount;
    public int GetDiamondCount() => diamondCount;
    public int GetEmeraldCount() => emeraldCount;

    // Сброс всех счётчиков
    public void ResetAll()
    {
        totalScore = 0;
        brainrotCount = 0;
        coalCount = 0;
        copperCount = 0;
        ironCount = 0;
        goldCount = 0;
        lapisCount = 0;
        redstoneCount = 0;
        diamondCount = 0;
        emeraldCount = 0;
        UpdateAllUI();
    }
}
