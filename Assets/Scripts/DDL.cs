using UnityEngine;
using System.Collections;

public class DDL : MonoBehaviour
{
    #region 字段
    public float currentScore = 0;
    #endregion

    #region 单例
    public static DDL _i;

    void Awake()
    {
        _i = this;
    }
    #endregion

    #region Unity内置函数
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion
}
