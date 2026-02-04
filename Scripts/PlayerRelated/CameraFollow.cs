using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    public float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    private Transform target;

    public float shakeIntensity = 0.0001f;
    public float decreaseFactor = 10.0f;
    public float shake = 0.0f;

    void Start()
    {
        // Automatically find the player by tag (recommended)
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("CameraFollow: No GameObject found with tag 'Player'");
            }
        }
    }

    void Update()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;

        if (shake > 0)
        {
            targetPosition += (Vector3)Random.insideUnitCircle * shakeIntensity;
            shake -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shake = 0.0f;
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
