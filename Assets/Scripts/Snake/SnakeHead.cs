using System;
using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SnakeHead : MonoBehaviour
{
    public int snakeLength = 5;
    public float stepLength = .5f;
    
    private LineRenderer _lineRenderer;
    private Camera mainCam;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        mainCam = Camera.main;
        StartCoroutine(SnakeTick());
    }


    private IEnumerator SnakeTick()
    {
        yield return new WaitForSeconds(.1f);
        Vector3 mousePosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector2 direction = mousePosition - transform.position;

        Vector3 newPoint = transform.position + (Vector3) direction.normalized * stepLength;
        transform.position = newPoint;

        Vector3[] points = new Vector3[_lineRenderer.positionCount];
        _lineRenderer.GetPositions(points);

        var pointsList = points.ToList();
        
        if(_lineRenderer.positionCount >= snakeLength) {pointsList.RemoveAt(0);}
        else { _lineRenderer.positionCount++; }
        pointsList.Add(newPoint);
        
        _lineRenderer.SetPositions(pointsList.ToArray());
        
        StartCoroutine(SnakeTick());
    }
}
