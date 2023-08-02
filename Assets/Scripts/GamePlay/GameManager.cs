using Extensions;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private TextMeshProUGUI gameStateText;

    [Header("Game State Text Content")]
    [SerializeField] private string winText;
    [SerializeField] private Color winColor;
    [SerializeField] private string loseText;
    [SerializeField] private Color loseColor;

    [Header("ENTER Icon")]
    [SerializeField] private RectTransform enterIcon;
    [SerializeField] private float xOffset;

    [Header("States")]
    [SerializeField] private Tile.TileState emptyState;
    [SerializeField] private Tile.TileState activeState;
    [SerializeField] private Tile.TileState incorrectState;
    [SerializeField] private Tile.TileState wrongSpotState;
    [SerializeField] private Tile.TileState correctState;

    private string[] answerWords;
    private string[] validWords;
    private string currWord;
    private int currRow = 0, currTile = 0;
    private Tile activeTile;
    private Animator activeTileAnimator;

    private UIManager uiManager;

    private void Awake()
    {
        board.emptyState = emptyState;
        uiManager = GetComponent<UIManager>();
    }

    private void Start()
    {
        LoadData();
        NewGame();

        enterIcon.gameObject.SetActive(false);
        gameStateText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (enabled)
            HandleUserInput();
    }

    private void OnEnable()
    {
        if (gameStateText)
            gameStateText.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (gameStateText)
            gameStateText.gameObject.SetActive(true);
    }

    public void NewGame()
    {
        Debug.Log("Starting a new game...");

        uiManager.SetMenuInactive();
        enabled = true;

        board.Clear();

        currTile = currRow = 0;
        activeTile = board.rows[currRow].tiles[currTile];
        activeTileAnimator = activeTile.GetComponent<Animator>();
        HighlightCurrentTile();

        SelectRandomWord();
        Debug.Log("Chosen word : " + currWord);
    }

    private void HandleUserInput()
    {
        // Get characters input by the user this frame
        foreach (char c in Input.inputString)
        {
            if (c == '\b') // Backspace
            {
                if (currTile > 0)
                {
                    board.rows[currRow].RemoveLetter(currTile - 1);
                    currTile--;
                }

                if (enterIcon.gameObject.activeSelf)
                    enterIcon.gameObject.SetActive(false);
            }
            else if (c == '\n' || c == '\r' || c == ' ') // Enter or Space
            {
                bool isValid = ValidateCurrentRow();

                if (isValid)
                    enterIcon.gameObject.SetActive(false);

                if (currRow < board.rowCount && isValid)
                {
                    SubmitCurrentRow();
                    currTile = 0;
                    currRow++;
                }
            }
            else if (char.IsLetter(c))
            {
                if (currTile < board.rows[currRow].tileCount)
                {
                    board.rows[currRow].AppendLetter(char.ToLower(c), currTile);
                    currTile++;
                }

                HandleEnterIcon();
            }

            if (0 <= currRow && currRow < board.rowCount)
                currTile = Mathf.Clamp(currTile, 0, board.rows[currRow].tileCount);

            if (enabled)
                HighlightCurrentTile();
        }
    }

    private void SubmitCurrentRow()
    {
        // Storing the remaining letter to avoid redunduncy when checking for a letter
        // each letter will be removed from the word when checked as correct
        string remaining = currWord;

        int i = 0;
        foreach (Tile tile in board.rows[currRow].tiles)
        {
            // First check
            // Check the correct and incorrect tiles
            if (currWord[i] == tile.letter)
            {
                board.rows[currRow].correctTileCount++;
                StartCoroutine(tile.SetState(correctState, i * 0.1f, true));
                remaining = remaining.Replace(' ', i);
            }
            else if (!currWord.Contains(tile.letter))
            {
                StartCoroutine(tile.SetState(incorrectState, i * 0.1f, true));
            }

            i++;
        }

        i = 0;
        foreach (Tile tile in board.rows[currRow].tiles)
        {
            // Second check
            // Go through again to avoid counting the same letter twice
            if (tile.state != correctState && tile.state != incorrectState)
            {
                if (remaining.Contains(tile.letter))
                {
                    board.rows[currRow].wrongSpotTileCount++;

                    StartCoroutine(tile.SetState(wrongSpotState, i * 0.1f, true));

                    int index = remaining.IndexOf(tile.letter);
                    remaining = remaining.Replace(' ', index);
                }
                else
                {
                    StartCoroutine(tile.SetState(incorrectState, i * 0.1f, true));
                }
            }

            i++;
        }

        if (HasWon())
        {
            gameStateText.text = winText;
            gameStateText.color = winColor;
            enabled = false;

            uiManager.SetGameState(UIManager.GameState.Win);
            StartCoroutine(uiManager.SetMenuActive());
        }
        else if (currRow + 1 >= board.rowCount)
        {
            gameStateText.text = loseText;
            gameStateText.color = loseColor;
            enabled = false;

            // It is almost a win if the player gets exactly 4 correct letters
            // or if they get at least 2 correct letters and 2 letters in the wrong spot
            if (board.rows[currRow].correctTileCount == 4 ||
                (board.rows[currRow].correctTileCount >= 2 && board.rows[currRow].wrongSpotTileCount >= 2))
                uiManager.SetGameState(UIManager.GameState.Almost);
            else
                uiManager.SetGameState(UIManager.GameState.Lose);

            uiManager.SetWordLabel(currWord.ToUpper());
            StartCoroutine(uiManager.SetMenuActive());
        }
    }

    private bool ValidateCurrentRow()
    {
        if (board.rows[currRow].filledTileCount < board.rows[currRow].tileCount)
            return false;

        // Convert the row into an array of char
        char[] chars = board.rows[currRow].tiles.Select(e => e.letter).ToArray();

        // Search for the input word in the list of valid word
        // Return true if found, and false otherwise
        return validWords.Find(chars) >= 0;
    }

    private void HighlightCurrentTile()
    {
        if (activeTile.state == activeState)
        {
            StartCoroutine(activeTile.SetState(emptyState));
            activeTileAnimator.SetBool("Highlighted", false);
        }

        if (0 <= currRow && currRow < board.rowCount &&
            0 <= currTile && currTile < board.rows[currRow].tileCount)
        {
            activeTile = board.rows[currRow].tiles[currTile];
            StartCoroutine(activeTile.SetState(activeState));

            activeTileAnimator = activeTile.GetComponent<Animator>();
            activeTileAnimator.SetBool("Highlighted", true);
        }
    }

    private void HandleEnterIcon()
    {
        bool isValid = ValidateCurrentRow();

        if (isValid)
        {
            RectTransform row = board.rows[currRow].rectTransform;

            float xPos = row.sizeDelta.x / 2 + row.anchoredPosition.x + xOffset;
            float yPos = row.anchoredPosition.y;

            enterIcon.anchoredPosition = new Vector2(xPos, yPos);
            enterIcon.gameObject.SetActive(true);
        }
        else
        {
            enterIcon.gameObject.SetActive(false);
        }
    }

    private void SelectRandomWord()
    {
        int index = Random.Range(0, answerWords.Length);
        currWord = answerWords[index].ToLower().Trim();
    }

    private void LoadData()
    {
        TextAsset textFile = Resources.Load("answerwords") as TextAsset;
        answerWords = textFile.ToString().Split('\n');

        textFile = Resources.Load("validwords") as TextAsset;
        validWords = textFile.ToString().Split('\n');
    }

    private bool HasWon()
    {
        foreach (Tile tile in board.rows[currRow].tiles)
        {
            if (tile.state != correctState)
                return false;
        }

        return true;
    }
}
