using TMPro;
using UnityEngine;

public class UIHome : UICanvas
{
    [SerializeField] TextMeshProUGUI textLevel;

    /* private void Awake()
     {
     }*/
    public override void Setup()
    {
        base.Setup();
        int level = DataManager.Ins.playerData.levelCurrent + 1;
        textLevel.text = level.ToString();

    }
    public override void Open()
    {
        UIManager.Ins.CloseAll();
        base.Open();
    }
    public void BtnPlay()
    {
        UIManager.Ins.CloseAll();
        UIManager.Ins.OpenUI<UIGameplay>();
        LevelManager.Ins.OnLoadLevel(DataManager.Ins.playerData.levelCurrent, () =>
        {
            CameraController.Ins.CenterCameraOnLevel(LevelManager.Ins.CurrentLevelData);
            GameManager.Ins.ChangeState(GameState.MainMenu);
        });
    }
}
