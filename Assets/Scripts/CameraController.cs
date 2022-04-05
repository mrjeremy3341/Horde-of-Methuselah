using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera cam;
    public Color confirmColor;
    public Color denyColor;
    public float panSpeed;
    public float zoomSpeed;
    public float panBorder;
    public Vector2 boundaries;
    public SpriteRenderer cursor;
    public SpriteRenderer icon;

    float zoom = 15;
    float minZoom = 5;
    float maxZoom = 50;

    private void Update()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int gridPos = ScreenToGrid(mousePos);
        Tile t = GameManager.instance.mapGrid.GetTile(gridPos.x, gridPos.y);
        GameManager.instance.placeTile = t;
        cursor.transform.position = t.worldPos;
        if (GameManager.instance.isPlacingBuilding)
        {
            if (GameManager.instance.isPlacementValid)
            {
                cursor.color = confirmColor;
            }
            else
            {
                cursor.color = denyColor;
            }

            icon.gameObject.SetActive(true);
        }
        else if (GameManager.instance.isSelling)
        {
            if (GameManager.instance.isValidSell)
            {
                cursor.color = confirmColor;
            }
            else
            {
                cursor.color = denyColor;
            }

            icon.gameObject.SetActive(false);
        }
        else
        {
            cursor.color = Color.white;
            icon.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - panBorder)
        {
            pos.y += panSpeed * Time.deltaTime * (zoom / 10);
        }
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= panBorder)
        {
            pos.y -= panSpeed * Time.deltaTime * (zoom / 10);
        }
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - panBorder)
        {
            pos.x += panSpeed * Time.deltaTime * (zoom / 10);
        }
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= panBorder)
        {
            pos.x -= panSpeed * Time.deltaTime * (zoom / 10);
        }

        float difference = Mathf.Clamp(zoom - 20, 0, zoom);
        pos.x = Mathf.Clamp(pos.x, -(boundaries.x - difference), boundaries.x - difference);
        pos.y = Mathf.Clamp(pos.y, -(boundaries.y - difference), boundaries.y - difference);
        transform.position = pos;

        if(Input.mouseScrollDelta.y > 0)
        {
            zoom -= zoomSpeed * Time.deltaTime * (zoom / 10);
            zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
            cam.orthographicSize = zoom;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            zoom += zoomSpeed * Time.deltaTime * (zoom / 10);
            zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
            cam.orthographicSize = zoom;
        }
    }

    Vector2Int ScreenToGrid(Vector3 pos)
    {
        int x = Mathf.FloorToInt((pos.x * 6) + 300);
        int y = Mathf.FloorToInt((pos.y * 6) + 300);
        x = Mathf.Clamp(x, 1, 598);
        y = Mathf.Clamp(y, 1, 598);

        return new Vector2Int(x, y);
    }
}
