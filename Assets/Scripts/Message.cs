using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using System.Linq;

public class Message : MonoBehaviour
{
    public RectTransform fromClass;
    public RectTransform toClass;
    public bool dashed = false;
    public float segmentWidth = 5;

    protected virtual float Width
    {
        get { return Mathf.Abs(fromClass.anchoredPosition.x - toClass.anchoredPosition.x); }
    }

    protected virtual bool Reversed
    {
        get { return fromClass.anchoredPosition.x > toClass.anchoredPosition.x; }
    }

    protected virtual int Orientation
    {
        get { return Reversed ? 0 : 180; }
    }

    protected virtual int Direction
    {
        get { return Reversed ? -1 : 1; }
    }

    protected virtual float StartPosX
    {
        get { return Reversed ? toClass.anchoredPosition.x : fromClass.anchoredPosition.x; }
    }

    protected virtual List<Vector2> LinePoints
    {
        get
        {
            var points = new List<Vector2>();

            points.Add(new Vector2(0, 0));
            if (dashed)
            {
                for (float i = 0; i < Width; i += segmentWidth)
                {
                    points.Add(new Vector2(i, 0));
                }
            }
            if (points.Count % 2 == 0)
            {
                points.RemoveAt(points.Count - 1);
            }
            points.Add(new Vector2(Width, 0));

            return points;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if target or source class removed delete message as well
        if(fromClass == null || toClass == null)
        {
            Destroy(gameObject);
            return;
        }

        var line = transform.Find("line");
        var label = transform.Find("label");
        var arrow = line.Find("arrow");

        UILineRenderer lr = line.GetComponent<UILineRenderer>();
        var rta = arrow.GetComponent<RectTransform>();

        rta.anchoredPosition = new Vector2(Direction * (Width - rta.sizeDelta.x) * 0.5f, 0);
        arrow.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, Orientation, 0);
        GetComponent<VerticalLayoutGroup>().padding.left = (int)StartPosX;

        line.GetComponent<LayoutElement>().minHeight = rta.sizeDelta.y;
        var rt = line.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(Width, 0); //using 0 since height is ignored due to layout

        rt = label.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(Width, 0);

        lr.Points = LinePoints.ToArray();
    }
}
