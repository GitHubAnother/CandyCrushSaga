using UnityEngine;
using System;

public class MoveBackGround : MonoBehaviour
{
    #region +
    public float moveSpeed = 2f;
    public float downPosition = -8.52f;
    public Transform[] BGs = null;
    #endregion

    #region Unity内置方法
    void Update()
    {
        try
        {
            foreach (Transform go in BGs)
            {
                go.Translate(Vector3.down * moveSpeed * Time.deltaTime);

                if (go.position.y <= downPosition)
                {
                    go.position = new Vector3(
                        go.position.x,
                        go.position.y + (Mathf.Abs(downPosition) * BGs.Length),
                        go.position.z);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("背景图片数组至少有一个元素！  " + e.Message);
        }
    }
    #endregion
}
