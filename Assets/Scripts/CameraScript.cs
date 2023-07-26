using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject player;
    [SerializeField] private float buffer;
    [SerializeField] private float smoothSpeed;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        transform.position = player.transform.position;
    }

    void FixedUpdate()
    {
        Vector3 smoothTarget = Vector3.Lerp(transform.position, player.transform.position, smoothSpeed * Time.deltaTime);
        transform.position = smoothTarget;
    }

    private (Vector3 center, float size) CalculateOrthoSize()
    {
        tileMap.CompressBounds();
        var bounds = tileMap.localBounds;
        bounds.Expand(buffer);

        var vertical = bounds.size.y;
        var horizontal = bounds.size.x * cam.pixelHeight / cam.pixelWidth;

        var size = Mathf.Max(horizontal, vertical) * 0.5f;
        var center = bounds.center + new Vector3(0, 0, -10);

        return (center, size);
    }
}
