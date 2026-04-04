using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ShockEffect : MonoBehaviour
{
    [SerializeField] private float _shockWaveTime = 1.25f;

    private Coroutine _shockWaveCoroutine;
    private Material _material;
    private static int _waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");

    private void Awake()
    {
        _material = GetComponent<SpriteRenderer>().material;
    }

    public void OnInteract(InputAction.CallbackContext context)
{
    if (context.performed)
    {
        CallShockWave(); // If press E, activate the shockwave effect
    }
}

    public void CallShockWave() // Call this if the enemy dies
    {
        if (_shockWaveCoroutine != null)
        {
            StopCoroutine(_shockWaveCoroutine);
        }
        _shockWaveCoroutine = StartCoroutine(ShockWaveAction(-0.1f, 1f));
    }

    private IEnumerator ShockWaveAction(float startPos, float endPos)
    {
        _material.SetFloat(_waveDistanceFromCenter, startPos);

        float elapsedTime = 0f;
        while (elapsedTime < _shockWaveTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedAmount = Mathf.Lerp(startPos, endPos, elapsedTime / _shockWaveTime);
            _material.SetFloat(_waveDistanceFromCenter, lerpedAmount);
            yield return null;
        }
    }
}
