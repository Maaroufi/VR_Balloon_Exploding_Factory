
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Linq;
using System.Threading;

public class DataToSave : DynamoDbBase
{
    private Table ObiTaskTable;
    private IAmazonDynamoDB _client;
    private string dateTime;
    private static AmazonDynamoDBClient client;
    private int participant_ID;
    private string handedness;
    private List<Document> listOfDocuments = new List<Document>();
    private List<float> dataCalibrationApertureList = new List<float>();
    private List<float> ListSpeedAssessed = new List<float>();
    private List<DataSpeed> dataCalibrationSpeedList = new List<DataSpeed>();
    private List<DataSpeed> dataTrialSpeedList = new List<DataSpeed>();

    private List<float> ListSpeedTrial = new List<float>();
    private List<float> ListApertureTrial = new List<float>();
    public static EQueue<float> queueLastTrials = new EQueue<float>(5);

    OVRSkeleton HandSkeleton;
    public string Hand_WristRoot_Pos, Hand_WristRoot_Orientation,
                        Thumb_Dist, Thumb_Prox,             //Thumb_Tip
                        Index_Dist, Index_Med, Index_Prox, //Index_Tip_Position, Index_Tip
                        Middle_Dist, Middle_Med, Middle_Prox,
                        Ring_Dist, Ring_Med, Ring_Prox,
                         Pinky_Dist, Pinky_Med, Pinky_Prox; //Pinky_Tip,
    public Transform Hand_WristRoot_Trans;
    public Vector3 ListIndexTip, ThumbTipPosition, PinkyTipPosition;  // Index_Tip_Pos,

    public static float difficultyLevel = 1f;
    public float lastDifficultyLevel = 0;
    public static string groupBelong;
    public string handStatus;

    [SerializeField]
    public static float speedApertureCycle, speedApertureAssessed, totalApertureAssessed;
    public float handApertureMaxAssessed, handApertureMinAssessed ;
    public float maxApertureTrigger, minApertureTrigger;
    public static int rewards, score = 0;

    public float timeAperture = 0f;

    public static float performance, performanceReference, performanceBlockBefore = 1f;
    public static float alpha = 0.1f;

    private DataSpeed biggestObject, smallestObject;

    public static float handAperture, lastHandMaxAperture, lastHandMinAperture, lastCycleAperture;

    [SerializeField]
    public static int Number_Cycles = 0;

    public static float relativeTime;
    public float startTime;
    public static int processFirst = 0;
    public static bool isEnteredMaxZone, isEnteredMinZone, isLeftMaxZone, isLeftMinZone = false;

    private bool isBigObjectExist, isSmallObjectExist = false;
    public static bool isNewTrial = false;

    // A effacer, pour debug seulement
    public bool isCabStarted = false;
    public bool isCabFinished = false;
    public bool isSpeedStarted = false;
    public bool isSpeedFinished = false;

    public static bool isBlockStarted = false;
    public static bool isBlockFinished = false;
    public static bool isNewLevel = false;

    public static bool isTrialStarted = false;
    public static bool isTrialFinished = false;

    public static bool isHandCalibrationStarted = false;
    public static bool isHandCalibrationJustFinished = false;

    public static bool isHandSpeedCalibrationStarted = false;
    public static bool isHandSpeedCalibrationJustFinished = false;

    public string nameOfTheTable;

    private int sameFrameFlag = 1;
    private int lastFrameFlag = 0;

    public float nbSegments = 0;

    public static float speedRef, apertureRef;
    public static float speedDiff, apertureDiff;

    public static float sumPerf = 0f;

    private float triggerApperture = 0.25f;
    private float refForSpeed = 0.1f;
    private float refForAperture = 0.5f;
    public static float betaSpeed = 1.2f;
    public static float betaAperture = 0.1f;
    public float betaSpeedInstantiate, betaApertureInstantiate;
    public static float performanceThreshold = 0.99f;
    public float performanceThresholdInstantiate;

    // For multithreading. Only to write on database cloud
    Thread childThread = null;
    AutoResetEvent ChildThreadWait = new AutoResetEvent(false);

    private bool updatedDB = false;

