using System;
using UnityEngine;

public class TestPointVisualizer : MonoBehaviour
{
    public Transform planeOrigin;       // Центр плоскости
    public Vector3 planeNormal = Vector3.up; // Нормаль плоскости
    public GameObject markerPrefab;     // Префаб сферы или точки

    private GameObject currentMarker;

    void Start()
    {
        // Создаём маркер, если задан префаб
        if (markerPrefab != null)
        {
            currentMarker = Instantiate(markerPrefab);
            currentMarker.SetActive(false);
        }
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(planeNormal, planeOrigin.position);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);

            if (currentMarker != null)
            {
                currentMarker.SetActive(true);
                currentMarker.transform.position = hitPoint;
            }

            Debug.Log("Точка под мышью на плоскости: " + hitPoint);
        }
        else
        {
            if (currentMarker != null)
                currentMarker.SetActive(false);
        }
    }
}