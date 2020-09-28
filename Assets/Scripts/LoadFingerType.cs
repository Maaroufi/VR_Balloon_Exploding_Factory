using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadFingerType : MonoBehaviour
{
    public GameObject indexRight = null;
    public GameObject indexLeft = null;
    private bool firstTimeRight = true;
    private bool firstTimeLeft = true;

    void FixedUpdate()
    {
        if (!indexRight)
        {
            indexRight = GameObject.Find("RightHandAnchor/OVRHandPrefab/Capsules/Hand_Index3_CapsuleRigidBody/Hand_Index3_CapsuleCollider");
        }

        if (indexRight && firstTimeRight)
        {
            indexRight.gameObject.AddComponent<KeyDetector>();
            Collider ColliderIndexProxJointRight = indexRight.GetComponent<Collider>();
            ColliderIndexProxJointRight.isTrigger = true;
            firstTimeRight = false;
        }
        if (!indexLeft)
        {
            indexLeft = GameObject.Find("LeftHandAnchor/OVRHandPrefab/Capsules/Hand_Index3_CapsuleRigidBody/Hand_Index3_CapsuleCollider");
        }

        if (indexLeft && firstTimeLeft)
        {
            indexLeft.gameObject.AddComponent<KeyDetector>();
            Collider ColliderIndexProxJointLeft = indexLeft.GetComponent<Collider>();
            ColliderIndexProxJointLeft.isTrigger = true;
            firstTimeLeft = false;
        }
    }
}