    void OnEnable()
    {
        EventManager.OnTableToFill += LoadAWSTable;
        EventManager.OnDataToLoad += LoadData;
    }

    void OnDisable()
    {
        EventManager.OnTableToFill -= LoadAWSTable;
        EventManager.OnDataToLoad -= LoadData;

        childThread.Abort();
    }

    private void Awake()
    {
        childThread = new Thread(ChildThreadLoop);
        childThread.Start();
    }

    private void Start()
    {
        dateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
        UnityInitializer.AttachToGameObject(this.gameObject);
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
        _client = Client;

        HandSkeleton = this.gameObject.GetComponent<OVRSkeleton>();

        //Get data from PlayerPrefs
        participant_ID = PlayerPrefs.GetInt("ParticipantID");
        handedness = PlayerPrefs.GetString("handedness");

        nameOfTheTable = "GaleaTask_" + participant_ID + "_" + dateTime;

        // Create table on DynamoDB
        //CreateTableListener();
        // Then Load it 10 seconds later (to give time to create the table)
        //Invoke("LoadTableListener", 10.0f);        

        if (participant_ID % 2 == 0)
        {
            groupBelong = "MS";
        }
        else
        {
            groupBelong = "ML";
        }

        betaSpeedInstantiate = betaSpeed;
        betaApertureInstantiate = betaAperture;
        performanceThresholdInstantiate = performanceThreshold;
    }

    void FixedUpdate()
    {
        betaSpeed = betaSpeedInstantiate;
        betaAperture = betaApertureInstantiate;
        performanceThreshold = performanceThresholdInstantiate;

        if (isCabStarted)
        {
            isHandCalibrationStarted = true;
            isCabStarted = false;
        }

        if (isCabFinished)
        {
            isHandCalibrationStarted = false;
            isHandCalibrationJustFinished = true;
            Number_Cycles = 0;
            isCabFinished = false;
        }

        if (isSpeedStarted)
        {
            isHandSpeedCalibrationStarted = true;
            isSpeedStarted = false;
        }

        if (isSpeedFinished)
        {
            isHandSpeedCalibrationStarted = false;
            isHandSpeedCalibrationJustFinished = true;
            Number_Cycles = 0;
            isSpeedFinished = false;
            isTrialStarted = true;
        }

        if (isTrialStarted ||
            isHandCalibrationStarted || isHandCalibrationJustFinished ||
            isHandSpeedCalibrationStarted || isHandSpeedCalibrationJustFinished)
        {
            if (isNewTrial)
            {
                startTime = Time.time;
                isNewTrial = false;
            }

            LoadData();
        }
        else if (isTrialFinished)
        {
            relativeTime = Time.time - startTime;
            performance = AveragePerformance(ListApertureTrial, ListSpeedTrial); 
            queueLastTrials.Enqueue(performance);
            rewards = Rewards(queueLastTrials, performance);
            ClaireAction.Encourage(rewards);
            StartCoroutine(ScoreDisplay.ChangeText());

            if (!updatedDB)
            {
                ChildThreadWait.Set();
                updatedDB = true;
            }

            if (relativeTime < 30)
            {
                int newBonus = (30 - (int)relativeTime) * 50;
                ScoreDisplay.newScore += newBonus;
            }

            ListApertureTrial.Clear();
            ListSpeedTrial.Clear();
            ListSpeedTrial.TrimExcess();
            ListApertureTrial.TrimExcess();
            Number_Cycles = 0;
            processFirst = 0;
            isTrialFinished = false;
        }

        if (isBlockFinished)
        {
            if (isNewLevel)
            {
                difficultyLevel = NewLevelDifficulty();
                isNewLevel = false;
            }
        }

        if (isBlockStarted)
        {
            updatedDB = false;
        }

        if (TaskLogic.isTaskFinished)
        {
            var dataDocument = new Document();
            // ********** Add data to a document *********************
            dataDocument["Timestamp"] = Time.realtimeSinceStartup;
            dataDocument["Is Task complete"] = "Task is complete";
            ObiTaskTable.PutItemAsync(dataDocument, (r) => { });
        }
    }

