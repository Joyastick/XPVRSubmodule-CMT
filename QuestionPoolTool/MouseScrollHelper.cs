using UnityEngine;

public class MouseScrollHelper : MonoBehaviour
{
    public RectTransform rect;
    public Vector2 startPos;
    public float yDiff;
    public bool isDragging;
    public float topLimit;
    private void Start()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;
    }

    private void Update()
    {
        isDragging = Input.GetMouseButton(0);
        if (Input.GetMouseButtonDown(0)) yDiff = rect.anchoredPosition.y - Input.mousePosition.y;
        if (!isDragging) return;
        var currentY = Input.mousePosition.y + yDiff;
        topLimit = startPos.y + rect.rect.height * .666f;
        if (currentY > topLimit)
            currentY = topLimit;
        
        if (currentY < startPos.y)
            currentY = startPos.y;
        rect.anchoredPosition = new Vector2(startPos.x, currentY);
    }
}