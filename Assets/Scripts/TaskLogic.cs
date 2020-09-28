using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TaskLogic : MonoBehaviour
{
    public static bool isBalloonStopped = true;
    static public int currentBlock = 0;
    static public int currentBalloon = 0;
    static public bool updateText = false;
    static public int DestroyedCounter = 0;
    static public bool isBalloonInflating = false;
    static public bool isTaskFinished = false;
    static public bool startCountdown = false;
    private bool isFirstCount = true;
    private AudioSource audioSource;

    public ParticleSystem dustExplosion;

    public GameObject rubberBalloon;
    public GameObject deflatedBalloon;
    public Transform animExplosion;
    private GameObject HandAnchor;

        void OnEnable()
    {
        EventManager.OnStopBalloon += StopBalloonMove;
        EventManager.OnBalloonExploded += ExplodeTheBalloon;
    }

    void OnDisable()
    {
        EventManager.OnStopBalloon -= StopBalloonMove;
        EventManager.OnBalloonExploded -= ExplodeTheBalloon;
    }

    private void Awake()
    {
        dustExplosion.Stop();
    }

    void Start()
    {
        currentBlock = 1;
        audioSource = GetComponent<AudioSource>();

        string handedness = PlayerPrefs.GetString("handedness");
        //string handedness = "Right";

        if (handedness == "Right")
        {
            HandAnchor = GameObject.Find("RightHandAnchor/OVRHandPrefab");
            HandAnchor.gameObject.AddComponent<DataToSave>();
        }
        else
        {
            HandAnchor = GameObject.Find("LeftHandAnchor/OVRHandPrefab");
            HandAnchor.gameObject.AddComponent<DataToSave>();
        }
    }

    private void StopBalloonMove()
    {
        isBalloonStopped = true;
    }

    private void ExplodeTheBalloon(GameObject balloonToExplode)
    {
        if (TaskLogic.currentBalloon % 5 == 0)
        {
            currentBlock++;
            DataToSave.isBlockStarted = false;
            DataToSave.isBlockFinished = true;
            if (!isTaskFinished)
            {
                DataToSave.isNewLevel = true;
            }
        }
        Renderer balloonToExplodeMaterial = balloonToExplode.GetComponent<Renderer>();
        Renderer rubberBalloonMaterial = rubberBalloon.GetComponent<Renderer>();
        rubberBalloonMaterial.material = balloonToExplodeMaterial.material;
        StartCoroutine(ShowAndHide(rubberBalloon, 0.12f));
        audioSource.Play();
        dustExplosion.Play();
        GameObject deflatedBalloonExplosion = Instantiate(deflatedBalloon, transform);
        deflatedBalloonExplosion.transform.position = animExplosion.position;
        deflatedBalloonExplosion.gameObject.AddComponent<Rigidbody>();
        deflatedBalloonExplosion.gameObject.AddComponent<MeshCollider>();
        deflatedBalloonExplosion.gameObject.AddComponent<Destroyer>();
        Renderer deflatedBalloonExplosionMaterial = deflatedBalloonExplosion.GetComponent<Renderer>();
        deflatedBalloonExplosionMaterial.material = balloonToExplodeMaterial.material;
        MeshCollider deflatedBalloonCollider = deflatedBalloonExplosion.GetComponent<MeshCollider>();
        deflatedBalloonCollider.convex = true;
        deflatedBalloon.tag = "Balloon";
        Destroy(balloonToExplode);
        isBalloonStopped = false;
        DestroyedCounter++;
        DataToSave.isTrialStarted = false;
        DataToSave.isTrialFinished = true;
        updateText = true;
        if (isTaskFinished)
        {
            StartCoroutine(ClaireAction.TaskIsFinished());
        }
    }

    IEnumerator ShowAndHide(GameObject go, float delay)
    {
        go.SetActive(true);
        yield return new WaitForSeconds(delay);
        go.SetActive(false);
    }
}
