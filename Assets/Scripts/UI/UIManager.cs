using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager;
    private VisualElement menu;
    private Label gameStateLabel;
    private Label wordLabel;

    private Button newGameButton;
    [SerializeField] private Button restartButton;

    private VisualElement answerContainer;
    private Label congratsLabel;

    [SerializeField] private string winText;
    [SerializeField] public string loseText;

    private const string inactiveClass = "inactive";

    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        gameManager = GetComponent<GameManager>();
        menu = root.Q<VisualElement>("Menu");
        gameStateLabel = menu.Q<Label>("GameStateLabel");
        wordLabel = menu.Q<Label>("Word");

        newGameButton = menu.Q<Button>("NewGameButton");
        restartButton = root.Q<VisualElement>("RestartButton").Q<Button>();

        answerContainer = menu.Q<VisualElement>("Answer");
        congratsLabel = menu.Q<Label>("Congrats");

        newGameButton.clicked += gameManager.NewGame;
        restartButton.clicked += gameManager.NewGame;
    }

    private void Update()
    {
        if (!gameManager.enabled && Input.GetKeyDown(KeyCode.Return))
        {
            gameManager.NewGame();
        }
    }

    public void SetMenuActive()
    {
        menu.RemoveFromClassList(inactiveClass);
    }
    public void SetMenuInactive()
    {
        menu.AddToClassList(inactiveClass);
    }
    public void SetGameState(bool hasWon)
    {
        answerContainer.style.display = hasWon ? DisplayStyle.None : DisplayStyle.Flex;
        congratsLabel.style.display = !hasWon ? DisplayStyle.None : DisplayStyle.Flex;

        gameStateLabel.text = hasWon ? winText : loseText;
    }
    public void SetWordLabel(string word)
    {
        wordLabel.text = word;
    }
}
