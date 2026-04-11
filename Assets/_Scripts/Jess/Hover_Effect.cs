using UnityEngine;
using UnityEngine.EventSystems;
using CaptainPinkTurd.AudioSystem;

public class Hover_Effect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public SoundData hover;
    public SoundData click;
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
        // Hover
        SoundManager.Instance
            .CreateSoundBuilder()
            .WithPosition(transform.position)
            .WithRandomPitch()
            .Play(hover);
                
        transform.localScale = origScale * sizeMultipler;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Un-Hover
        transform.localScale = origScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Click
        SoundManager.Instance
            .CreateSoundBuilder()
            .WithPosition(transform.position)
            .WithRandomPitch()
            .Play(click);

    }
}
