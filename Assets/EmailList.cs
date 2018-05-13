using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmailList : MonoBehaviour {

    public SuperScrollRect uscrollRect;
    public InputField inputField;
    public Dictionary<string, GameObject> dict = new Dictionary<string, GameObject>();
	// Use this for initialization
	void Start () {
        uscrollRect.initChildrenCallback = InitChildrenCallback;
        uscrollRect.updateChildrenCallback = UpdateChildrenCallback;
	}

    private void InitChildrenCallback(GameObject go)
    {
        dict.Add(go.name, go);
    }

    private void UpdateChildrenCallback(int index, string name)
    {
        print(string.Format("index = {0},go.name={1}", index, name));
        dict[name].transform.Find("Text").GetComponent<Text>().text = index+"";
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
