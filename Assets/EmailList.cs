using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmailList : MonoBehaviour {

    public UScrollRect uscrollRect;
    public InputField inputField;
	// Use this for initialization
	void Start () {
        uscrollRect.updateChildrenCallback = UpdateChildrenCallback;
	}

    private void UpdateChildrenCallback(int index, GameObject go, int srcIndex)
    {
        print(string.Format("index = {0},go.name={1},srcIndex={2} ", index, go.name, srcIndex));
        go.transform.Find("Text").GetComponent<Text>().text = index+"";
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void ScrollTo()
    {
        int num = 0;
        int.TryParse(inputField.text, out num);
        uscrollRect.ScrollTo(num);
    }

    public void InitList()
    {
        int num = 0;
        int.TryParse(inputField.text, out num);
        uscrollRect.InitList(num);
    }
}
