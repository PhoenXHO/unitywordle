using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private const string inactiveClass = "inactive";

    private GameManager gameManager;
    private VisualElement menu;
    private Label gameStateLabel;
    private Label wordLabel;

    private Button newGameButton;
    private Button restartButton;
    private Button quitButton;

    private VisualElement answerContainer;
    private Label congratsLabel;

    [SerializeField] private string winText;
    [SerializeField] public string loseText;
    [SerializeField] public string almostText;

    [Header("Post-Processing")]
    [SerializeField] private Volume volume;

    private DepthOfField depthOfField;

    private void Awake()
    {
        volume.profile.TryGet(out depthOfField);
    }

    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        gameManager = GetComponent<GameManager>();
        menu = root.Q<VisualElement>("Menu");
        gameStateLabel = menu.Q<Label>("GameStateLabel");
        wordLabel = menu.Q<Label>("Word");

        newGameButton = menu.Q<Button>("NewGameButton");
        restartButton = root.Q<VisualElement>("RestartButton").Q<Button>();
        quitButton = root.Q<VisualElement>("QuitButton").Q<Button>();

        answerContainer = menu.Q<VisualElement>("Answer");
        congratsLabel = menu.Q<Label>("Congrats");

        newGameButton.clicked += gameManager.NewGame;
        restartButton.clicked += gameManager.NewGame;
        quitButton.clicked += Application.Quit;
    }

    private void Update()
    {
        if (!gameManager.enabled && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)))
        {
            gameManager.NewGame();
        }
    }

    public IEnumerator SetMenuActive()
    {
        yield return new WaitForSeconds(1f);
        depthOfField.active = true;
        menu.RemoveFromClassList(inactiveClass);
    }
    public void SetMenuInactive()
    {
        depthOfField.active = false;
        menu.AddToClassList(inactiveClass);
    }
    public void SetGameState(GameState state)
    {
        // Show the answer only if the player lost
        answerContainer.style.display = (state == GameState.Win) ? DisplayStyle.None : DisplayStyle.Flex;
        // Show the congrats message only if the player won
        congratsLabel.style.display = (state != GameState.Win) ? DisplayStyle.None : DisplayStyle.Flex;

        if (state == GameState.Almost)
            gameStateLabel.text = almostText;
        else if (state == GameState.Lose)
            gameStateLabel.text = loseText;
        else if (state == GameState.Win)
            gameStateLabel.text = winText;
    }
    public void SetWordLabel(string word)
    {
        wordLabel.text = word;
    }

    public enum GameState
    {
        Almost,
        Lose,
        Win
    }
}
