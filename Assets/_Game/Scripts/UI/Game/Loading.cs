using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Image imgFill;
    public float duration = 2f;
    private void Awake()
    {
        imgFill.fillAmount = 0;
        RunSlider();
    }

    public void RunSlider()
    {
        DOVirtual.Float(0, 1, duration, (value) =>
        {
            imgFill.fillAmount = value;
        }).SetEase(Ease.InCubic)
        .OnComplete(() =>
        {
            DOVirtual.DelayedCall(1f, () =>
            {
                SceneController.Ins.ChangeScene(Const.HOME_SCENE, () =>
                {
                    //UIManager.Ins.OpenUI<UIHome>();
                    LevelManager.Ins.OnLoadLevel(1, () =>
                    {
                        CameraController.Ins.CenterCameraOnLevel(LevelManager.Ins.CurrentLevelData);
                        GameManager.Ins.ChangeState(GameState.MainMenu);
                    });
                });
            });
        });
    }

}
