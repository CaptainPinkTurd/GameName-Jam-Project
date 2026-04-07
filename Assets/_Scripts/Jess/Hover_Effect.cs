using UnityEngine;
using UnityEngine.EventSystems;

public class Hover_Effect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float sizeMultipler = 1.2f;
    public float speed = 10f;

    private Vector3 origScale;
    private Vector3 targetScale;
    void Start()
    {
        origScale = transform.localScale;
        targetScale = origScale;
    }

    public void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = origScale * sizeMultipler;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = origScale;
    }
}
