/*
作者名称:YHB

脚本作用:工具脚本  用来播放帧动画

建立时间:2016.9.21.17.33
*/

using System;
using UnityEngine;

public class FrameAnimation : MonoBehaviour
{
    #region + 
    public bool Loop = true;//默认循环播放动画
    public int frameRate = 10;//总时间里要播放的sprite精灵图片张数,帧率，1秒播放几张图片
    public float totalTime = 1f;//总的时间,默认是1秒
    public Sprite[] spriteArray;//sprite精灵图片数组

    [HideInInspector]
    public bool OnceDes = false;//表示播放完一次动画后就销毁自身
    #endregion

    #region -
    private SpriteRenderer spriteRenderer;//sprite精灵图片渲染器
    private float loopTimer = 0;
    private float onceDesTimer = 0;
    #endregion

    #region Unity内置方法
    void Start()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
        }
    }
    void Update()
    {
        PlayAnim();
    }
    #endregion

    #region -PlayAnim用来播放帧动画
    void PlayAnim()
    {
        try
        {
            if (OnceDes)
            {
                Loop = false;
                onceDesTimer += Time.deltaTime;
                int frameIndex = (int)(onceDesTimer / (totalTime / frameRate));

                if (frameIndex >= spriteArray.Length)
                {
                    //确保动画的最后一张图片能被播放出来
                    spriteRenderer.sprite = spriteArray[spriteArray.Length - 1];

                    //动画播放一次播放完了就销毁自身
                    Destroy(this.gameObject);
                }
                else
                {
                    spriteRenderer.sprite = spriteArray[frameIndex];
                }
            }

            if (Loop)
            {
                loopTimer += Time.deltaTime;
                int frameIndex = (int)(loopTimer / (totalTime / frameRate));
                int frame = frameIndex % spriteArray.Length;
                spriteRenderer.sprite = spriteArray[frame];
            }

        }
        catch (Exception e)
        {
            if (frameRate == 0)
            {
                Debug.LogError("动画播放出错---帧率(分母)不能为0 !");
            }
            else
            {
                Debug.LogError("动画播放出错---精灵图片数组不能为空 !");
            }

            Debug.LogError(e.Message);
        }
    }
    #endregion
}
