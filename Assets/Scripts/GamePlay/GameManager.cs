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

    private UIManager uiManager;

    private void Start()
    {
        board.emptyState = emptyState;

        uiManager = GetComponent<UIManager>();

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
        HighlightCurrentTile();

        SelectRandomWord();
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
            if (currWord[i] == tile.letter)
            {
                tile.SetState(correctState);
                remaining = remaining.Replace(' ', i);
            }
            else if (!currWord.Contains(tile.letter))
            {
                tile.SetState(incorrectState);
            }

            i++;
        }

        i = 0;
        foreach (Tile tile in board.rows[currRow].tiles)
        {
            if (tile.state != correctState && tile.state != incorrectState)
            {
                if (remaining.Contains(tile.letter))
                {
                    tile.SetState(wrongSpotState);

                    int index = remaining.IndexOf(tile.letter);
                    remaining = remaining.Replace(' ', index);
                }
                else
                {
                    tile.SetState(incorrectState);
                }
            }

            i++;
        }

        if (HasWon())
        {
            gameStateText.text = winText;
            gameStateText.color = winColor;
            enabled = false;

            uiManager.SetGameState(true);
            uiManager.SetMenuActive();
        }
        else if (currRow + 1 >= board.rowCount)
        {
            gameStateText.text = loseText;
            gameStateText.color = loseColor;
            enabled = false;

            uiManager.SetGameState(false);
            uiManager.SetMenuActive();
            uiManager.SetWordLabel(currWord.ToUpper());
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
            activeTile.SetState(emptyState);

        if (0 <= currRow && currRow < board.rowCount &&
            0 <= currTile && currTile < board.rows[currRow].tileCount)
        {
            activeTile = board.rows[currRow].tiles[currTile];
            activeTile.SetState(activeState);
        }
    }

    private void HandleEnterIcon()
    {
        bool isValid = ValidateCurrentRow();

        if (isValid)
        {
            RectTransform row = board.rows[currRow].rectTransform;

            float xPos = row.sizeDelta.x / 2 + row.position.x + xOffset;
            float yPos = row.position.y;

            enterIcon.position = new Vector2(xPos, yPos);

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
