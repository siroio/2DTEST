using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CheckGround : MonoBehaviour
{
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

    void Update()
    {
        PerformRaycast();
    }

    /// <summary>
    /// Raycastを実行し、ヒットしたポイントを記録するメソッド
    /// </summary>
    private void PerformRaycast()
    {
        if (collider2D == null)
        {
            Debug.LogWarning("BoxCollider2D is not assigned.");
            return;
        }

        // レイキャストの起点と方向
        castOrigin = transform.position + offset;
        castDirection = -transform.up;

        // コライダーの厚みを計算
        float colliderThickness = collider2D.bounds.extents.y;

        // ヒット情報をクリア
        hitPoints.Clear();

        // レイキャストを実行
        var hits = Physics2D.BoxCastNonAlloc(castOrigin, size, 0, castDirection, raycastHits, distance + colliderThickness, layer);

        if (hits > 0)
        {
            float closestDistance = Mathf.Infinity;
            float contactAngle = 0f;

            for (int i = 0; i < hits; i++)
            {
                // ヒット箇所を記録
                hitPoints.Add(raycastHits[i].point);

                // 符号付き距離を計算
                var hitToPlayer = transform.position - new Vector3(raycastHits[i].point.x, raycastHits[i].point.y, 0.0f);
                float dist = Vector2.Dot(hitToPlayer, raycastHits[i].normal);
                float adjustedDistance = dist - colliderThickness; // コライダーの厚みを引く
                float angle = Vector2.Angle(transform.up, raycastHits[i].normal);

                if (adjustedDistance < closestDistance && angle < angleLimit)
                {
                    closestDistance = adjustedDistance;
                    contactAngle = angle;
                }
            }

            // テキストに距離と角度を表示
            text.text = $"Distance: {closestDistance:F2}m\nAngle: {contactAngle:F2}° \n";
            text.text += closestDistance <= 0.02f && closestDistance >= Mathf.Epsilon && contactAngle < angleLimit ? "IsGrounded" : "";
        }
        else
        {
            // ヒットしなかった場合は空欄にする
            text.text = "No ground detected";
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        // レイキャストの範囲を青色で表示
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + offset, size);

        // レイキャストの方向を緑色で表示
        Gizmos.color = Color.green;
        Gizmos.DrawRay(castOrigin, castDirection * distance);

        // ヒット箇所を赤色で表示
        Gizmos.color = Color.red;
        foreach (var point in hitPoints)
        {
            Gizmos.DrawSphere(point, 0.05f);
        }
    }
}
