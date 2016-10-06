/*
作者名称:YHB

脚本作用:倒计时的功能脚本,跟UI-Text并用

建立时间:2016.10.6.10.39
*/

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 这个脚本必须得挂载到用于倒计时的Text上
/// </summary>
public class CountDown : MonoBehaviour
{
    #region ++
    public static float startTime = 0;
    public static float TotalTime = 90f;//倒计时的总时间
    public static bool GameOver = true;//开启或关闭倒计时，一般在进入游戏场景设置为true，结束游戏设置为false
    #endregion

    #region -
    private Text TimeText;
    private float timer;
    private float ShowTextTime;//用来显示的剩余时间
    private float Minutes;//分
    private float Seconds;//秒
    private float Fractions;//秒后面的小数,用乘100在跟100取余来表示
    #endregion

    #region Unity内置函数
    void Start()
    {
        TimeText = this.GetComponent<Text>();
    }
    void Update()//具体的倒计时逻辑
    {
        if (!GameOver)
        {
            timer = Time.time - startTime;

            ShowTextTime = TotalTime - timer;

            //显示到UI之前，算出具体的数值
            Minutes = (int)(ShowTextTime / 60);
            Seconds = (int)(ShowTextTime % 60);
            Fractions = (int)((ShowTextTime * 100) % 100);

            //显示
            TimeText.text = string.Format("{0}:{1:00}:{2:00}:{3:00}", "Time", Minutes, Seconds, Fractions);

            if (ShowTextTime <= 0)
            {
                GameOver = true;
            }
        }
    }
    #endregion
}
