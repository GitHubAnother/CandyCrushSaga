using UnityEngine;

public class ReturnHeightScore
{
    /// <summary>
    /// 返回本地存储的最高分数
    /// </summary>
    /// <param name="currentScore">一局游戏结束后的当前分数</param>
    /// <param name="ScoreName">指定存储分数的名称</param>
    /// <returns>本地最高得分</returns>
    public static string Score(float currentScore, string ScoreName)
    {
        float height = PlayerPrefs.GetFloat(ScoreName);

        if (currentScore > height)
        {
            PlayerPrefs.SetFloat(ScoreName, currentScore);
        }

        return PlayerPrefs.GetFloat(ScoreName).ToString();
    }
}
