using UnityEngine;

public class FoliageSway : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] private float swaySpeed = 1f;
    [SerializeField] private float swayAmount = 0.5f;
    [SerializeField] private bool swayX = true;
    [SerializeField] private bool swayZ = true;
    
    [Header("Variation")]
    [SerializeField] private float randomOffset = 1f;

    private Vector3 startRotation;
    private float timeOffset;

    private void Awake()
    {
        startRotation = transform.localEulerAngles;
        timeOffset = Random.Range(0f, randomOffset);
    }

    private void Update()
    {
        float time = (Time.time + timeOffset) * swaySpeed;
        
        float swayX = this.swayX ? Mathf.Sin(time) * swayAmount : 0f;
        float swayZ = this.swayZ ? Mathf.Cos(time * 0.8f) * swayAmount : 0f;
        
        Vector3 targetRotation = startRotation + new Vector3(swayX, 0f, swayZ);
        transform.localEulerAngles = targetRotation;
    }
}
