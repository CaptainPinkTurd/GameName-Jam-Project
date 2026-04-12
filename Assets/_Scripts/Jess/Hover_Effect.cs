using UnityEngine;
using UnityEngine.EventSystems;
using CaptainPinkTurd.AudioSystem;

public class Hover_Effect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public float sizeMultiplier = 1.2f;
    public float speed = 10f;
    public SoundData hoverInSfx;
    public SoundData hoverOutSfx;
    public SoundData click;

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
            .Play(hoverInSfx);
                
        transform.localScale = origScale * sizeMultiplier;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Un-Hover
        SoundManager.Instance
            .CreateSoundBuilder()
            .WithPosition(transform.position)
            .WithRandomPitch(-.1f, .1f)
            .Play(hoverOutSfx);
        
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
