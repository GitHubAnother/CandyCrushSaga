using UnityEngine;

/// <summary>
/// 鼠标点转射线
/// </summary>
public class MousePositionToRay
{
    /// <summary>
    /// 将鼠标所在的屏幕坐标转换为射线，判断有没有跟所要交互的对象碰到
    /// </summary>
    /// <param name="tagName">转换后的射线所要碰到的GameObject对象的tag标签</param>
    /// <returns>碰到了返回true，没有碰到就返回false</returns>
    public static bool MPSPToRay(string tagName)
    {
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(r, out hit))
        {
            if (hit.collider.tag == tagName)
            {
                return true;
            }
        }

        return false;
    }
}
