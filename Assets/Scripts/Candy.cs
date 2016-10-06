using UnityEngine;

public class Candy : MonoBehaviour
{
    #region +
    public int candyNumber = 6;//生成不同糖果的个数
    public GameObject[] bgs;

    [HideInInspector]
    public float Xoff = -6f;//X轴偏移
    [HideInInspector]
    public float Yoff = -5f;//Y轴偏移
    [HideInInspector]
    public int bgIndex;//糖果背景图片的索引号
    [HideInInspector]
    public int rIndex = 0;//行索引
    [HideInInspector]
    public int cIndex = 0;//列索引
    #endregion

    #region -
    private GameObject bg;
    private SpriteRenderer spriteRenderer;
    #endregion

    #region Unity内置函数
    void OnMouseDown()
    {
        GameController._i.Select(this);
    }
    #endregion

    #region +selected选中后的变化
    public bool Selected
    {
        set
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = value ? Color.blue : Color.white;
            }
        }
    }
    #endregion

    #region +UpdatePosition位置更新
    public void UpdatePosition()
    {
        AddRandomBG();
        transform.position = new Vector3(cIndex + Xoff, rIndex + Yoff, 0f);
    }
    #endregion

    #region -AddRandomBG添加随机背景图片
    private void AddRandomBG()
    {
        if (bg != null)
        {
            return;
        }

        candyNumber = Mathf.Clamp(candyNumber, 6, bgs.Length);
        bgIndex = Random.Range(0, candyNumber);
        bg = Instantiate(bgs[bgIndex]) as GameObject;
        bg.transform.parent = this.transform;

        spriteRenderer = bg.GetComponent<SpriteRenderer>();
    }
    #endregion

    #region +Dispose销毁后的内存释放处理
    public void Dispose()
    {
        Destroy(bg.gameObject);
        Destroy(this.gameObject);
    }
    #endregion

    #region +TweenToPosition糖果的移动效果
    public void TweenToPosition()
    {
        AddRandomBG();//随机生成一个糖果
        iTween.MoveTo(this.gameObject, iTween.Hash("x", cIndex + Xoff, "y", rIndex + Yoff, "time", 0.16f));
    }
    #endregion
}
