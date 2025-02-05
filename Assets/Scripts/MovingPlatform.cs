using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float speed = 3f;

    private Vector3 targetPosition;

    void Start()
    {
        targetPosition = endPoint.position;
    }

    void Update()
    {
        // 床を移動させる
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // 目標位置に到達したら、目標位置を切り替える
        if (transform.position == targetPosition)
        {
            targetPosition = targetPosition == startPoint.position ? endPoint.position : startPoint.position;
        }
    }

    // オブジェクトが床に乗ったとき
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(transform);  // オブジェクトを床の子にする
        }
    }

    // オブジェクトが床から降りたとき
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(null);  // 子オブジェクトから外す
        }
    }
}
