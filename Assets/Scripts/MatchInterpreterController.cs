using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MatchInterpreterController : MonoBehaviour {
    public JSONReader jsonReader; // Reference to the JSONReader script
    public Camera mainCamera; // Reference to the main camera
    private Transform rightController; // Reference to the right controller
    private Transform leftController; // Reference to the right controller
    private LineRenderer laserLineRenderer; // Reference to the LineRenderer component
    private bool isOnField = true;
    public Timeline timeline;

    public bool newVisualization = true;
    public RenderTexture CurrentRenderTexture;
    public RenderTexture NextRenderTexture;
    private bool isSlowMotion = false;
    private bool isPaused = true;
    int currentFrame = 0;

    private GameObject oculusInteractionSampleRig;
    private GameObject centerEye;
    private UserMovementLogger uML;
    private UserInputLogger uIL;
    private float timeSinceLastLog = 0f; // Variável para acumular o tempo
    private float timeSinceLineInteraction = 0f; // Variável para acumular o tempo

    private LineRenderer lastLinerenderer = null;
    private bool setCameraOnHead = false;
    GameObject referencia = null;
    string referenciaName = "";

    public GameObject tutorial;
    private bool isTutorialActive = false;
    private List<LineRenderer> lrList;
    OVRPlayerController playerController;

    private int currentTest1 = 0;
    private int totalTests1 = 10;

    private bool allMessagesHasBeenShown = false;
    private bool isShowingTestMessages = false;

    //private bool actualPlayingTest = false;
    private int actualPlayingTest = 0;
    private int totalPlayingTest = 2;

    private bool testSetup = false;
    private bool testStarted = false;
    private bool testFinished = false;

    private bool test2isMoving = false;

    GameObject cameraRig;

    private GameObject testMessageObject;


    private int[] frameNumbers = {
       2250, 1123, 640, 9336, 9066, 7490, 7038, 6879, 4492, 3589, 790
    };

    private string[] playerObjects = {
       "playerObject 14013", "playerObject 6609", "playerObject 10046", "playerObject 14013","playerObject 12011",
        "playerObject 3120", "playerObject 5124", "playerObject 16126", "playerObject 5124",  "playerObject 3120"
    };


    void Start() {
        rightController = GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor").transform;
        leftController = GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor").transform;
        // Create a new GameObject for the laser line
        GameObject laserLineObject = new("LaserLine");
        // Add a LineRenderer component to the new GameObject
        laserLineRenderer = laserLineObject.AddComponent<LineRenderer>();
        // Configure the LineRenderer
        laserLineRenderer.startWidth = 0.01f; // Set the start width of the line
        laserLineRenderer.endWidth = 0.01f; // Set the end width of the line
        laserLineRenderer.material = new Material(Shader.Find("Unlit/Color")) { color = Color.red };
        laserLineRenderer.positionCount = 2; // Set the number of positions in the line
        oculusInteractionSampleRig = GameObject.Find("OculusInteractionSampleRig");
        lrList = jsonReader.GetLineRendererFromPlayerList();
        playerController = oculusInteractionSampleRig.GetComponent<OVRPlayerController>();
        cameraRig = GameObject.Find("OVRCameraRig");
        centerEye = GameObject.Find("CenterEyeAnchor");
        uML = new();
        uIL = new();
    }

    void setTutorialonRightHand() {
        tutorial.transform.position = rightController.transform.position;
        tutorial.transform.LookAt(oculusInteractionSampleRig.transform);
        tutorial.transform.Rotate(90, -90, 0);
    }

    void PauseUnpause() {
        if (!isPaused) {
            uIL.LogCustomLine("Pausado");
        } else
            uIL.LogCustomLine("DesPausado");

        jsonReader.TogglePlay();
        isPaused = !isPaused;
    }

    void Update() {
        currentFrame = jsonReader.GetFrameIndex();

        timeSinceLastLog += Time.deltaTime;
        timeSinceLineInteraction += Time.deltaTime;

        if (actualPlayingTest != 0) {
            if (actualPlayingTest == 1) {
                if (testSetup == false) {
                    uIL.LogCustomLine($"Teste {currentTest1} do {actualPlayingTest} Iniciado");
                    uML.LogCustomLine($"Teste {currentTest1} do {actualPlayingTest} Iniciado");
                    jsonReader.SetFrameIndex(frameNumbers[currentTest1 - 1] - 155);
                    testSetup = true;
                    referencia = GameObject.Find(playerObjects[currentTest1 - 1]).GetComponentInChildren<Camera>().gameObject;
                    referenciaName = GameObject.Find(playerObjects[currentTest1 - 1]).gameObject.name;
                    setCameraOnHead = true;
                    if (!isPaused)
                        PauseUnpause();
                    SetCameraToPlayer(playerController, referencia);
                    ShowTestMessage($"Teste {currentTest1} de {totalTests1} Iniciado\n" +
                                    $"Atenção\n" +
                                    "\n--Pressione A--");
                }
                if (testSetup && currentFrame < frameNumbers[currentTest1 - 1] - 150) {
                    SetCameraToPlayer(playerController, referencia);
                    testStarted = true;
                }
                if (currentFrame >= (frameNumbers[currentTest1 - 1] - 3) && currentFrame <= (frameNumbers[currentTest1 - 1] + 3) && !testFinished) {
                    if (!isPaused) {
                        PauseUnpause();
                        HideTestMessage();
                    }
                    ShowTestMessages();

                    currentTest1Message += 1;
                    isShowingTestMessages = true;
                    if (allMessagesHasBeenShown && isPaused) {
                        PauseUnpause();
                    }
                    testFinished = true;
                }
                if (testStarted && currentFrame >= (frameNumbers[currentTest1 - 1] + 200)) {
                    NextTest();
                }
            } if (actualPlayingTest == 2) {
                if (testSetup == false) {
                    jsonReader.SetFrameIndex(590);
                    referencia = GameObject.Find("playerObject 5124").GetComponentInChildren<Camera>().gameObject;
                    PauseUnpause();
                    HideTestMessage();
                    ShowTestMessage("Teste 2 Iniciado\nPressione A para continuar...");
                    setCameraOnHead = true;
                    testSetup = true;
                    isSlowMotion = true;

                } else if (currentFrame >= 966 && testStarted == false) { // >=966 pq as vezes assincronismo acontece
                    
                    if (!isPaused) { 
                        PauseUnpause();
                        HideTestMessage();
                    }
                    isShowingTestMessages = true;
                    ShowTestMessages();
                    testStarted = true; 

                }if (OVRInput.GetDown(OVRInput.Button.One) && test2isMoving == false) {
                    if (!isPaused)
                        PauseUnpause();

                    if (testStarted == true) {
                        referencia.SetActive(false);
                        setCameraOnHead = false;
                        uIL.LogCustomLine("Tempo de Análise Concluído");
                        test2isMoving = true;
                        isSlowMotion = false;
                    }
                } else if (jsonReader.GetFrameIndex() >= 1100 && testFinished == false) {

                    testFinished = true;
                    test2isMoving = false;
                    if (!isPaused) {
                        PauseUnpause();
                        ShowTestMessage("Teste 2 Concluído\nPressione A para ver o prosseguimento da jogada...");
                    }
                    uIL.LogCustomLine("------- Teste 2 Finalizado -------");
                    ResetBooleans();
                    referencia.SetActive(true);
                    
                }
            }
        }


        if (OVRInput.GetDown(OVRInput.Button.Two) && !OVRInput.GetDown(OVRInput.Button.Four)) {
            uIL.LogCustomLine($"Tutorial {isTutorialActive}");
            isTutorialActive = !isTutorialActive;
            tutorial.SetActive(isTutorialActive);
        }

        // Verifica se um segundo se passou
        if (timeSinceLastLog >= .5f) {
            uML.LogMovement(centerEye.transform);
            timeSinceLastLog = 0f; // Reseta o acumulador de tempo
        }

        if (playerController.enabled is false && setCameraOnHead == true) {
            SetCameraToPlayer(playerController, referencia);
        }

        if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y > 0.5f) {
            timeline.SetSliderValue(timeline.GetSliderValue() + (int)Math.Round(25 * OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y));
            uIL.LogCustomLine("Tempo Avançado");
        }
        if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y < -0.5f) {
            timeline.SetSliderValue(timeline.GetSliderValue() + (int)Math.Round(25 * OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y));
            uIL.LogCustomLine("Tempo Retrocedido");
        }

        if (OVRInput.GetDown(OVRInput.Button.One)) { // Pausa ou despausa a cena
            if (actualPlayingTest != 0 && !allMessagesHasBeenShown && isShowingTestMessages) {
                HideTestMessage();
                ShowTestMessages();
                currentTest1Message += 1;
            } else {
                PauseUnpause();
            }
        }

        setTutorialonRightHand();

        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger)) {
            if (timeSinceLineInteraction >= .175f) {
                Ray ray = new(leftController.position, leftController.forward);
                laserLineRenderer.SetPosition(0, leftController.position); // Set the start position of the laser line
                if (Physics.Raycast(ray, out RaycastHit hit)) {
                    Transform parent = hit.transform.parent;
                    laserLineRenderer.SetPosition(1, hit.point); // Set the end position of the laser line to the hit point

                    if (actualPlayingTest == 0) {
                        jsonReader.SetPlayerHeatMap(parent.gameObject, true);
                        referencia = parent.GetComponentInChildren<Camera>().gameObject;
                        referenciaName = parent.gameObject.name;
                    }
                    if (parent != null) {
                        Tracer tracer = parent.GetComponentInChildren<Tracer>();
                        if (tracer != null) {
                            LineRenderer lineRenderer = tracer.GetComponent<LineRenderer>();
                            if (lineRenderer != null) {
                                if (lastLinerenderer is null) {
                                    lastLinerenderer = lineRenderer;
                                }
                                if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger)) { //Ativar 1 só
                                    foreach (LineRenderer lr in lrList) {
                                        lr.enabled = false;
                                        lr.positionCount = 0;
                                    }
                                    lastLinerenderer.enabled = false;
                                    lineRenderer.enabled = !lineRenderer.enabled;
                                    jsonReader.PoupulatePathRender(parent.gameObject, currentFrame);
                                } else {
                                    lineRenderer.enabled = !lineRenderer.enabled;
                                    if (lineRenderer.enabled != false)
                                        jsonReader.PoupulatePathRender(parent.gameObject, currentFrame);
                                }
                                lastLinerenderer = lineRenderer;

                                if (!setCameraOnHead) {
                                    uIL.LogUserInput($"Path do Jogador {referenciaName} Ativado");
                                }
                            }
                        }

                        if (parent.GetComponentInChildren<Camera>().gameObject != null && hit.transform.parent.name is not "BallPrefab" && hit.transform.parent.name is not "Stadium") {
                            if (OVRInput.GetDown(OVRInput.Button.Four)) {
                                setCameraOnHead = true;
                                timeSinceLineInteraction = 0f; // Reseta o acumulador de tempo
                                SetCameraToPlayer(playerController, referencia);
                            }
                        }
                    }

                    Tracer balllineRenderer = parent.GetComponentInChildren<Tracer>();
                    if (balllineRenderer != null) {
                        balllineRenderer.enabled = !balllineRenderer.enabled;
                        uIL.LogUserInput("Path da Bola Ativado");

                    }

                } else {
                    laserLineRenderer.SetPosition(1, ray.GetPoint(100)); // Set the end position of the laser line to a point far away
                    jsonReader.SetPlayerHeatMap(null, true);
                }
                laserLineRenderer.enabled = true; // Show the laser line
            }
        } else {
            laserLineRenderer.enabled = false; // Hide the laser line
        }

        if (OVRInput.GetDown(OVRInput.Button.Three)) {
            if (isSlowMotion) {
                Time.timeScale = 1f; // restore normal speed
                uIL.LogUserInput("Slow Motion Off");

                isSlowMotion = false;
            } else {
                Time.timeScale = 0.33f; // slow down to 50% speed
                uIL.LogUserInput("Slow Motion On");

                isSlowMotion = true;
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.Four)) { // desativa todos os tracers
            uIL.LogUserInput("Paths Desativados por Comando");
            foreach (LineRenderer lr in lrList) {
                lr.enabled = false;
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.Start)) { // teleport para arquibancada / campo
            if (playerController.enabled == false) {
                playerController.enabled = true;
            }
            Transform parent = cameraRig.transform.parent;
            isOnField = !isOnField;
            parent.GetComponent<OVRPlayerController>().enabled = false;
            CharacterController cc = parent.GetComponent<CharacterController>();
            cc.enabled = false;
            if (isOnField) {
                uIL.LogUserInput("TP Campo");
                cameraRig.transform.parent.position = new Vector3(0, 1, 0);
            } else {
                uIL.LogUserInput("TP Arquibancada");
                cameraRig.transform.parent.position = new Vector3(0, 17, -53);
            }
            cc.enabled = true;
            parent.GetComponent<OVRPlayerController>().enabled = true;
        }

        if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) &&
            OVRInput.GetDown(OVRInput.Button.Two) && OVRInput.GetDown(OVRInput.Button.Four)) {
            actualPlayingTest = 2;
            NextTest();
        }



        if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) &&
            OVRInput.GetDown(OVRInput.Button.Two) && OVRInput.GetDown(OVRInput.Button.Four)) {
            actualPlayingTest = 1;
            NextTest();
        }

        if (jsonReader.isPlaying) {
            jsonReader.InstantiatePlayersFromFrame(jsonReader.currentFrameIndex);
        }
        jsonReader.UpdateShirtNumberRotation(oculusInteractionSampleRig.transform);
    }

    void SetCameraToPlayer(OVRPlayerController playerController, GameObject referencia) {
        if (referencia is not null) {
            oculusInteractionSampleRig.transform.position = referencia.transform.position;

            playerController.enabled = false;
            if (OVRInput.Get(OVRInput.Button.Start)) {
                uIL.LogUserInput($"Vendo o Ponto de Vista do Jogador {referenciaName}");

                uIL.LogUserInput("Saiu da Visão do Jogador");
                playerController.enabled = true;
                setCameraOnHead = false;
                return;
            } else {
                Quaternion originalRotation = referencia.transform.rotation;
                Quaternion adjustedRotation = new(0f, originalRotation.y, 0f, originalRotation.w);
                oculusInteractionSampleRig.transform.rotation = adjustedRotation;
                if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x > 0.5f) {
                    // Rotate the Camera Rig to the right
                    oculusInteractionSampleRig.transform.Rotate(0, 10, 0);
                }
                if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x < -0.5f) {
                    // Rotate the Camera Rig to the left
                    oculusInteractionSampleRig.transform.Rotate(0, -10, 0);
                }
            }
        }
    }

    void NextTest() {
        if (actualPlayingTest == 1) {
            uML.LogCustomLine($"Teste Tomada Decisão {currentTest1} Finalizado");
            uIL.LogCustomLine($"Teste Tomada Decisão {currentTest1} Finalizado");
            currentTest1 += 1;

            ResetBooleans();
            if (currentTest1 >= totalTests1) {
                ResetTest();
            }
        } else if (actualPlayingTest == 2) {
            uML.LogCustomLine($"Teste {actualPlayingTest} Finalizado");
            uIL.LogCustomLine($"Teste {actualPlayingTest} Finalizado");
        }
    }

    void ResetBooleans() {
        testStarted = false;
        testSetup = false;
        testFinished = false;

        allMessagesHasBeenShown = false;
        currentTest1Message = 0;

        test2isMoving = false;
    }

    void ResetTest() {
        setCameraOnHead = false;
        playerController.enabled = true;
        ResetBooleans();
        actualPlayingTest = 0;
        currentTest1 = 0;
        uML.LogCustomLine("Todos os Testes Finalizados");
        uIL.LogCustomLine("Todos os Testes Finalizados");
        uML.CreateNewTestLogFile();
        uIL.CreateNewTestLogFile();
    }
    void ShowTestMessage(string message) {
        // Cria um novo GameObject para a mensagem de teste
        testMessageObject = new GameObject("TestMessage");
        testMessageObject.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
        // Adiciona um componente TextMesh ao GameObject
        TextMesh textMesh = testMessageObject.AddComponent<TextMesh>();
        textMesh.text = message;
        textMesh.fontSize = 200;
        textMesh.color = Color.white;

        // Define a posição do texto na frente do oculusInteractionSampleRig
        testMessageObject.transform.position = oculusInteractionSampleRig.transform.position + oculusInteractionSampleRig.transform.forward * 10.0f;
        testMessageObject.transform.rotation = Quaternion.LookRotation(cameraRig.transform.forward);
    }

    private string[][] passDecisionTestMessages = new string[][]
    {
        new string[]
        {
            "Qual é o seu Time",
            "Quem tocou a bola para você",
            "O que opções de jogada você tem",
            "Qual seria sua escolha nesse lance?\n(Passe,Drible,Chute...)"
        },
        new string[]
        {
            "A partir de agora você assumirá o controle desse jogador",
            "Ele é um (posição) do time (azul/vermelho)",
            "Mova-se e posicione-se como se fosse você jogando"
        }
    };

    private int currentTest1Message = 0;

    void ShowTestMessages() {
        if (currentTest1Message == passDecisionTestMessages.Length) { 
            allMessagesHasBeenShown = true;
            isShowingTestMessages = false;
            PauseUnpause();
        }
        else {
            if (actualPlayingTest == 1) { 
                ShowTestMessage($"Teste {currentTest1} de {totalTests1}\n" +
                $"Responda em Voz Alta\n" +
                $"{passDecisionTestMessages[0][currentTest1Message]}" +
                $"\n-- Pressione A para continuar --");
                allMessagesHasBeenShown = false;
            }
            else if(actualPlayingTest == 2) {
                ShowTestMessage($"Teste 2: Movimentação\n" +
               $"{passDecisionTestMessages[1][currentTest1Message]}" +
               $"\n-- Pressione A para continuar --");
                allMessagesHasBeenShown = false;
            }
        }
    }

    void HideTestMessage() {
        if (testMessageObject != null) {
            Destroy(testMessageObject);
            testMessageObject = null;
        }
    }

}
