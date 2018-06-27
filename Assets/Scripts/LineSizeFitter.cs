using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;

[ExecuteInEditMode]
public class LineSizeFitter : MonoBehaviour
{
    public float offset;

    private UILineRenderer lineRenderer;
    private LayoutElement layoutElement;


    private void Awake()
    {
        lineRenderer = GetComponent<UILineRenderer>();
        layoutElement = GetComponent<LayoutElement>();
    }

    void Start()
    {
        UpdateSize();
    }

    public void UpdateSize()
    {
        Debug.Log(lineRenderer.Points[1]);
        var size = CalcSize(lineRenderer.Points);
        Debug.Log(size);
        layoutElement.minWidth = size.x;
        layoutElement.minHeight = size.y;
    }

    private Vector2 CalcSize(IEnumerable<Vector2> points)
    {
        if (points == null || points.Count() == 0)
        {
            return new Vector2(offset, offset);
        }

        var init = points.First();
        float minx = init.x;
        float miny = init.y;
        float maxx = init.x;
        float maxy = init.y;

        foreach (var point in points)
        {
            minx = Mathf.Min(minx, point.x);
            miny = Mathf.Min(miny, point.y);
            maxx = Mathf.Max(maxx, point.x);
            maxy = Mathf.Max(maxy, point.y);
        }

        return new Vector2(maxx - minx + offset, maxy - miny + offset);
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying)
        {
            UpdateSize();
        }
    }
}
