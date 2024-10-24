using UnityEngine;
 
public class LightFlicker : MonoBehaviour
{
    [SerializeField] private float flickerIntensity = 0.2f;
    [SerializeField] private float flickersPerSecond = 3.0f;
    [SerializeField] private float speedRandomness = 1.0f;
 
    private float _time;
    private float _startingIntensity;
    private Light _light;
 
    void Start()
    {
        _light = GetComponent<Light>();
        _startingIntensity = _light.intensity;
    }
    
    void Update()
    {
        _time += Time.deltaTime * (1 - Random.Range(-speedRandomness, speedRandomness)) * Mathf.PI;
        _light.intensity = _startingIntensity + Mathf.Sin(_time * flickersPerSecond) * flickerIntensity;
    }
}