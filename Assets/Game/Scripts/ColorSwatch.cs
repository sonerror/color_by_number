using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AssetKits.ParticleImage;
public class ColorSwatch : MonoBehaviour
{
    public int ID { get; private set; }

    bool Completed;
    bool Selected;
    [SerializeField] TextMeshProUGUI Text;
    [SerializeField] Image Background;
    [SerializeField] Image Border;
    [SerializeField] List<Pixel> listPixelByID;
    [SerializeField] ParticleImage onCompleteParticle; 
    public void SetData(int id, Color color)
    {
        ID = id;
        Text.text = id.ToString();
        Background.color = color;
        Border.color = Color.black;
    }
    public void PlayOnCompleteParticle()
    {
        onCompleteParticle.startColor = Background.color;
        onCompleteParticle.Play();
    }
    public void SetCompleted()
    {
        Completed = true;
        Text.text = "";
        Border.color = Color.gray;
        Selected = false;
    }

    public void SetSelected(bool selected)
    {
        if (!Completed)
        {
            Selected = selected;
            if (Selected)
            {
                Border.color = Color.yellow;
            }
            else
            {
                Border.color = Color.black;
            }
        }
        else
        {
            Border.color = Color.gray;
        }
    }

    public void OnClick()
    {
        Debug.Log("Color Swatch ID Selected: " + ID);
        LevelManager.Ins.SetSelectedColorID(ID);
    }
}