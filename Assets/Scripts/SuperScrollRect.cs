using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class SuperScrollRect : MonoBehaviour
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

    public Action<GameObject> initChildrenCallback = null;
    public Action<int, string> updateChildrenCallback = null;

    public bool isInitChildren;

    public GridLayoutGroup gridLayoutGroup;

    public Vector2 startPosition;

    public bool isScrollTo;

    private void Start()
    {
        this.Init();
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
            go.name = i + "";
            this.InitChilrenCallback(go);
        }

        isInitChildren = true;

        SetAmount(this.totalAmount);
    }


    private void ScrollCallBack(Vector2 data)
    {
        if (isScrollTo == false)
        {
            UpdateListView();
        }
    }

    public List<GameObject> tmpList = new List<GameObject>();

    private void UpdateListView()
    {
        bool needUpdate = false;
        Vector2 currentPos = content.anchoredPosition;
        if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            float upPosY = (startIndex + 1) * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);
            float bottomPosY = startIndex * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);
            //列
            if (currentPos.y > upPosY)
            {
                if (lastIndex < Mathf.CeilToInt(totalAmount / gridLayoutGroup.constraintCount))
                {
                    tmpList.Clear();
                    int index = 0;
                    GameObject go = null;
                    for (int i = 0; i < gridLayoutGroup.constraintCount; i++)
                    {
                        index = i;

                        go = itemList[index];

                        if (lastIndex * gridLayoutGroup.constraintCount + index < totalAmount)
                        {
                            go.SetActive(true);
                            UpdateChildrenCallback(index + lastIndex * gridLayoutGroup.constraintCount, go);
                        }
                        else
                        {
                            go.SetActive(false);
                        }
                        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(go.GetComponent<RectTransform>().anchoredPosition.x, -(gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y) * lastIndex);

                        tmpList.Add(go);
                    }

                    for (int i = 0; i < tmpList.Count; i++)
                    {
                        go = tmpList[i];
                        itemList.Remove(go);
                        itemList.Add(go);
                    }

                    startIndex++;
                    lastIndex++;

                    needUpdate = true;
                }
            }
            else if (currentPos.y < bottomPosY)
            {
                if (startIndex > 0)
                {
                    tmpList.Clear();
                    int index = 0;
                    startIndex--;
                    for (int i = 0; i < gridLayoutGroup.constraintCount; i++)
                    {
                        index = itemList.Count - gridLayoutGroup.constraintCount + i;

                        itemList[index].SetActive(true);

                        itemList[index].GetComponent<RectTransform>().anchoredPosition = new Vector2(itemList[index].GetComponent<RectTransform>().anchoredPosition.x, -(gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y) * startIndex);

                        UpdateChildrenCallback(i + startIndex * gridLayoutGroup.constraintCount, itemList[index]);

                        tmpList.Add(itemList[index]);
                    }

                    for (int i = 0; i < tmpList.Count; i++)
                    {
                        GameObject go = tmpList[i];
                        itemList.Remove(go);
                        itemList.Insert(i, go);
                    }


                    lastIndex--;

                    needUpdate = true;
                }
            }
        }
        else
        {
            //行
            float leftPosX = -(startIndex + 1) * (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x);
            float rightPosX = -startIndex * (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x);
            if (currentPos.x < leftPosX)
            {
                if (lastIndex < Mathf.CeilToInt(totalAmount / gridLayoutGroup.constraintCount))
                {
                    tmpList.Clear();
                    int index = 0;

                    GameObject go = null;
                    for (int i = 0; i < gridLayoutGroup.constraintCount; i++)
                    {
                        index = i;

                        go = itemList[index];
                        if (lastIndex * gridLayoutGroup.constraintCount + index < totalAmount)
                        {
                            go.SetActive(true);
                            UpdateChildrenCallback(index + lastIndex * gridLayoutGroup.constraintCount, go);
                        }
                        else
                        {
                            go.SetActive(false);
                        }

                        go.GetComponent<RectTransform>().anchoredPosition = new Vector2((gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x) * lastIndex, go.GetComponent<RectTransform>().anchoredPosition.y);


                        tmpList.Add(go);
                    }

                    for (int i = 0; i < tmpList.Count; i++)
                    {
                        go = tmpList[i];
                        itemList.Remove(go);
                        itemList.Add(go);
                    }

                    startIndex++;
                    lastIndex++;

                    needUpdate = true;
                }
            }
            else if (currentPos.x > rightPosX)
            {
                if (startIndex > 0)
                {
                    tmpList.Clear();
                    int index = 0;
                    startIndex--;
                    for (int i = 0; i < gridLayoutGroup.constraintCount; i++)
                    {
                        index = itemList.Count - gridLayoutGroup.constraintCount + i;

                        itemList[index].SetActive(true);

                        itemList[index].GetComponent<RectTransform>().anchoredPosition = new Vector2((gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x) * startIndex, itemList[index].GetComponent<RectTransform>().anchoredPosition.y);

                        UpdateChildrenCallback(i + startIndex * gridLayoutGroup.constraintCount, itemList[index]);

                        tmpList.Add(itemList[index]);
                    }

                    for (int i = 0; i < tmpList.Count; i++)
                    {
                        GameObject go = tmpList[i];
                        itemList.Remove(go);
                        itemList.Insert(i, go);
                    }


                    lastIndex--;

                    needUpdate = true;
                }
            }
        }

        //是否需要更新下一次
        if (needUpdate)
        {
            this.UpdateListView();
        }
    }


    //创建item
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

    public void SetAmount(float amount)
    {
        this.totalAmount = amount;
        if (isInitChildren)
        {
            UpdateContentSize();
            UpdateAllView();
        }
    }

    private void UpdateContentSize()
    {
        Vector2 size = Vector2.zero;
        if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            if (this.totalAmount > 0)
            {
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

        content.sizeDelta = size;
    }

    private void UpdateAllView(int startIndex = 0)
    {
        scrollRect.StopMovement();


        //确定位置
        if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            float targetY = 0;

            targetY = startIndex * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);
            if (content.sizeDelta.y - targetY < sizeDelta.y)
            {
                if (content.sizeDelta.y < sizeDelta.y)
                {
                    startIndex = 0;
                    targetY = 0;
                }
                else
                {
                    startIndex = Mathf.FloorToInt((content.sizeDelta.y - sizeDelta.y) / (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y));
                    targetY = content.sizeDelta.y - sizeDelta.y;
                }
            }
            content.anchoredPosition3D = new Vector3(content.anchoredPosition3D.x, targetY);
        }
        else
        {
            float targetX = 0;

            targetX = startIndex * (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x);
            if (content.sizeDelta.x - targetX < sizeDelta.x)
            {
                if (content.sizeDelta.x < sizeDelta.x)
                {
                    startIndex = 0;
                    targetX = 0;
                }
                else
                {
                    startIndex = Mathf.FloorToInt((content.sizeDelta.x - sizeDelta.x) / (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x));
                    targetX = content.sizeDelta.x - sizeDelta.x;
                }
            }
            content.anchoredPosition3D = new Vector3(-targetX, content.anchoredPosition3D.y);
        }


        this.startIndex = startIndex;
        this.lastIndex = this.startIndex;

        Vector2 pos = Vector2.zero;

        int index = 0;
        for (int i = 0; i < itemList.Count; i++)
        {
            GameObject go = itemList[i];
            index = this.startIndex * gridLayoutGroup.constraintCount + i;
            if (index >= totalAmount)
            {
                go.SetActive(false);
            }
            else
            {
                go.SetActive(true);

                UpdateChildrenCallback(index, go);

                if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
                {
                    pos.x = Mathf.CeilToInt(index % gridLayoutGroup.constraintCount) * (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x);
                    pos.y = -Mathf.CeilToInt(index / gridLayoutGroup.constraintCount) * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);
                }
                else
                {
                    pos.x = Mathf.CeilToInt(index / gridLayoutGroup.constraintCount) * (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x);
                    pos.y = -Mathf.CeilToInt(index % gridLayoutGroup.constraintCount) * (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);
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
    public void UpdateChildrenCallback(int index, GameObject go)
    {
        if (updateChildrenCallback != null)
        {
            updateChildrenCallback(index, go.name);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="go"></param>
    public void InitChilrenCallback(GameObject go)
    {
        if (initChildrenCallback != null)
        {
            initChildrenCallback(go);
        }
    }


}
