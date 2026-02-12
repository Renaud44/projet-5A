using UnityEngine;

public class BindXRHands : MonoBehaviour
{
    public FollowXRHand leftFollower;
    public FollowXRHand rightFollower;

    void Start()
    {
        InvokeRepeating(nameof(FindHands), 0.5f, 1f);
    }

    void FindHands()
    {
        if (leftFollower.targetHand == null)
        {
            var left = GameObject.Find("Left Hand Mesh(Clone)");
            if (left != null)
                leftFollower.targetHand = left.transform;
        }

        if (rightFollower.targetHand == null)
        {
            var right = GameObject.Find("Right Hand Mesh(Clone)");
            if (right != null)
                rightFollower.targetHand = right.transform;
        }
    }
}