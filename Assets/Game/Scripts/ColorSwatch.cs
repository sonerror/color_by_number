using UnityEngine;

public class ColorSwatch : MonoBehaviour
{
    public int ID { get; private set; }
    private SpriteRenderer _renderer;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void SetData(int id, Color color)
    {
        ID = id;
        _renderer.color = color;
    }

    public void SetSelected(bool selected)
    {
        // ví dụ: làm sáng hoặc tối màu khi được chọn
        _renderer.color *= selected ? 1.2f : 0.8f;
    }

    public void SetCompleted()
    {
        // ví dụ: làm mờ khi hoàn thành
        _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 0.5f);
    }
}
