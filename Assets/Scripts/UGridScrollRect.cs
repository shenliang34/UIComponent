using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UGridScrollRect : MonoBehaviour
{
    public Transform container;

    public GridLayoutGroup gridLayoutGroup;

    public Stack<GameObject> pools = new Stack<GameObject>();

    public List<GameObject> itemlist = new List<GameObject>();

    public GameObject itemCell;

    private static float buttonHeight = 30;

    public Vector2 sizeDelta = Vector2.zero;


    // Use this for initialization
    void Start()
    {
        sizeDelta = this.GetComponent<RectTransform>().sizeDelta;

        StartCoroutine(InitChildren());
    }

    private IEnumerator InitChildren()
    {
        yield return 0;
    }



    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }

    private static void UpdatePosition()
    {

    }

    private void OnGUI()
    {
        float vy = 0;
        if (GUI.Button(new Rect(0, vy, 100,buttonHeight), "添加一个"))
        {
            print("添加一个");
            this.CreateItem();
        }

        if (GUI.Button(new Rect(0, vy+=buttonHeight, 100,buttonHeight), "删除前面一个"))
        {
            print("删除前面一个");
            this.CreateItem();
        }

        if (GUI.Button(new Rect(0, vy += buttonHeight, 100, buttonHeight), "删除后面一个"))
        {
            print("删除后面一个");
            this.CreateItem();
        }
    }

    public void PushInPool(GameObject item)
    {
        pools.Push(item);
    }

    //
    public GameObject PullOutPool()
    {
        GameObject item = null;
        if (pools.Count>0)
        {
            item = pools.Pop();
        }
        return item;
    }

    public GameObject CreateItem()
    {
        GameObject item = PullOutPool();
        if (item == null)
        {
            item = Instantiate(itemCell) as GameObject;
            item.transform.SetParent(container);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            item.SetActive(true);
            itemlist.Add(item);
        }
        return item;
    }
}
