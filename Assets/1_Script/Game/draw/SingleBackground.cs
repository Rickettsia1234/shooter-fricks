using UnityEngine;

public class SingleBackground : MonoBehaviour
{
    public Transform target;
    public float tileSizeX = 19.2f;
    public float tileSizeY = 10.8f;

    private void LateUpdate()
    {
        Vector3 targetPos = target.position;
        float newX = Mathf.Floor(targetPos.x / tileSizeX) * tileSizeX;
        float newY = Mathf.Floor(targetPos.y / tileSizeY) * tileSizeY;
        transform.position = new Vector3(newX, newY, transform.position.z);
    }
}