using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    #region +
    public GameObject candyPrefab;//要实例化的糖果的prefab
    public GameObject ExplosionFX;//爆炸特效 
    public Text ScoreText;
    public AudioClip swapClip;
    public AudioClip explodeClip;
    public AudioClip match3Clip;
    public AudioClip wrongClip;


    #endregion

    #region -
    private AudioSource audioSource;
    private int rowNum = 8;//行数
    private int columnNum = 11;//列数
    private Candy firstCandy;//第一次点击的那个糖果
    private ArrayList candyArray;//所有糖果的行列集合
    private ArrayList matches;
    private GameObject go;//临时游戏物体
    private Candy candy;//临时Candy脚本
    private int rIndex;//临时记录的行索引，只用于两个糖果交换
    private int cIndex;//临时记录的列索引，只用于两个糖果交换
    #endregion

    #region 分数的只读属性方法
    private float sum = 0;//记录总分数的

    public float Sum
    {
        get
        {
            return sum;
        }
    }
    #endregion

    #region  Awake单例
    public static GameController _i;

    void Awake()
    {
        _i = this;
    }
    #endregion

    #region Unity内置函数
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        candyArray = new ArrayList();

        for (int rIndex = 0; rIndex < rowNum; rIndex++)
        {
            ArrayList temp = new ArrayList();

            for (int cIndex = 0; cIndex < columnNum; cIndex++)
            {
                temp.Add(AddCandy(rIndex, cIndex));
            }

            candyArray.Add(temp);
        }

        //第一次检测糖果是否可以消除
        if (CheckMatches())
        {
            RemoveMatches();
        }

        if (Application.loadedLevelName == "PlayGame")
        {
            CountDown.startTime = Time.time;
        }
    }
    void Update()
    {
        if (CountDown.GameOver)//游戏结束
        {
            DDL._i.currentScore = sum;
            sum = 0;
            SceneManager.LoadScene(2);
        }
    }
    #endregion

    #region -AddCandy实例化添加一个糖果，返回糖果上的Candy脚本
    private Candy AddCandy(int rIndex, int cIndex)
    {
        go = Instantiate(candyPrefab) as GameObject;
        candy = go.GetComponent<Candy>();

        candy.transform.parent = this.transform;
        candy.cIndex = cIndex;
        candy.rIndex = rIndex;
        candy.UpdatePosition();//更新位置

        return candy;
    }
    #endregion

    #region 对糖果集合的获取和设置的操作

    #region -GetCandy从糖果集合里获取单个糖果
    private Candy GetCandy(int rIndex, int cIndex)
    {
        ArrayList temp = candyArray[rIndex] as ArrayList;
        Candy candy = temp[cIndex] as Candy;

        return candy;
    }
    #endregion

    #region -SetCandy设置糖果集合里的单个糖果
    private void SetCandy(int rIndex, int cIndex, Candy candy)
    {
        ArrayList temp = candyArray[rIndex] as ArrayList;
        temp[cIndex] = candy;
    }
    #endregion

    #endregion

    #region +Select点击糖果
    public void Select(Candy candy)
    {
        if (firstCandy == null)//点击后发现第一次点击的糖果为空
        {
            firstCandy = candy;//那就将第一次点击的糖果记录下来
            firstCandy.Selected = true;
            return;
        }
        else//表示已经有第一次点击的糖果了，按这就是第二次点击了，要交换两个糖果了
        {
            #region 三消交换的核心算法
            //1.第一个点击的糖果的行索引减去第二个点击的糖果的行索引的绝对值为0或1
            //2.第一个点击的糖果的列索引减去第二个点击的糖果的列索引的绝对值为0或1
            //如果上面1和2的结果相加的和为1那就表示第二个点击的糖果在第一个点击的糖果的同一列的上面或下面，同一行的左边或右边，这样才能交换两个糖果
            if ((Mathf.Abs(firstCandy.rIndex - candy.rIndex) + Mathf.Abs(firstCandy.cIndex - candy.cIndex)) == 1)
            {
                //交换糖果的位置，在检测是否可以消除，可以的话就消除，然后在糖果集合里修改数据，播放糖果移动动画，实例化特效播放音乐
                //如果检测到不能交换的话那就把这两个糖果再换回到原来的位置
                StartCoroutine(ExchangeTwo(firstCandy, candy));
            }
            else//如果点击的糖果无法交换
            {
                audioSource.PlayOneShot(wrongClip);//播放音乐
            }
            #endregion

            firstCandy.Selected = false;
            firstCandy = null;//这一步一定要置空不然会点一个交换一个
        }
    }
    #endregion

    #region -Exchange交换两个糖果的位置
    private void Exchange(Candy candy1, Candy candy2)
    {
        audioSource.PlayOneShot(swapClip);//糖果的交换的音乐

        //重新设置糖果集合里的数据
        SetCandy(candy2.rIndex, candy2.cIndex, candy1);
        SetCandy(candy1.rIndex, candy1.cIndex, candy2);

        //记录candy1也就是第一个点击时记录下来的糖果的行列索引
        rIndex = candy1.rIndex;
        cIndex = candy1.cIndex;

        //交换candy1的行列索引
        candy1.rIndex = candy2.rIndex;
        candy1.cIndex = candy2.cIndex;

        //交换candy2的行列索引
        candy2.rIndex = rIndex;
        candy2.cIndex = cIndex;

        //同时执行candy1和candy2的位置更新方法
        candy1.TweenToPosition();
        candy2.TweenToPosition();
    }
    #endregion

    #region -Remove删除糖果并创建新的糖果同时实现糖果的缓动效果
    private void Remove(Candy candy)
    {
        AddEffect(candy.transform.position);
        audioSource.PlayOneShot(explodeClip);//消除糖果发出的爆炸声音
        candy.Dispose();//释放内存

        //更新界面上的图像，使当前这一列的糖果在消除调一个之后能往下移动
        //但是糖果行列集合里面是没有改的
        int cIndex = candy.cIndex;//临时记录的列索引
        for (int rIndex = candy.rIndex + 1; rIndex < rowNum; rIndex++)
        {
            Candy candy2 = GetCandy(rIndex, cIndex);//获取传进来的糖果上面的一些糖果
            candy2.rIndex--;//重新设置这个糖果的行索引
            candy2.TweenToPosition();//糖果的移动动画

            SetCandy(rIndex - 1, cIndex, candy2);//这样就把糖果行列集合里的数据都修改了
        }

        //补全糖果向下移动后的空位
        Candy newCandy = AddCandy(rowNum - 1, cIndex);//在当前行的最大行索引的位置随机生成一个糖果
        newCandy.rIndex = rowNum;//重新设置行索引为行数这样糖果就在蓝色背景的上面了
        newCandy.UpdatePosition();//只是设置了行索引还不行，还得更新位置这样就会在界面上显示
        newCandy.rIndex--;//再减1就把它的行索引设置为正确的了
        newCandy.TweenToPosition();//最后加上糖果的移动动画就会出现一种糖果从上往下掉下来的效果
        SetCandy(rowNum - 1, cIndex, newCandy);//然后设置行列集合里的糖果的数据，保持一致这样下次消除糖果就不会出错了
    }
    #endregion

    #region  三消核心检测算法

    #region -CheckMatches检测没有没可以消除的3个以上同行或同列的糖果
    private bool CheckMatches()
    {
        //只要有一行或者一列里面存在可以消除的糖果就行了
        return CheckHorizontalMatches() || CheckVerticalMatches();
    }
    #endregion

    #region -CheckHorizontalMatches检测同一行3个以上相同的糖果
    private bool CheckHorizontalMatches()
    {
        bool result = false;

        for (int rIndex = 0; rIndex < rowNum; rIndex++)
        {
            //columnNum - 2这是因为只要知道3个糖果里面的第一个就能得出后面的两个糖果了
            for (int cIndex = 0; cIndex < columnNum - 2; cIndex++)
            {
                //当前行第一列的糖果的索引号等于当前行第二列的糖果的索引号                                并且  当前行第二列的糖果的索引号等于当前行第三列的糖果的索引号
                if ((GetCandy(rIndex, cIndex).bgIndex == GetCandy(rIndex, cIndex + 1).bgIndex) && (GetCandy(rIndex, cIndex + 2).bgIndex == GetCandy(rIndex, cIndex + 1).bgIndex))
                {
                    audioSource.PlayOneShot(match3Clip);//播放消除糖果的音乐
                    result = true;//结果true表示可以消除了

                    //添加这3个相同的糖果到集合中
                    AddMatch(GetCandy(rIndex, cIndex));//添加糖果1
                    AddMatch(GetCandy(rIndex, cIndex + 1));//添加糖果2
                    AddMatch(GetCandy(rIndex, cIndex + 2));//添加糖果3
                }
            }
        }
        return result;
    }
    #endregion

    #region -CheckVerticalMatches检测同一列3个以上相同的糖果与上面的检测同行一样，只是遍历的循序相反
    private bool CheckVerticalMatches()
    {
        bool result = false;

        for (int cIndex = 0; cIndex < columnNum; cIndex++)
        {
            for (int rIndex = 0; rIndex < rowNum - 2; rIndex++)
            {
                if ((GetCandy(rIndex, cIndex).bgIndex == GetCandy(rIndex + 1, cIndex).bgIndex) && (GetCandy(rIndex + 2, cIndex).bgIndex == GetCandy(rIndex + 1, cIndex).bgIndex))
                {
                    audioSource.PlayOneShot(match3Clip);
                    result = true;

                    AddMatch(GetCandy(rIndex, cIndex));
                    AddMatch(GetCandy(rIndex + 1, cIndex));
                    AddMatch(GetCandy(rIndex + 2, cIndex));
                }
            }
        }

        return result;
    }
    #endregion

    #region -AddMatch检测到同行或同列有3个以上相同的糖果时记录下来
    private void AddMatch(Candy candy)
    {
        if (matches == null)//集合为空的话
        {
            matches = new ArrayList();//就创建一个新的集合
        }

        int index = matches.IndexOf(candy);//返回当前糖果在集合里的索引

        //这样就可以避免重复的了  
        //比如 1比2(相同)并且2比3(相同) 那就会添加糖果1 2 3
        //然后 2比3(相同)并且3比4(相同) 那就会添加糖果2 3 4
        //但是原来已经有糖果2 3了再次返回糖果2 3的索引就不是-1了，这样糖果2 3就不会添加到集合里去了
        if (index == -1)//等于-1表示集合里没有这个糖果
        {
            matches.Add(candy);//那就可以添加这个糖果了
        }
    }
    #endregion

    #region -RemoveMatches检测到有相同的糖果后就可以进行删除了,删除的是检测方法里相同糖果集合里的数据
    private void RemoveMatches()
    {
        Candy temp;//一个临时的糖果
        for (int i = 0; i < matches.Count; i++)
        {
            temp = matches[i] as Candy;
            Remove(temp);//这里不能用clear，要用自定义的删除
        }

        Score(matches.Count);
        matches = new ArrayList();//重新创建一个，不然就是空的不能添加糖果了
        StartCoroutine(WaitAndCheck());//调用协程
    }
    #endregion

    #endregion

    #region WaitAndCheck检测消除协程
    IEnumerator WaitAndCheck()//间隔一定的时间再次检测，有相同的糖果就消除
    {
        yield return new WaitForSeconds(0.2f);

        if (CheckMatches())
        {
            RemoveMatches();
        }
    }
    #endregion

    #region Exchange2协程
    IEnumerator ExchangeTwo(Candy candy1, Candy candy2)
    {
        Exchange(candy1, candy2);//先交换

        yield return new WaitForSeconds(0.2f);//然后暂停0.7秒

        if (CheckMatches())//最后在检测是否可以消除
        {
            RemoveMatches();//检测到有3个以上相同的糖果了那就进行消除
        }
        else//没有检测到可以消除的糖果
        {
            Exchange(candy1, candy2);//那就将这两个交换位置后的糖果再交换回去到原来的位置
        }
    }
    #endregion

    #region -AddEffect添加爆炸特效
    private void AddEffect(Vector3 pos)
    {
        Instantiate(ExplosionFX, pos, Quaternion.identity);

        CameraShake.shakeFor(0.5f, 0.05f);//相机震动
    }
    #endregion

    #region  -Score分数加成的方法
    void Score(int count)
    {
        if (count < 3)
        {
            return;//小于3调用这个方法肯定是有问题了
        }

        //根据matches.Count的数量来成倍增加分数
        //1.3个就乘100=300分
        //2.4个就乘120=480分
        //3.5个就乘150=750分
        //4.6个就乘200=1200分
        //5.7个就乘300=2100分
        //6.8个就乘450=3600分
        //7.9个就乘666=5994分
        switch (count)
        {
            case 3:
                count *= 100;
                break;
            case 4:
                count *= 120;
                break;
            case 5:
                count *= 150;
                break;
            case 6:
                count *= 200;
                break;
            case 7:
                count *= 300;
                break;
            case 8:
                count *= 450;
                break;
            case 9:
                count *= 666;
                break;
            default:
                //这里大于9个的---全部乘以1000---一般情况下不太可能
                count *= 1000;
                break;
        }

        sum += count;
        ScoreText.text = "Score：" + sum.ToString();
    }
    #endregion
}
