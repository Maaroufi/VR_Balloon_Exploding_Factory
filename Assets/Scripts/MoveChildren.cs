using UnityEngine;

public class MoveChildren : MonoBehaviour
{
    public Vector3 movementPerSecond;

    void FixedUpdate()
    {
        if (!TaskLogic.isBalloonStopped)
        {
            foreach (Transform child in transform)
            {
                child.Translate(movementPerSecond * Time.deltaTime, Space.World);
            }
        }
    }
}
