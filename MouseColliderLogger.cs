
using HighlightPlus;
using Unity.VisualScripting;
using UnityEngine;

public class MouseColliderLogger : MonoBehaviour
{
    // 当鼠标进入 Collider 区域时调用
    private void OnMouseEnter()
    {
        if (isMove) {
            Debug.Log("鼠标已进入 Collider: " + gameObject.name);
            this.transform.GetComponent<HighlightEffect>().highlighted = true;
        }
    }

    // 当鼠标在 Collider 区域内时每帧调用（可选，用于持续检测）
    private void OnMouseOver()
    {
        Debug.Log("鼠标已悬浮 Collider: " + gameObject.name);
        // 如果只需要进入/离开事件，此方法可留空或删除
    }

    public bool isMove;
    public bool isShowMeau;
    // 当鼠标离开 Collider 区域时调用
    private void OnMouseExit()
    {
        if (isMove)
        {
            this.transform.GetComponent<HighlightEffect>().highlighted = false;
        }
        Debug.Log("鼠标已离开 Collider: " + gameObject.name);
    }
    // 当鼠标在 Collider 上按下时调用
    private void OnMouseDown()
    {
        Debug.Log("鼠标点击了 Collider: " + gameObject.name);
        GameMgr._instance.Current_Click_T = this.transform;
        GameMgr._instance.CurrentClick_Shine(this.transform);


        if (isMove) {
            GameMgr._instance.Translateto();
        }

        if (isShowMeau) {
            //this.transform.GetComponent<HighlightEffect>().highlighted = true;
        }

    }

}
