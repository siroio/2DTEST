using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class CheckGround : MonoBehaviour
{
    private const float DISTANCE_THRESHOLD = 0.02f;

    private RaycastHit2D[] raycastHits = new RaycastHit2D[6];

    [SerializeField]
    private new BoxCollider2D collider2D; // BoxCollider2Dを使用

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private Vector2 size;

    [SerializeField]
    private float distance;

    [SerializeField]
    private float angleLimit;

    [SerializeField]
    private LayerMask layer;

    [SerializeField]
    private bool showGizmos = true;

    [SerializeField]
    private TextMeshProUGUI text;

    private Vector3 castOrigin;
    private Vector3 castDirection;
    private List<Vector3> hitPoints = new List<Vector3>();
    private bool isGrounded = false;

    void Update()
    {
        PerformRaycast();
    }

    private void PerformRaycast()
    {
        if (collider2D == null)
        {
            Debug.LogWarning("BoxCollider2D is not assigned.");
            return;
        }

        castOrigin = transform.position + offset;
        castDirection = -transform.up;

        float colliderThickness = collider2D.bounds.extents.y;
        hitPoints.Clear();

        // Raycastの実行
        var hits = Physics2D.BoxCastNonAlloc(castOrigin, size, 0, castDirection, raycastHits, distance + colliderThickness, layer);

        if (hits > 0)
        {
            float closestDistance = Mathf.Infinity;
            float minAngle = Mathf.Infinity;

            for (int i = 0; i < hits; i++)
            {
                hitPoints.Add(raycastHits[i].point);

                // ヒット箇所からコライダーまでの垂直距離を計算
                Vector2 hitToOrigin = raycastHits[i].point - (Vector2)castOrigin;
                float verticalDistance = Vector2.Dot(hitToOrigin, castDirection.normalized);

                // コライダーの厚みを差し引く
                float adjustedDistance = verticalDistance - colliderThickness;
                float angle = Vector2.Angle(transform.up, raycastHits[i].normal);

                if ((adjustedDistance < 0 && adjustedDistance > closestDistance) || // 負の距離の場合は最大値（0に近いもの）を選択
                    (adjustedDistance >= 0 && adjustedDistance <= closestDistance)) // 正の距離の場合は最小値を選択
                {
                    closestDistance = adjustedDistance;
                    minAngle = angle;
                    Debug.DrawRay(raycastHits[i].point, raycastHits[i].normal * adjustedDistance, Color.red);
                }
            }
            isGrounded = IsGrounded(closestDistance, minAngle);
            text.text = $"Distance: {closestDistance}m\nAngle: {minAngle:F3}°\nLimitAngle:{angleLimit + Mathf.Epsilon:F3}\n";
            text.text += $"angle: {Mathf.Approximately(minAngle, angleLimit) || minAngle < angleLimit}\n";
            text.text += $"dist: {(closestDistance - Physics2D.defaultContactOffset) <= DISTANCE_THRESHOLD}\n";
            text.text += isGrounded ? "IsGrounded" : "";
        }
        else
        {
            text.text = "No ground detected";
        }
    }

    private bool IsGrounded(float distance, float angle)
    {
        return ((distance - Physics2D.defaultContactOffset) <= DISTANCE_THRESHOLD) &&
                (Mathf.Approximately(angle, angleLimit) || angle < angleLimit);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + offset, size);

        Gizmos.color = isGrounded ? Color.green : Color.red;
        foreach (var point in hitPoints)
        {
            Gizmos.DrawSphere(point, 0.05f);
        }
    }
}
