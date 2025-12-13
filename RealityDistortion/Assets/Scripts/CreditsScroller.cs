using UnityEngine;
using TMPro;

public class CreditsScroller : MonoBehaviour
{
    [Header("References")]
    public RectTransform contentRoot;      // obiectul care conține textele
    public TextMeshProUGUI creditsText;    // textul 1
    public TextMeshProUGUI contributionsText; // textul 2
    public TextMeshProUGUI creditsTitle; // textul 2

    [Header("Scroll Settings")]
    public float scrollSpeed = 50f;        // viteza derulării
    public float spacing = 200f;           // spațiu între cele două blocuri de text

    private float totalHeight;

    void Start()
    {
        // Plasăm textele unul sub altul
        PositionTexts();

        // Calculăm înălțimea totală care trebuie derulată
        totalHeight = contentRoot.sizeDelta.y + Screen.height;
    }

    void Update()
    {
        // Derulăm continutul în sus
        contentRoot.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
    }

    void PositionTexts()
    {
        Canvas.ForceUpdateCanvases();

        float titleH = creditsTitle.preferredHeight;
        float creditsH = creditsText.preferredHeight;
        float contribH = contributionsText.preferredHeight;

        // TOTAL DIMENSIUNE:
        float totalHeight =
            titleH +
            spacing +
            creditsH +
            spacing +
            contribH;

        contentRoot.sizeDelta = new Vector2(contentRoot.sizeDelta.x, totalHeight);

        // +-----------------------+
        // | Bloc 1: TITLU         |
        // +-----------------------+
        creditsTitle.rectTransform.anchoredPosition = new Vector2(0, 0);

        // +-----------------------+
        // | Bloc 2: CREDITS       |
        // +-----------------------+
        creditsText.rectTransform.anchoredPosition =
            new Vector2(0, -(titleH + spacing));

        // +-----------------------+
        // | Bloc 3: CONTRIBUTIONS |
        // +-----------------------+
        contributionsText.rectTransform.anchoredPosition =
            new Vector2(0, -(titleH + spacing + creditsH + spacing));
    }
}
