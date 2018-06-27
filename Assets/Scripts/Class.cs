using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class Class : MonoBehaviour {

    private GameObject diagram;
    private Transform lifeline;
    private UILineRenderer lifelineRenderer;

	// Use this for initialization
	void Start () {
        diagram = GameObject.Find("SequenceDiagramVL");
        lifeline = transform.Find("lifeline");
        lifelineRenderer = lifeline.GetComponent<UILineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        var csize = lifeline.parent.GetComponent<RectTransform>().sizeDelta;
        var toX = csize.x * 0.5f;
        var toY = diagram.GetComponent<RectTransform>().sizeDelta.y;

        lifelineRenderer.Points[0] = new Vector2(toX, -csize.y);
        lifelineRenderer.Points[1] = new Vector2(toX, -toY);
        lifelineRenderer.SetVerticesDirty();
	}
}
