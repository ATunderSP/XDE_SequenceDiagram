using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SequenceDiagram : MonoBehaviour {

    public GameObject classPrefab;
    public GameObject messagePrefab;

    private Transform classes;
    private Transform messages;

    private Dictionary<string, Transform> classesDict;

    IEnumerator Test()
    {
        AddClass("test1");
        AddClass("test2");
        AddClass("test3");
        Canvas.ForceUpdateCanvases();
        AddMessage("test1", "test2", "bla()");
        AddMessage("test2", "test3", "bla()");
        AddMessage("test3", "test2", "bla()", true);
        yield return new WaitForSeconds(5);
        RemoveClass("test3");
        yield return new WaitForEndOfFrame();
        AddClass("test3");
        AddMessage("test2", "test3", "bla()", true);
    }

    private void Awake()
    {
        classes = transform.Find("ClassesHL");
        messages = transform.Find("MessagesVL");
        classesDict = new Dictionary<string, Transform>();
    }

    // Use this for initialization
    void Start () {
        //StartCoroutine(Test());
    }

    public Transform AddClass(string name)
    {
        if(!classesDict.ContainsKey(name) || classesDict[name] == null)
        {
            var cls = Instantiate(classPrefab, classes);
            var tmp = cls.GetComponentInChildren<TextMeshProUGUI>();
            cls.name = tmp.text = name;
            classesDict[name] = cls.transform;
        }
        return classesDict[name];
    }

    public void AddMessage(string from, string to, string message, bool dashed=false)
    {
        var msg = Instantiate(messagePrefab, messages);
        var label = msg.transform.Find("label");
        var msgScript = msg.GetComponent<Message>();
        var fromClass = AddClass(from);
        var toClass = AddClass(to);
        msgScript.fromClass = fromClass.GetComponent<RectTransform>();
        msgScript.toClass = toClass.GetComponent<RectTransform>();
        msgScript.dashed = dashed;
        label.GetComponent<TextMeshProUGUI>().text = message;
    }

    public void RemoveMessage(string from, string to)
    {
        //TODO
        //destroy
        //but need unique id and from to is not unique
        //probably will be possible from UI only
    }

    public void RemoveClass(string name)
    {
        var toRemove = classesDict[name];
        Destroy(toRemove.gameObject);
        classesDict.Remove(name);
    }

}