    private void LoadData()
    {
        var dataDocument = new Document();

        Thumb_Dist = HandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Thumb3].Transform.localEulerAngles.ToString("F6");
        Thumb_Prox = HandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Thumb2].Transform.localEulerAngles.ToString("F6");
        Index_Dist = HandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Index3].Transform.localEulerAngles.ToString("F6");
        Index_Med = HandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Index2].Transform.localEulerAngles.ToString("F6");
        Index_Prox = HandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Index1].Transform.localEulerAngles.ToString("F6");
        Middle_Dist = HandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Middle3].Transform.localEulerAngles.ToString("F6");
        Middle_Med = HandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Middle2].Transform.localEulerAngles.ToString("F6");
        Middle_Prox = HandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Middle1].Transform.localEulerAngles.ToString("F6");
        Ring_Dist = HandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Ring3].Transform.localEulerAngles.ToString("F6");
        Ring_Med = HandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Ring2].Transform.localEulerAngles.ToString("F6");
        Ring_Prox = HandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Ring1].Transform.localEulerAngles.ToString("F6");
        Pinky_Dist = HandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Pinky3].Transform.localEulerAngles.ToString("F6");
        Pinky_Med = HandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Pinky2].Transform.localEulerAngles.ToString("F6");
        Pinky_Prox = HandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Pinky1].Transform.localEulerAngles.ToString("F6");

        Hand_WristRoot_Orientation = HandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_WristRoot].Transform.eulerAngles.ToString("F6");
        Hand_WristRoot_Pos = HandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_WristRoot].Transform.position.ToString("F6");

        Hand_WristRoot_Trans = HandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_WristRoot].Transform;

        ThumbTipPosition = HandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_IndexTip].Transform.position;
        PinkyTipPosition = HandSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Thumb1].Transform.position;

        handAperture = Vector3.Distance(ThumbTipPosition, PinkyTipPosition);

        float timestamp = Time.realtimeSinceStartup;

        if (isTrialStarted)
        {
            relativeTime = Time.time - startTime;

            if (handAperture > maxApertureTrigger)
            {
                handStatus = "Open";
                isEnteredMaxZone = true;
                isEnteredMinZone = false;
                isLeftMaxZone = false;
                isLeftMinZone = false;
            }
            else if (handAperture < minApertureTrigger)
            {
                handStatus = "Close";
                isEnteredMaxZone = false;
                isEnteredMinZone = true;
                isLeftMinZone = false;
                isLeftMaxZone = false;
            }
            else
            {
                if (isEnteredMaxZone)
                {
                    if (processFirst == 0)
                    {
                        processFirst++;
                    }
                    else if (processFirst == 1)
                    {
                        Number_Cycles++;
                        processFirst = 0;
                    }

                    HandPositioning.PlayPumpSound();

                    isEnteredMinZone = false;
                    isLeftMinZone = false;
                    isEnteredMaxZone = false;
                    isLeftMaxZone = true;
                }
                else if (isEnteredMinZone)
                {
                    if (processFirst == 0)
                    {
                        processFirst++;
                    }
                    else if (processFirst == 1)
                    {
                        Number_Cycles++;
                        processFirst = 0;
                    }

                    isLeftMaxZone = false;
                    isEnteredMaxZone = false;
                    isEnteredMinZone = false;
                    isLeftMinZone = true;

                    BalloonAnimatorManager.startPumping = true;
                }
            }

            if (isLeftMaxZone && !isBigObjectExist)
            {
                biggestObject = dataTrialSpeedList.OrderByDescending(item => item.Grasp_Range).First();
                dataTrialSpeedList.Clear();
                dataTrialSpeedList.TrimExcess();
                isBigObjectExist = true;
                isLeftMaxZone = false;
                lastFrameFlag++;
            }
            if (isLeftMinZone && !isSmallObjectExist && (sameFrameFlag == lastFrameFlag) && isBigObjectExist)
            {
                smallestObject = dataTrialSpeedList.OrderByDescending(item => item.Grasp_Range).Last();
                dataTrialSpeedList.Clear();
                dataTrialSpeedList.TrimExcess();
                isSmallObjectExist = true;
                isLeftMinZone = false;
                lastFrameFlag = 0;
                sameFrameFlag = 1;
            }

            if (isSmallObjectExist && isBigObjectExist)
            {
                timeAperture = smallestObject.Timestamp - biggestObject.Timestamp;

                lastHandMaxAperture = biggestObject.Grasp_Range;
                lastHandMinAperture = smallestObject.Grasp_Range;
                lastCycleAperture = lastHandMaxAperture - lastHandMinAperture;
                ListApertureTrial.Add(lastCycleAperture);

                speedApertureCycle = lastCycleAperture / timeAperture;
                ListSpeedTrial.Add(speedApertureCycle);

                if (groupBelong == "ML")
                {
                    if (lastCycleAperture > apertureRef && lastCycleAperture < totalApertureAssessed)
                    {
                        ScoreDisplay.newScore += 10;
                    }
                    else if (lastCycleAperture > totalApertureAssessed)
                    {
                        ScoreDisplay.newScore += 50;
                    }
                }
                else
                {
                    if (speedApertureCycle > speedRef && speedApertureCycle < speedApertureAssessed)
                    {
                        ScoreDisplay.newScore += 10;
                    }
                    else if (speedApertureCycle > speedApertureAssessed)
                    {
                        ScoreDisplay.newScore += 50;
                    }
                }

                isSmallObjectExist = false;
                isBigObjectExist = false;

                biggestObject.Grasp_Range = 0;
                biggestObject.Timestamp = 0;
                smallestObject.Grasp_Range = 0;
                smallestObject.Timestamp = 0;
            }

            else if (isSmallObjectExist && !isBigObjectExist)
            {
                isSmallObjectExist = false;
                isBigObjectExist = false;

                biggestObject.Grasp_Range = 0;
                biggestObject.Timestamp = 0;
                smallestObject.Grasp_Range = 0;
                smallestObject.Timestamp = 0;
            }

            if (isEnteredMaxZone || isEnteredMinZone)
            {
                DataSpeed dataTrialSpeed = new DataSpeed();
                dataTrialSpeed.Timestamp = timestamp;
                dataTrialSpeed.Grasp_Range = handAperture;

                dataTrialSpeedList.Add(dataTrialSpeed);
            }

            // ********** Add data to a document *********************
            dataDocument["Timestamp"] = timestamp;
            dataDocument["Participant_ID"] = participant_ID;
            dataDocument["Handedness"] = handedness;
            dataDocument["Trial_Number"] = TaskLogic.currentBalloon;
            dataDocument["Block_Number"] = TaskLogic.currentBlock;
            dataDocument["RelativeTime"] = relativeTime;
            dataDocument["Grasp_Range"] = handAperture;
            dataDocument["Number_Cycles"] = Number_Cycles;
            dataDocument["Palm_Position"] = Hand_WristRoot_Pos;
            dataDocument["Palm_Orientation"] = Hand_WristRoot_Orientation;
            dataDocument["Difficulty_Level"] = difficultyLevel;
            dataDocument["Score"] = score;
            dataDocument["Volume_Pumped"] = nbSegments;
            dataDocument["Rewards"] = rewards;
            dataDocument["Speed_Performance_Cycle"] = speedApertureCycle;
            dataDocument["Aperture_Performance_Cycle"] = lastCycleAperture;

            dataDocument["Thumb_Prox"] = Thumb_Prox;
            dataDocument["Thumb_Dist"] = Thumb_Dist;

            dataDocument["Index_Prox"] = Index_Prox;
            dataDocument["Index_Med"] = Index_Med;
            dataDocument["Index_Dist"] = Index_Dist;

            dataDocument["Middle_Prox"] = Middle_Prox;
            dataDocument["Middle_Med"] = Middle_Med;
            dataDocument["Middle_Dist"] = Middle_Dist;

            dataDocument["Ring_Prox"] = Ring_Prox;
            dataDocument["Ring_Med"] = Ring_Med;
            dataDocument["Ring_Dist"] = Ring_Dist;

            dataDocument["Pinky_Prox"] = Pinky_Prox;
            dataDocument["Pinky_Med"] = Pinky_Med;
            dataDocument["Pinky_Dist"] = Pinky_Dist;

            listOfDocuments.Add(dataDocument);
        }

        if (isHandCalibrationStarted)
        {
            dataDocument["Trial_Number"] = "Calibration_Aperture";
            dataDocument["Timestamp"] = timestamp;
            dataDocument["Participant_ID"] = participant_ID;
            dataDocument["Handedness"] = handedness;
            dataDocument["RelativeTime"] = relativeTime;
            dataDocument["Grasp_Range"] = handAperture;
            dataDocument["Number_Cycles"] = Number_Cycles;
            dataDocument["Palm_Position"] = Hand_WristRoot_Pos;
            dataDocument["Palm_Orientation"] = Hand_WristRoot_Orientation;
            dataDocument["Difficulty_Level"] = difficultyLevel;
            dataDocument["Score"] = score;
            dataDocument["Volume_Pumped"] = nbSegments;
            dataDocument["Rewards"] = rewards;
            dataDocument["Speed_Performance_Cycle"] = speedApertureCycle;
            dataDocument["Aperture_Performance_Cycle"] = lastCycleAperture;

            dataDocument["Thumb_Prox"] = Thumb_Prox;
            dataDocument["Thumb_Dist"] = Thumb_Dist;

            dataDocument["Index_Prox"] = Index_Prox;
            dataDocument["Index_Med"] = Index_Med;
            dataDocument["Index_Dist"] = Index_Dist;

            dataDocument["Middle_Prox"] = Middle_Prox;
            dataDocument["Middle_Med"] = Middle_Med;
            dataDocument["Middle_Dist"] = Middle_Dist;

            dataDocument["Ring_Prox"] = Ring_Prox;
            dataDocument["Ring_Med"] = Ring_Med;
            dataDocument["Ring_Dist"] = Ring_Dist;

            dataDocument["Pinky_Prox"] = Pinky_Prox;
            dataDocument["Pinky_Med"] = Pinky_Med;
            dataDocument["Pinky_Dist"] = Pinky_Dist;

            listOfDocuments.Add(dataDocument);

            dataCalibrationApertureList.Add(handAperture);
        }

        if (isHandSpeedCalibrationStarted)
        {

            if (handAperture > maxApertureTrigger)
            {
                handStatus = "Open";
                isEnteredMaxZone = true;
                isEnteredMinZone = false;
                isLeftMaxZone = false;
                isLeftMinZone = false;
            }
            else if (handAperture < minApertureTrigger)
            {
                handStatus = "Close";
                isEnteredMaxZone = false;
                isEnteredMinZone = true;
                isLeftMinZone = false;
                isLeftMaxZone = false;
            }
            else
            {
                if (isEnteredMaxZone)
                {
                    if (processFirst == 0)
                    {
                        processFirst++;
                    }
                    else if (processFirst == 1)
                    {
                        Number_Cycles++;
                        processFirst = 0;
                    }

                    isEnteredMinZone = false;
                    isLeftMinZone = false;
                    isEnteredMaxZone = false;
                    isLeftMaxZone = true;
                }
                else if (isEnteredMinZone)
                {
                    if (processFirst == 0)
                    {
                        processFirst++;
                    }
                    else if (processFirst == 1)
                    {
                        Number_Cycles++;
                        processFirst = 0;
                    }

                    isLeftMaxZone = false;
                    isEnteredMaxZone = false;
                    isEnteredMinZone = false;
                    isLeftMinZone = true;
                }
            }

            if (isLeftMaxZone && !isBigObjectExist)
            {
                biggestObject = dataCalibrationSpeedList.OrderByDescending(item => item.Grasp_Range).First();
                dataCalibrationSpeedList.Clear();
                dataCalibrationSpeedList.TrimExcess();
                isBigObjectExist = true;
                isLeftMaxZone = false;
                lastFrameFlag++;
            }
            if (isLeftMinZone && !isSmallObjectExist && (sameFrameFlag == lastFrameFlag) && isBigObjectExist)
            {
                smallestObject = dataCalibrationSpeedList.OrderByDescending(item => item.Grasp_Range).Last();
                dataCalibrationSpeedList.Clear();
                dataCalibrationSpeedList.TrimExcess();
                isSmallObjectExist = true;
                isLeftMinZone = false;
                lastFrameFlag = 0;
                sameFrameFlag = 1;
            }
            if (isSmallObjectExist && isBigObjectExist)
            {
                timeAperture = smallestObject.Timestamp - biggestObject.Timestamp;

                lastHandMaxAperture = biggestObject.Grasp_Range;
                lastHandMinAperture = smallestObject.Grasp_Range;
                lastCycleAperture = lastHandMaxAperture - lastHandMinAperture;

                speedApertureCycle = lastCycleAperture / timeAperture;
                ListSpeedAssessed.Add(speedApertureCycle);
                isSmallObjectExist = false;
                isBigObjectExist = false;

                biggestObject.Grasp_Range = 0;
                biggestObject.Timestamp = 0;
                smallestObject.Grasp_Range = 0;
                smallestObject.Timestamp = 0;
            }
            else if (isSmallObjectExist && !isBigObjectExist)
            {
                isSmallObjectExist = false;
                isBigObjectExist = false;

                biggestObject.Grasp_Range = 0;
                biggestObject.Timestamp = 0;
                smallestObject.Grasp_Range = 0;
                smallestObject.Timestamp = 0;
            }

            if (isEnteredMaxZone || isEnteredMinZone)
            {
                DataSpeed dataCabSpeed = new DataSpeed();
                dataCabSpeed.Timestamp = Time.realtimeSinceStartup; ;
                dataCabSpeed.Grasp_Range = handAperture;

                dataCalibrationSpeedList.Add(dataCabSpeed);
            }

            dataDocument["Trial_Number"] = "Calibration_Speed";
            dataDocument["Timestamp"] = timestamp;
            dataDocument["Participant_ID"] = participant_ID;
            dataDocument["Handedness"] = handedness;
            dataDocument["RelativeTime"] = relativeTime;
            dataDocument["Grasp_Range"] = handAperture;
            dataDocument["Number_Cycles"] = Number_Cycles;
            dataDocument["Palm_Position"] = Hand_WristRoot_Pos;
            dataDocument["Palm_Orientation"] = Hand_WristRoot_Orientation;
            dataDocument["Difficulty_Level"] = difficultyLevel;
            dataDocument["Score"] = score;
            dataDocument["Volume_Pumped"] = nbSegments;
            dataDocument["Rewards"] = rewards;
            dataDocument["Speed_Performance_Cycle"] = speedApertureCycle;
            dataDocument["Aperture_Performance_Cycle"] = lastCycleAperture;

            dataDocument["Thumb_Prox"] = Thumb_Prox;
            dataDocument["Thumb_Dist"] = Thumb_Dist;

            dataDocument["Index_Prox"] = Index_Prox;
            dataDocument["Index_Med"] = Index_Med;
            dataDocument["Index_Dist"] = Index_Dist;

            dataDocument["Middle_Prox"] = Middle_Prox;
            dataDocument["Middle_Med"] = Middle_Med;
            dataDocument["Middle_Dist"] = Middle_Dist;

            dataDocument["Ring_Prox"] = Ring_Prox;
            dataDocument["Ring_Med"] = Ring_Med;
            dataDocument["Ring_Dist"] = Ring_Dist;

            dataDocument["Pinky_Prox"] = Pinky_Prox;
            dataDocument["Pinky_Med"] = Pinky_Med;
            dataDocument["Pinky_Dist"] = Pinky_Dist;

            listOfDocuments.Add(dataDocument);
        }

        if (isHandCalibrationJustFinished)
        {
            handApertureMaxAssessed = dataCalibrationApertureList.Max();
            handApertureMinAssessed = dataCalibrationApertureList.Min();

            totalApertureAssessed = handApertureMaxAssessed - handApertureMinAssessed;
            apertureRef = totalApertureAssessed * refForAperture;

            ChildThreadWait.Set();

            maxApertureTrigger = handApertureMaxAssessed - (totalApertureAssessed * triggerApperture);
            minApertureTrigger = handApertureMinAssessed + (totalApertureAssessed * triggerApperture);

            dataCalibrationApertureList.Clear();
            dataCalibrationApertureList.TrimExcess();
            isHandCalibrationJustFinished = false;
            PanelParameters.updateOncePlease = true;
        }

        if (isHandSpeedCalibrationJustFinished)
        {
            speedApertureAssessed = ListSpeedAssessed.Max();
            ListSpeedAssessed.Clear();
            ListSpeedAssessed.TrimExcess();
            speedRef = speedApertureAssessed * refForSpeed;

            ChildThreadWait.Set();

            dataCalibrationSpeedList.Clear();
            dataCalibrationSpeedList.TrimExcess();
            isHandSpeedCalibrationJustFinished = false;

            isLeftMaxZone = false;
            isEnteredMaxZone = false;
            isEnteredMinZone = false;
            isLeftMinZone = false;
            PanelParameters.updateOncePlease = true;
        }
    }

    void LoadTableListener()
    {
        Table.LoadTableAsync(_client, nameOfTheTable, (loadTableResult) =>
        {
            if (loadTableResult.Exception != null)
            {
                Debug.Log("\n failed to load table");
                LoadTableListener();
            }
            else
            {
                ObiTaskTable = loadTableResult.Result;
                Debug.Log("\n Table loaded");
            }
        });
    }
    
    void ChildThreadLoop()
    {      
        while (true)
        {
            ChildThreadWait.WaitOne();

            DocumentBatchWrite documentBatchWrite = new DocumentBatchWrite(ObiTaskTable);
            foreach (var dataDocument in listOfDocuments)
            {
                documentBatchWrite.AddDocumentToPut(dataDocument);
            }

            documentBatchWrite.ExecuteAsync((result) =>
            {
                if (result.Exception != null)
                {
                    Debug.Log("Error");
                    return;
                }
            });

            listOfDocuments.Clear();
            listOfDocuments.TrimExcess();
        }
    }

    IEnumerator LoadAWSTable()
    {
        DocumentBatchWrite documentBatchWrite = new DocumentBatchWrite(ObiTaskTable);
        foreach (var dataDocument in listOfDocuments)
        {
            documentBatchWrite.AddDocumentToPut(dataDocument);
        }

        documentBatchWrite.ExecuteAsync((result) =>
        {
            if (result.Exception != null)
            {
                Debug.Log("Error");
                return;
            }
        });

        listOfDocuments.Clear();
        listOfDocuments.TrimExcess();
        yield return null;
    }

    IEnumerator LoadAWSTableThread()
    {
        var thread = new System.Threading.Thread(() => {
            DocumentBatchWrite documentBatchWrite = new DocumentBatchWrite(ObiTaskTable);
            foreach (var dataDocument in listOfDocuments)
            {
                documentBatchWrite.AddDocumentToPut(dataDocument);
            }

            documentBatchWrite.ExecuteAsync((result) =>
            {
                if (result.Exception != null)
                {
                    Debug.Log("Error");
                    return;
                }
            });

            listOfDocuments.Clear();
            listOfDocuments.TrimExcess();
        });

        thread.Start();

        while (thread.IsAlive)
        {
            yield return null;
        }
    }

    IEnumerator LoadAWSTableOneByOne()
    {
        var thread = new System.Threading.Thread(() => {
            foreach (var dataDocument in listOfDocuments)
            {
                ObiTaskTable.PutItemAsync(dataDocument, (r) => { });
            }
            listOfDocuments.Clear();
            listOfDocuments.TrimExcess();
        });

        thread.Start();

        while (thread.IsAlive)
        {
            yield return null;
        }
    }

    public static float FillingBalloon()
    {
        float nbSegments = 0;

        speedDiff = speedApertureCycle - speedRef;

        if (groupBelong == "MS")
        {
            // MS group is rewarded by speed but with condition to achieve X% of assessed aperture (max)
            if (lastCycleAperture < apertureRef)
            {
                nbSegments = 0;
            }
            else
            {
                if (speedApertureCycle < speedRef)
                {
                    speedDiff = 0;
                }
                else
                {
                    speedDiff = speedApertureCycle - speedRef;
                }
                nbSegments = ((speedApertureCycle + (betaSpeed * (speedDiff * speedDiff))) / 100f) / difficultyLevel;
            }
        }
        if (groupBelong == "ML")
        {
            Debug.Log("ML Debug - Groupe ML entrer");
            // ML group is rewarded by aperture but with condition to achieve X% of assessed speed (max)
            if (speedApertureCycle < speedRef)
            {
                nbSegments = 0;
                Debug.Log("ML Debug - Meme pas entrer");
            }
            else
            {
                if (lastCycleAperture < apertureRef)
                {
                    apertureDiff = 0;
                    Debug.Log("ML Debug - Aperture diff = 0");
                }
                else
                {
                    apertureDiff = lastCycleAperture - apertureRef;
                    Debug.Log("ML Debug - Aperture diff = " + apertureDiff);
                }
                nbSegments = betaAperture * apertureDiff / difficultyLevel;
                Debug.Log("ML Debug - nbSegments = " + nbSegments);
            }
        }
        return nbSegments * 10f;
    }

    public static int Rewards(EQueue<float> queueLastTrials, float performance)
    {
        int newRewards = 0;

        foreach (var item in queueLastTrials)
        {
            if (performance > item * performanceThreshold)
            {
                newRewards++;
            }
        }

        return newRewards;
    }

    public static float NewLevelDifficulty()
    {
        float newLevel = 0f;

        foreach (var item in queueLastTrials)
        {
            sumPerf += item;
        }

        performanceBlockBefore = sumPerf / 5;

        if (groupBelong == "MS")
        {
            performanceReference = speedRef;
            newLevel = difficultyLevel * (1 + alpha * (performanceBlockBefore - performanceReference));
        }
        else
        {
            performanceReference = apertureRef;
            newLevel = difficultyLevel * (1 + alpha * (performanceBlockBefore - performanceReference));
        }

        return newLevel;
    }

    public static float AveragePerformance(List<float> listAperture, List<float> listSpeed)
    {
        float newPerformance = 0f;
        float totalPastPerformance = 0f;

        if (groupBelong == "MS")
        {
            foreach (var item in listSpeed)
            {
                totalPastPerformance += item;
            }
            newPerformance = totalPastPerformance / listSpeed.Count;
        }
        else if (groupBelong == "ML")
        {
            foreach (var item in listAperture)
            {
                totalPastPerformance += item;
            }
            newPerformance = totalPastPerformance / listAperture.Count;
        }

        return newPerformance;
    }

    public static float CyclePerformance(int nbCycle, float time)
    {
        float cyclePerformance;

        cyclePerformance = time / nbCycle;

        return cyclePerformance;
    }

    void CreateTableListener()
    {
        Debug.Log("\n Creating table");

        var productCatalogTableRequest = new CreateTableRequest
        {
            AttributeDefinitions = new List<AttributeDefinition>()
            {
                new AttributeDefinition
                {
                        AttributeName = "Timestamp",
                        AttributeType = "N"
                }
            },
            KeySchema = new List<KeySchemaElement>
            {
                new KeySchemaElement
                {
                        AttributeName = "Timestamp",
                        KeyType = "HASH"
                }
            },
            ProvisionedThroughput = new ProvisionedThroughput
            {
                ReadCapacityUnits = 3000,
                WriteCapacityUnits = 3000
            },
            TableName = nameOfTheTable
        };

        Client.CreateTableAsync(productCatalogTableRequest, (result) =>
        {
            if (result.Exception != null)
            {
                return;
            }
            var tableDescription = result.Response.TableDescription;
        });
    }
}

[Serializable]
public class DataSpeed
{
    public float Timestamp { get; set; }
    public float Grasp_Range { get; set; }
}

public class EQueue<T> : Queue<T>
{
    public int QSize;

    public EQueue(int size)
    {
        QSize = size;
    }

    public new void Enqueue(T val)
    {
        base.Enqueue(val);
        if (this.Count > QSize)
        {
            this.Dequeue();
        }
    }
}
