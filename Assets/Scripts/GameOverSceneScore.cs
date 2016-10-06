using UnityEngine;
using UnityEngine.UI;

public class GameOverSceneScore : MonoBehaviour
{
    #region 字段
    private Text heightText;
    private Text currentText;
    #endregion

    #region Unity内置函数
    void Start()
    {
        heightText = this.transform.Find("HeightScoreTextBG/Text").GetComponent<Text>();
        currentText = this.transform.Find("CurrentScoreTextBG/Text").GetComponent<Text>();

        if (Application.loadedLevelName == "GameOverScene")
        {
            heightText.text = ReturnHeightScore.Score(DDL._i.currentScore, "CandyScore");
            currentText.text = DDL._i.currentScore.ToString();
        }
    }
    #endregion
}
