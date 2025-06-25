using TMPro;
using UnityEngine;
using DG.Tweening;

public class Pixel : GameUnit
{
    public int ID { get; private set; }
    [SerializeField] TextMeshPro Text;
    [SerializeField] Color PixelColor;
    [SerializeField] SpriteRenderer Background;
    [SerializeField] SpriteRenderer Border;

    private Color initialBackgroundColor;

    public bool IsFilledIn
    {
        get
        {
            return Background.color == PixelColor;
        }
    }

    public void SetData(int id, Color color)
    {
        ID = id;
        PixelColor = color;
        Border.color = new Color(0.95f, 0.95f, 0.95f, 1);
        Text.text = id.ToString();

        initialBackgroundColor = Color.Lerp(new Color(PixelColor.grayscale, PixelColor.grayscale, PixelColor.grayscale), Color.white, 0.85f);
        Background.color = initialBackgroundColor;
    }

    public void SetSelected(bool selected)
    {
        if (!IsFilledIn)
        {
            if (selected)
            {
                Background.color = new Color(0.5f, 0.5f, 0.5f, 1);
            }
            else
            {
                Background.color = initialBackgroundColor;
            }
        }
    }

    public void Fill()
    {
        if (!IsFilledIn)
        {
            Border.color = PixelColor;
            Background.color = PixelColor;
            Text.text = "";
            Background.DOKill();
        }
    }

    public void FillWrong()
    {
        if (!IsFilledIn)
        {
            Background.color = new Color(1, 170 / 255f, 170 / 255f, 1);

            Background.DOKill();

            Background.DOColor(initialBackgroundColor, 0.2f).SetDelay(0.3f);
        }
    }
}