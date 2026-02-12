using UnityEngine;

public class FollowXRHand : MonoBehaviour
{
    public Transform targetHand;

    void Update()
    {
        if (targetHand == null)
            return;

        transform.position = targetHand.position;
        transform.rotation = targetHand.rotation;
    }
}
