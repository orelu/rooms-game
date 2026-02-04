using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItemProporties : MonoBehaviour
{
    public float squareSize = 1f;
    public float angularSpeed = 45f;

    private LineRenderer square1;
    private LineRenderer square2;

    private float angleOffset1 = 15f;
    private float angleOffset2 = 0f;

    public LayerMask players;

    public Item item;

    public bool isXPOrb = false;
    public int xp = 0;
    public Sprite xpOrb;

    void Start()
    {
        square1 = CreateSquare("Square1");
        square2 = CreateSquare("Square2");
    }

    void UpdateLineColor(LineRenderer lr, float xp)
    {
        // Apply your equation
        float t = -1f / (0.25f * xp + 1f) + 1f;
        float t2 = -1f / (0.5f * xp + 1f) + 1f;

        // Clamp just in case due to floating-point errors
        t = Mathf.Clamp01(t);
        t2 = Mathf.Clamp01(t);

        lr.startColor = Color.Lerp(Color.white, Color.green, t);
        lr.endColor =Color.Lerp(Color.white, Color.green, t2);
    }


    LineRenderer CreateSquare(string name)
    {
        GameObject squareObj = new GameObject(name);
        squareObj.transform.parent = transform;  // Still parented for organization
        squareObj.transform.localPosition = Vector3.zero;

        LineRenderer lr = squareObj.AddComponent<LineRenderer>();
        lr.positionCount = 5;
        lr.loop = false;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        UpdateLineColor(lr, xp);


        return lr;
    }

    void Update()
    {
        angleOffset1 += angularSpeed * Time.deltaTime;
        angleOffset2 -= angularSpeed * Time.deltaTime;

        DrawSquare(square1, angleOffset1);
        DrawSquare(square2, angleOffset2);

        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(transform.position, 0.5f, players);
        foreach (Collider2D player in hitPlayer) {
            Debug.Log(xp);
            if (isXPOrb) {
                PlayerStats.instance.gainExperience(xp);
            } else {
                Inventory.instance.Add(item);
            }
            Destroy(gameObject);
        }
    }

    void DrawSquare(LineRenderer lr, float rotation)
    {
        float halfSize = squareSize / 2f;
        Vector3[] corners = new Vector3[5];

        // Define unrotated corners centered at origin
        Vector3[] baseCorners = new Vector3[]
        {
            new Vector3(halfSize, halfSize, 0),
            new Vector3(halfSize, -halfSize, 0),
            new Vector3(-halfSize, -halfSize, 0),
            new Vector3(-halfSize, halfSize, 0),
        };

        // Apply rotation around Z and offset by the object's world position
        for (int i = 0; i < 4; i++)
        {
            corners[i] = transform.position + RotatePoint(baseCorners[i], rotation);
        }
        corners[4] = corners[0]; // Close the square

        lr.SetPositions(corners);
    }

    Vector3 RotatePoint(Vector3 point, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector3(
            point.x * cos - point.y * sin,
            point.x * sin + point.y * cos,
            0
        );
    }
}
