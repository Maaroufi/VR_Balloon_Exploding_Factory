using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemyAction : MonoBehaviour
{
    Animator anim;
    public Transform firstTargetTransform;
    public Transform secondTargetTransform;
    bool isWalkingTowardsFirst = false;
    bool isWalkingTowardsSecond = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isWalkingTowardsFirst)
        {
            Vector3 firstTargetDir = new Vector3(firstTargetTransform.transform.position.x - transform.position.x, 0f,
                                firstTargetTransform.transform.position.z - transform.position.z);
            Quaternion rotateFirst = Quaternion.LookRotation(firstTargetDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotateFirst, 0.05f);
            transform.Translate(Vector3.forward * 0.03f);

            if (Vector3.Distance(transform.position, firstTargetTransform.transform.position) < 0.6)
            {
                anim.SetTrigger("isTurning");
                anim.SetBool("isSecond", true);
                isWalkingTowardsFirst = false;
                anim.SetTrigger("isWalking");
            }
        }

        if (isWalkingTowardsSecond)
        {
            Vector3 secondTargetDir = new Vector3(secondTargetTransform.transform.position.x - transform.position.x, 0f,
                                secondTargetTransform.transform.position.z - transform.position.z);
            Quaternion rotateSecond = Quaternion.LookRotation(secondTargetDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotateSecond, 0.05f);
            transform.Translate(Vector3.forward * 0.03f);

            if (Vector3.Distance(transform.position, secondTargetTransform.transform.position) < 0.6)
            {
                isWalkingTowardsSecond = false;
                Destroy(gameObject);
            }
        }
    }

    void StartWalking()
    {
        if (!anim.GetBool("isSecond"))
        {
            isWalkingTowardsFirst = true;
        }
        if (anim.GetBool("isSecond"))
        {
            isWalkingTowardsSecond = true;
        }
    }
}
