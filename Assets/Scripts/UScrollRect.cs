using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UScrollRect : MonoBehaviour
{
    public GameObject itemCellPrefab;

    public string itemCellPath;

    public ScrollRect scrollRect;
    //
    public RectTransform content;

    /// <summary>
    /// 当前正在使用的Item
    /// </summary>
    public List<GameObject> itemList = new List<GameObject>();

    /// <summary>
    /// 回收后的Item
    /// </summary>
    public List<GameObject> itemPool = new List<GameObject>();

    public int startIndex = 0;
    public int lastIndex = 0;

    public Vector2 sizeDelta;

    /// <summary>
    /// 最小的数量  根据宽高得来的
    /// </summary>
    public int minAmount = 0;

    /// <summary>
    /// 拓展
    /// </summary>
    public int extend = 1;

    public float totalAmount = 0;

    public delegate void UpdateChildrenCallbackDelegate(int index, GameObject go, int srcIndex);
    public UpdateChildrenCallbackDelegate updateChildrenCallback = null;

    public bool isInitChildren;

    public GridLayoutGroup gridLayoutGroup;

    public Vector2 startPosition;

    private void Start()
    {
        if (isInitChildren == false)
        {
            this.Init();
        }
    }

    private void Init()
    {
        content = scrollRect.content;
        gridLayoutGroup = content.GetComponent<GridLayoutGroup>();
        //位置代码控制
        gridLayoutGroup.enabled = false;
        //
        scrollRect.onValueChanged.AddListener((data) => { ScrollCallBack(data); });
        sizeDelta = this.GetComponent<RectTransform>().sizeDelta;

        //行
        int col = 0;
        //列
        int row = 0;
        if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            //固定列
            col = gridLayoutGroup.constraintCount;
            row = Mathf.CeilToInt(sizeDelta.y / (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y));

            minAmount = col * (row + extend);
        }
        else
        {
            //固定行
            col = Mathf.CeilToInt(sizeDelta.x / (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x));
            row = gridLayoutGroup.constraintCount;

            minAmount = (col + extend) * row;
        }

        //初始化
        StartCoroutine(InitChildren());
    }

    IEnumerator InitChildren()
    {
        yield return 0;

        for (int i = 0; i < minAmount; i++)
        {
            GameObject go = this.CreateItem();
        }

        isInitChildren = true;
    }


    private void ScrollCallBack(Vector2 data)
    {
        if (itemList.Count > 0)
        {
            UpdateListView();
        }
    }

    public List<GameObject> tmpList = new List<GameObject>();

    private void UpdateListView()
    {
        Vector2 currentPos = content.anchoredPosition;
        if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            float upPosY = (startIndex + 1) * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);
            float bottomPosY = startIndex * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);
            //列
            if (currentPos.y > upPosY)
            {
                print("totalIndex" + Mathf.CeilToInt(totalAmount / gridLayoutGroup.constraintCount) + " lastIndex=" + lastIndex);
                if (lastIndex < Mathf.CeilToInt(totalAmount / gridLayoutGroup.constraintCount))
                {
                    tmpList.Clear();
                    int index = 0;
                    for (int i = 0; i < gridLayoutGroup.constraintCount; i++)
                    {
                        index = i;

                        itemList[index].GetComponent<RectTransform>().anchoredPosition = new Vector2(itemList[index].GetComponent<RectTransform>().anchoredPosition.x, -(gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y) * lastIndex);

                        updateChildrenCallback(index + lastIndex * gridLayoutGroup.constraintCount, itemList[index], index + startIndex * gridLayoutGroup.constraintCount);

                        tmpList.Add(itemList[index]);
                    }

                    for (int i = 0; i < tmpList.Count; i++)
                    {
                        GameObject go = tmpList[i];
                        itemList.Remove(go);
                        itemList.Add(go);
                    }

                    startIndex++;
                    lastIndex++;
                    print("越界y up");
                }
            }
            else if(currentPos.y<bottomPosY)
            {
                print("越界y bottom"+bottomPosY);
                if (startIndex >0)
                {
                    tmpList.Clear();
                    int index = 0;
                    startIndex--;
                    for (int i = 0; i < gridLayoutGroup.constraintCount; i++)
                    {
                        index = itemList.Count - gridLayoutGroup.constraintCount + i;

                        itemList[index].GetComponent<RectTransform>().anchoredPosition = new Vector2(itemList[index].GetComponent<RectTransform>().anchoredPosition.x, -(gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y) * startIndex);

                        updateChildrenCallback(i + startIndex * gridLayoutGroup.constraintCount, itemList[index], index + startIndex * gridLayoutGroup.constraintCount);

                        tmpList.Add(itemList[index]);
                    }

                    for (int i = 0; i < tmpList.Count; i++)
                    {
                        GameObject go = tmpList[i];
                        itemList.Remove(go);
                        itemList.Insert(i, go);
                    }

                    
                    lastIndex--;
                }
            }
        }
        else
        {
            //行
            float leftPosX = (startIndex + 1) * (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x);
            if (currentPos.x < -leftPosX)
            {
                startIndex++;
                lastIndex++;
                print("越界x");
            }
        }

        startPosition = currentPos;
    }

    private GameObject CreateItem()
    {
        GameObject item = null;
        if (string.IsNullOrEmpty(itemCellPath))
        {
            item = Instantiate(itemCellPrefab);
            item.transform.SetParent(content);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;
            itemList.Add(item);
        }
        return item;
    }

    //回收
    private void RecoveryItem(GameObject item)
    {
        if (itemPool.Contains(item) == false)
        {
            itemPool.Add(item);
        }
        if (itemList.Contains(item) == true)
        {
            itemList.Remove(item);
        }
        item.SetActive(false);
    }

    //添加到首位置
    private void AddItemToStart()
    {
        GameObject item = this.CreateItem();
        item.SetActive(true);
    }

    //添加到末位置
    private void AddItemToEnd()
    {

    }

    //移除首位置
    private void RemoveItemFromStart()
    {

    }

    //移除末位置
    private void RemoveItemFromEnd()
    {

    }


    public void SetAmount(int amount)
    {
        this.totalAmount = amount;

        if (isInitChildren == false)
        {
            this.Init();
        }

        UpdateContentSize();
        UpdateAllView();
    }

    private void UpdateContentSize()
    {
        Vector2 size = Vector2.zero;
        if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            if (this.totalAmount > 0)
            {
                print("xxxx" + Mathf.CeilToInt(this.totalAmount / gridLayoutGroup.constraintCount));
                //列 列 列
                size.x = gridLayoutGroup.constraintCount * (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x) - gridLayoutGroup.spacing.x;
                size.y = Mathf.CeilToInt(this.totalAmount / gridLayoutGroup.constraintCount) * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y) - gridLayoutGroup.spacing.y;
            }
        }
        else
        {
            if (this.totalAmount > 0)
            {
                //行
                //行
                //行
                size.x = Mathf.CeilToInt(this.totalAmount / gridLayoutGroup.constraintCount) * (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x) - gridLayoutGroup.spacing.x;
                size.y = gridLayoutGroup.constraintCount * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y) - gridLayoutGroup.spacing.y;
            }
        }

        print("size = " + size);
        content.sizeDelta = size;
    }

    private void UpdateAllView(int startIndex = 0)
    {
        this.startIndex = startIndex;
        this.lastIndex = startIndex;

        Vector2 pos = Vector2.zero;

        for (int i = 0; i < itemList.Count; i++)
        {
            GameObject go = itemList[i];
            if (this.startIndex + i >= totalAmount)
            {
                go.SetActive(false);
            }
            else
            {
                go.SetActive(true);

                UpdateChildrenCallback(i, go, i);

                if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
                {
                    pos.x = Mathf.CeilToInt(i % gridLayoutGroup.constraintCount) * (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x);
                    pos.y = -Mathf.CeilToInt(i / gridLayoutGroup.constraintCount) * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);
                }
                else
                {
                    pos.x = Mathf.CeilToInt(i / gridLayoutGroup.constraintCount) * (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x);
                    pos.y = -Mathf.CeilToInt(i % gridLayoutGroup.constraintCount) * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);
                }
                if (i % gridLayoutGroup.constraintCount == 0)
                {
                    this.lastIndex++;
                }
                go.GetComponent<RectTransform>().anchoredPosition = pos;
            }
        }
    }

    public void ScrollTo(int num)
    {
        UpdateAllView(num);
    }

    public void InitList(int num)
    {
        this.SetAmount(num);
    }


    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="index"></param>
    /// <param name="trans"></param>
    /// <param name="srcIndex"></param>
    public void UpdateChildrenCallback(int index, GameObject go, int srcIndex)
    {
        if (updateChildrenCallback != null)
        {
            updateChildrenCallback(index, go, srcIndex);
        }
    }


}
