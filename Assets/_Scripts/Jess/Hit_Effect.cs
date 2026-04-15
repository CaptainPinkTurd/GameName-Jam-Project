using UnityEngine;
using DG.Tweening;

public class Hit_Effect : MonoBehaviour
{
    [SerializeField] private float _duration = 0.25f;

    private int _hitEffectAmount = Shader.PropertyToID("_HitEffectAmount");
    private SpriteRenderer[] _spriteRenderers;
    private Material[] _materials;
    private float _lerpAmount;

    private void Awake()
    {
        // Grab all SpriteRenderers from children (red / blue character)
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        _materials = new Material[_spriteRenderers.Length];
        for (int i = 0; i < _materials.Length; i++)
        {
            _materials[i] = _spriteRenderers[i].material;
        }
    }

    public void PlayHitEffect()
    {
        _lerpAmount = 0f;
        DOTween.To(GetLerpValue, SetLerpValue, 1f, _duration)
            .SetEase(Ease.OutExpo)
            .OnUpdate(OnLerpUpdate)
            .OnComplete(OnLerpComplete);
    }

    private float GetLerpValue()
    {
        return _lerpAmount;
    }

    private void SetLerpValue(float newValue)
    {
        _lerpAmount = newValue;
    }

    private void OnLerpUpdate()
    {
        // Apply effect to all child materials
        for (int i = 0; i < _materials.Length; i++)
        {
            _materials[i].SetFloat(_hitEffectAmount, GetLerpValue());
        }
    }

    private void OnLerpComplete()
    {
        // Fade back down
        DOTween.To(GetLerpValue, SetLerpValue, 0f, _duration)
            .OnUpdate(OnLerpUpdate);
    }
}
