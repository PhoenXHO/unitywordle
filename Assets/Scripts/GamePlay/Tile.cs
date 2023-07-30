using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    private TextMeshProUGUI letterTMP;
    private Image image;
    private Outline outline;

    public char letter { get; private set; }
    public TileState state { get; private set; }

    private void Awake()
    {
        image = GetComponent<Image>();
        outline = GetComponent<Outline>();
        letterTMP = GetComponentInChildren<TextMeshProUGUI>();
        letterTMP.text = "";
    }

    public void SetLetter(char letter)
    {
        this.letter = letter;
        letterTMP.text = letter.ToString();
    }
    public void Clear()
    {
        SetLetter('\0');
    }
    public void SetState(TileState state)
    {
        this.state = state;
        image.color = state.fillColor;
        outline.effectColor = state.outlineColor;
    }

    [Serializable]
    public class TileState
    {
        public Color fillColor;
        public Color outlineColor;
    }
}
