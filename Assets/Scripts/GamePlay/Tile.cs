using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    private Image image;
    private Outline outline;
    private Animator animator;
    private TextMeshProUGUI letterTMP;

    public char letter { get; private set; }
    public TileState state { get; private set; }

    private void Awake()
    {
        image = GetComponent<Image>();
        outline = GetComponent<Outline>();
        animator = GetComponent<Animator>();
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
    public IEnumerator SetState(TileState state, float delay = 0f, bool flip = false)
    {
        this.state = state;

        if (flip)
        {
            yield return new WaitForSeconds(delay);
            animator.SetTrigger("Flip");
            yield return new WaitForSeconds(0.1f);
        }

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
