using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightBreathing : MonoBehaviour
{
    public float intensityStart = 2f;
    public float intensityEnd = 7.5f;
    public float speed = 1f;

    private Light2D _light;
    private float _timer;

    private void Awake()
    {
        _light = GetComponent<Light2D>();
    }

    private void Update()
    {
        if (!_light) return;
        _timer += Time.deltaTime * speed;
        var t = (Mathf.Sin(_timer) + 1f) / 2f;
        _light.intensity = Mathf.Lerp(intensityStart, intensityEnd, t);
    }

    public void SetColor(Color color)
    {
        if (_light == null) return;
        _light.color = color;
    }

    public Color GetColor() => _light != null ? _light.color : Color.white;
}