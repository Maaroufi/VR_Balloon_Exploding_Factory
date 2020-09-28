using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClaireAction : MonoBehaviour
{
    public static Animator animatorClairStatic;
    Animator animatorClair;
    public Animator animatorRemy;
    public Animator animatorScreen;

    public Transform targetTransformIntroduce;
    public Transform targetTransformToPlayer1;
    public Transform targetTransformToPlayer2;
    public Transform targetTransformToPlayer3;
    bool isWalkingTowardsConveyor = false;
    bool isWalkingTowardsPlayer1 = false;
    bool isWalkingTowardsPlayer2 = false;
    bool isWalkingTowardsPlayer3 = false;
    public static AudioSource audioSource;
    public AudioClip shoesSound;
    public AudioClip conversation;
    public AudioClip firstMessage;
    public AudioClip BalloonTaskExplanation;
    public AudioClip Calibration;
    public AudioClip LetsStart;

    public static AudioClip performBetter;
    public static AudioClip good;
    public static AudioClip veryGood;
    public static AudioClip wellDone;
    public static AudioClip great;
    public static AudioClip impressed;

    public static AudioClip taskFinished;

    public AudioClip performBetterCopy;
    public AudioClip goodCopy;
    public AudioClip veryGoodCopy;
    public AudioClip wellDoneCopy;
    public AudioClip greatCopy;
    public AudioClip impressedCopy;
    public AudioClip taskFinishedCopy;

    private float timeWait;
    public GameObject ipad;
    public static GameObject ipadStatic;

    private void Start()
    {
        animatorClair = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        animatorClair.SetTrigger("isConversatingGuy");
        audioSource.clip = conversation;
        audioSource.volume = 0.05f;
        audioSource.Play();
        ipad.SetActive(false);

        ipadStatic = ipad;
        animatorClairStatic = animatorClair;

        performBetter = performBetterCopy;
        good = goodCopy;
        veryGood = veryGoodCopy;
        wellDone = wellDoneCopy;
        great = greatCopy;
        impressed = impressedCopy;
        taskFinished = taskFinishedCopy;
    }

    private void Update()
    {
        if (isWalkingTowardsConveyor)
        {
            WalkingToward(targetTransformIntroduce);
        }
        else if (isWalkingTowardsPlayer1)
        {
            WalkingToward(targetTransformToPlayer1);
        }
        else if (isWalkingTowardsPlayer2)
        {
            WalkingToward(targetTransformToPlayer2);
        }
        else if (isWalkingTowardsPlayer3)
        {
            Vector3 targetDir3 = new Vector3(targetTransformToPlayer3.transform.position.x - transform.position.x, 0f,
                                targetTransformToPlayer3.transform.position.z - transform.position.z);
            Quaternion rotate3 = Quaternion.LookRotation(targetDir3);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotate3, 0.05f);
            animatorClair.SetTrigger("isArrivedToPlayer"); 

        }
    }

    void WalkingToward(Transform targetTransform)
    {
        Vector3 targetDir = new Vector3(targetTransform.transform.position.x - transform.position.x, 0f,
                                        targetTransform.transform.position.z - transform.position.z);
        Quaternion rotate = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotate, 0.05f);
        transform.Translate(Vector3.forward * 0.03f);

        if (Vector3.Distance(transform.position, targetTransform.transform.position) < 0.6)
        {
            if (isWalkingTowardsConveyor)
            {        
                animatorClair.SetTrigger("isConversationStopped");
                animatorClair.SetTrigger("isIntroducing");
                isWalkingTowardsConveyor = false;
            }
            else if (isWalkingTowardsPlayer1)
            {
                isWalkingTowardsPlayer1 = false;
                isWalkingTowardsPlayer2 = true;
            }
            else if (isWalkingTowardsPlayer2)
            {
                isWalkingTowardsPlayer3 = true;
                isWalkingTowardsPlayer2 = false;
            }
            audioSource.Stop();
        }
    }

    void TriggerBalloon()
    {
        audioSource.clip = LetsStart;
        audioSource.Play();
        TaskLogic.isBalloonStopped = false;
        EventManager.SpawnTriggered();
        animatorClair.SetTrigger("isEncouraging");
    }

    void StartWalkingConveyor()
    {
        animatorClair.SetTrigger("isWalking");
        audioSource.clip = shoesSound;
        audioSource.volume = 0.1f;
        audioSource.Play();
        isWalkingTowardsConveyor = true;
    }

    void StartWalkingPlayer()
    {
        animatorClair.SetTrigger("isWalkingToPlayer");
        audioSource.clip = shoesSound;
        audioSource.Play();
        isWalkingTowardsPlayer1 = true;
    }

    void StopTalk()
    {
        animatorRemy.SetTrigger("stopTalking");
    }

    void StartWelcome()
    {
        StartCoroutine(StWelcome());       
    }

    void FirstMessage()
    {
        audioSource.clip = firstMessage;
        audioSource.Play();
    }

    void StartInstruction()
    {
        StartCoroutine(StInstruction());
    }

    void StartCalibration()
    {
        StartCoroutine(StCalibration());
    }

    void StartScoring()
    {
        StartCoroutine(StScoring());
    }

    void StartTaskInfo()
    {
        StartCoroutine(StTaskInfo());
    }

    void DisplayIpad()
    {
        ipad.SetActive(true);
    }

    IEnumerator StWelcome()
    {
        //animatorScreen.Play("WelcomeScreen");
        animatorScreen.SetTrigger("isWelcoming");
        yield return new WaitForFixedUpdate();
        timeWait = 50; 
        yield return new WaitForSeconds(timeWait);

        animatorScreen.SetTrigger("isWelcomingClose");
        yield return new WaitForFixedUpdate();
        timeWait = animatorScreen.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(timeWait);

        animatorClair.SetTrigger("isIntroStopped");
        animatorClair.SetTrigger("isInstructing");
    }

    IEnumerator StInstruction()
    {
        animatorScreen.SetTrigger("isInstructionStarted");
        yield return new WaitForFixedUpdate();
        timeWait = 8;
        yield return new WaitForSeconds(timeWait);

        animatorScreen.SetTrigger("isInstructionClose");
        yield return new WaitForFixedUpdate();
        timeWait = animatorScreen.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(timeWait);

        animatorClair.SetTrigger("isInstructingStopped");
        animatorClair.SetTrigger("isShowingTask");
    }

    IEnumerator StTaskInfo()
    {
        audioSource.clip = BalloonTaskExplanation;
        audioSource.Play();
        animatorScreen.SetTrigger("isTaskInfo");
        yield return new WaitForFixedUpdate();
        timeWait = 39;
        //timeWait = animatorScreen.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(timeWait);
        animatorScreen.SetTrigger("isTaskInfoClosed");

        yield return new WaitForFixedUpdate();
        timeWait = 14;
        yield return new WaitForSeconds(timeWait);

        animatorClair.SetTrigger("isShowingTaskStopped");
        animatorClair.SetTrigger("isCalibrating");
    }

    IEnumerator StCalibration()
    {
        audioSource.clip = Calibration;
        audioSource.Play();
        animatorScreen.SetTrigger("isCalibrating");
        yield return new WaitForFixedUpdate();
        timeWait = 28;
        yield return new WaitForSeconds(timeWait);

        // Aperture calibration
        DataToSave.isHandCalibrationStarted = true;
        yield return new WaitForFixedUpdate();
        timeWait = 9;
        yield return new WaitForSeconds(timeWait);
        DataToSave.isHandCalibrationStarted = false;
        DataToSave.isHandCalibrationJustFinished = true;

        yield return new WaitForFixedUpdate();
        timeWait = 9;
        yield return new WaitForSeconds(timeWait);

        yield return new WaitForFixedUpdate();
        timeWait = 10;
        yield return new WaitForSeconds(timeWait);

        // Speed Calibration
        DataToSave.isHandSpeedCalibrationStarted = true;
        yield return new WaitForFixedUpdate();
        timeWait = 10;
        yield return new WaitForSeconds(timeWait);
        DataToSave.isHandSpeedCalibrationStarted = false;
        DataToSave.isHandSpeedCalibrationJustFinished = true;
        animatorScreen.SetTrigger("isCalibratingClose");

        yield return new WaitForFixedUpdate();
        timeWait = 6;
        yield return new WaitForSeconds(timeWait);
        animatorClair.SetTrigger("isCalibratingStopped");
        StartWalkingPlayer();
    }

    IEnumerator StScoring()
    {
        animatorScreen.SetTrigger("isScoringStarted");
        yield return new WaitForFixedUpdate();
        timeWait = animatorScreen.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(timeWait);
    }

    public static IEnumerator TaskIsFinished()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(5f);
        animatorClairStatic.SetTrigger("isEncouragingFinished");
        animatorClairStatic.SetTrigger("isTaskFinished");
        ipadStatic.SetActive(false);
        audioSource.clip = taskFinished;
        audioSource.Play();
        yield return new WaitForSeconds(15f);
        Application.Quit();
    }

    public static void Encourage(int rewards)
    {
        switch (rewards)
        {
            case 5:
                audioSource.clip = impressed;
                audioSource.Play();
                break;
            case 4:
                audioSource.clip = great;
                audioSource.Play();
                break;
            case 3:
                audioSource.clip = wellDone;
                audioSource.Play();
                break;
            case 2:
                audioSource.clip = veryGood;
                audioSource.Play();
                break;
            case 1:
                audioSource.clip = good;
                audioSource.Play();
                break;
            case 0:
                audioSource.clip = performBetter;
                audioSource.Play();
                break;
            default:
                print("Incorrect intelligence level.");
                break;
        }
    }
}
