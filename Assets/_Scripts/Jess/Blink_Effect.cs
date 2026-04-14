using UnityEngine;
using System.Collections;

public class Blink_Effect : MonoBehaviour
{
    public float speed = 2f;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float alpha = Mathf.Abs(Mathf.Sin(Time.time * speed));
        Color c = sr.color;
        c.a = alpha;
        sr.color = c;
    }
}