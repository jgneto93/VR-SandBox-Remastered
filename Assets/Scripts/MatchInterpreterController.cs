using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
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
    private GameObject subtitles;

    private UserMovementLogger uML;
    private UserInputLogger uIL;
    private float timeSinceLastLog = 0f; // Vari�vel para acumular o tempo
    private float timeSinceLineInteraction = 0f; // Vari�vel para acumular o tempo

    private LineRenderer lastLinerenderer = null;
    private bool setCameraOnHead = false;
    GameObject referencia = null;
    string referenciaName = "";

    public GameObject tutorial;
    private bool isTutorialActive = false;
    private List<LineRenderer> lrList;
    OVRPlayerController playerController;

    private int currentTest1 = 0;
    private int totalTests1 = 3;

    private bool allMessagesHasBeenShown = false;
    private bool isShowingTestMessages = false;

    //private bool actualPlayingTest = false;
    private int actualPlayingTest = 0;
    private int totalPlayingTest = 4;

    private bool testSetup = false;
    private bool testStarted = false;
    private bool testFinished = false;

    private bool test2isMoving = false;

    private bool LAmovDirectionN = false, LAmovDirectionS = false, LAmovDirectionL = false, LAmovDirectionO = false;
    private bool rotationTest = false;
    GameObject cameraRig;
    Canvas canvas;

    private int[] frameNumbers = {
        1123, 640, 9336, 9066, 3589,
        7490, 7038,  6879, 4492,2250
    };

    private string[] playerObjects = {
        "playerObject 6609", "playerObject 10046", "playerObject 14013","playerObject 12011", "playerObject 14013",
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
        lrList.Add(GameObject.Find("BallPrefab").transform.GetComponentInChildren<LineRenderer>());
        playerController = oculusInteractionSampleRig.GetComponent<OVRPlayerController>();
        cameraRig = GameObject.Find("OVRCameraRig");
        centerEye = GameObject.Find("CenterEyeAnchor");
        subtitles = GameObject.Find("Subtitles");
        uML = new();
        uIL = new();
        canvas = GameObject.Find("Subtitles").GetComponent<Canvas>();
        HideTestMessage();
    }

    void setTutorialonRightHand() {
        tutorial.transform.position = rightController.transform.position;
        tutorial.transform.LookAt(oculusInteractionSampleRig.transform);
        tutorial.transform.Rotate(90, -90, 0);
    }

    void PauseUnpause() {
        if (!isPaused) {
            uIL.LogCustomLine("A-Pause");
        } else
            uIL.LogCustomLine("A-Resume");

        jsonReader.TogglePlay();
        isPaused = !isPaused;
    }
    void EnablePlayerAfterTest() {

    }
    void DisablePlayerForTest() {

    }

    void LogMovement() {
        if (timeSinceLastLog >= .5f) {
            uML.LogMovement(centerEye.transform);
            timeSinceLastLog = 0f; // Reseta o acumulador de tempo
        }
    }

    void Update() {
        currentFrame = jsonReader.GetFrameIndex();
        
        timeSinceLastLog += Time.deltaTime;
        timeSinceLineInteraction += Time.deltaTime;

        LogMovement();

        
        if (actualPlayingTest != 0) {
            if (actualPlayingTest == 1) {
                if (testSetup == false) {
                    if (!isPaused) 
                        PauseUnpause();
                    ShowTestMessages();
                    isShowingTestMessages = true;
                    if (allMessagesHasBeenShown) {
                        rotationTest = false;
                        ResetMovementBooleans();
                        testSetup = true;
                        testStarted = true;
                    }
                }

                if (testStarted == true && testFinished == false) {
                    if (!rotationTest) {
                        ShowTestMessage($"{(LAmovDirectionS == false ? "Mova-se para Tr�s\n" : "")}" +
                            $"{(LAmovDirectionL == false ? "Mova-se para Direita\n" : "")}" +
                            $"{(LAmovDirectionN == false ? "Mova-se para Frente\n" : "")}" +
                            $"{(LAmovDirectionO == false ? "Mova-se para Esquerda\n" : "")}");
                        if (LAmovDirectionS && LAmovDirectionO && LAmovDirectionN && LAmovDirectionL) {
                            rotationTest = true;
                            ResetMovementBooleans();
                        }
                    }
                    if (rotationTest) {
                        ShowTestMessage($"{(LAmovDirectionL == false ? "Rotacione-se para Direita\n" : "")}" +
                                        $"{(LAmovDirectionO == false ? "Rotacione-se para Esquerda\n" : "")}");
                        if (LAmovDirectionO && LAmovDirectionL) {
                            ShowTestMessage($"Treinamento de movimenta��o finalizado\n" +
                                            "--Quando Preparado Pressione A--");
                            testFinished = true;
                        }
                    }
                }
                if (testFinished) {
                    NextTest();
                    if (!isPaused)
                        PauseUnpause();
                }
            }
            if (actualPlayingTest == 2) {
                // x (-40 <-> 40)
                if (testSetup == false) {
                    playerController.enabled = false;
                    oculusInteractionSampleRig.transform.position = new Vector3(0, 1, -40); // Canto da linha do meio campo
                    jsonReader.SetFrameIndex(10);
                    ShowTestMessages();
                    isShowingTestMessages = true;

                    if (allMessagesHasBeenShown) {
                        testSetup = true;
                        testStarted = true;
                        playerController.enabled = true;
                    }
                }
                if (testStarted && testSetup) {
                    if (isPaused)
                        PauseUnpause();
                    if (oculusInteractionSampleRig.transform.position.z >= 30) {
                        testFinished = true;
                    }
                }
                if (testFinished) {
                    if (!isPaused)
                        PauseUnpause();
                    NextTest();
                }
            }
            if (actualPlayingTest == 3) {
                if (testSetup == false) {
                    
                    jsonReader.SetFrameIndex(frameNumbers[currentTest1] - 155);
                    testSetup = true;
                    referencia = GameObject.Find(playerObjects[currentTest1]).GetComponentInChildren<Camera>().gameObject;
                    referenciaName = GameObject.Find(playerObjects[currentTest1]).gameObject.name;
                    setCameraOnHead = true;
                    SetCameraToPlayer(playerController, referencia);
                    
                    ShowTestMessage($"Teste {currentTest1} de {totalTests1} Iniciado\n" +
                                    $"Aten��o\n" +
                                    "-Pressione A--");
                }
                if (testSetup && currentFrame < frameNumbers[currentTest1] - 150 && !testStarted) {
                    if (!isPaused) {
                        HideTestMessage();
                        PauseUnpause();
                    }
                    testStarted = true;
                }
                if (currentFrame >= (frameNumbers[currentTest1] - 3) && currentFrame <= (frameNumbers[currentTest1] + 3) && !testFinished) {
                    if (!isPaused) {
                        HideTestMessage();
                        PauseUnpause();
                    }
                    ShowTestMessages();
                    isShowingTestMessages = true;
                    if (allMessagesHasBeenShown && isPaused) {
                        HideTestMessage();
                        PauseUnpause();
                    }
                    testFinished = true;
                }
                if (testStarted && currentFrame >= (frameNumbers[currentTest1] + 200)) {
                    NextTest();
                }
            } 
            if (actualPlayingTest == 4) {
                if (testSetup == false && testStarted == false) {
                    jsonReader.SetFrameIndex(590);
                    referencia = GameObject.Find("playerObject 5124").GetComponentInChildren<Camera>().gameObject;
                    if(!isPaused)
                        PauseUnpause();
                    setCameraOnHead = true;
                    SetCameraToPlayer(playerController, referencia);
                    isSlowMotion = true;
                    ShowTestMessage("Teste 2 (Movimenta��o) Iniciado\nPressione A para continuar...");
                    testSetup = true;

                } else if (currentFrame >= 966 && testStarted == false) {
                    HideTestMessage();
                    if (!isPaused) {
                        PauseUnpause();
                        //GameObject.Find("playerObject 5124").SetActive(false);
                    }
                    ShowTestMessages();
                    isShowingTestMessages = true;
                    if (allMessagesHasBeenShown && isPaused) {
                        PauseUnpause();
                    }
                    testStarted = true;
                }
                if (testStarted == true && test2isMoving == false) {
                    if (testStarted == true) {
                        if (playerController.enabled == false) {
                            playerController.enabled = true;
                        }
                        //GameObject.Find("playerObject 5124").SetActive(false);
                        setCameraOnHead = false;
                        playerController.enabled = true;
                        test2isMoving = true;
                    }
                } else if (jsonReader.GetFrameIndex() >= 1100 && testFinished == false) {
                    testFinished = true;
                    if (!isPaused) {
                        //GameObject.Find("playerObject 5124").SetActive(true);
                        PauseUnpause();
                        ShowTestMessage("Teste 2 Conclu�do\nPressione A para ver\no prosseguimento da jogada...");
                    }
                    NextTest();
                }
            }
        }
        

        if (OVRInput.GetDown(OVRInput.Button.Two) && !OVRInput.GetDown(OVRInput.Button.Four) && (!OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) || !OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))) {
            uIL.LogCustomLine($"Tutorial {isTutorialActive}");
            isTutorialActive = !isTutorialActive;
            tutorial.SetActive(isTutorialActive);
        }

        if (playerController.enabled is false && setCameraOnHead == true) {
            SetCameraToPlayer(playerController, referencia);
        }

        if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y > 0.5f) {
            timeline.SetSliderValue(timeline.GetSliderValue() + (int)Math.Round(25 * OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y));
            uIL.LogCustomLine("Tempo Avan�ado");
        }
        if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y < -0.5f) {
            timeline.SetSliderValue(timeline.GetSliderValue() + (int)Math.Round(25 * OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y));
            uIL.LogCustomLine("Tempo Retrocedido");
        }

        if(actualPlayingTest == 1 && testStarted == true && testFinished == false) {
            if (!rotationTest) { //Se n eh o de rotacao eh o de movimenta��o
                if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x > 0.95f) {
                    if (!LAmovDirectionL) {
                        LAmovDirectionL = true;
                    }
                }
                if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x < -0.95f) {
                    if (!LAmovDirectionO) {
                        LAmovDirectionO = true;
                    }
                }
                if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y > 0.95f) {
                    if (!LAmovDirectionN) {
                        LAmovDirectionN = true;
                    }
                }
                if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y < -0.95f) {
                    if (!LAmovDirectionS) {
                        LAmovDirectionS = true;
                    }
                }
            }
            else if (rotationTest) {  //se eh o de rotacao
                if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x > 0.95f) {
                    if (!LAmovDirectionL) {
                        LAmovDirectionL = true;
                    }
                }
                if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x < -0.95f) {
                    if (!LAmovDirectionO) {
                        LAmovDirectionO = true;
                    }
                }
            }
        }


        if (OVRInput.GetDown(OVRInput.Button.One)) { // Pausa ou despausa a cena
            if (actualPlayingTest != 0 && isShowingTestMessages) {
                ShowTestMessages();
                currentTestMessages += 1;
            } else {
                PauseUnpause();
            }
        }

        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger)) {
            if (timeSinceLineInteraction >= .175f) {
                Ray ray = new(leftController.position, leftController.forward);
                laserLineRenderer.SetPosition(0, leftController.position);
                if (Physics.Raycast(ray, out RaycastHit hit)) {
                    Transform parent = hit.transform.parent;
                    laserLineRenderer.SetPosition(1, hit.point);

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
                                if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger)) { //Ativar 1 s�
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
                                if (actualPlayingTest != 0) {
                                    uIL.LogCustomLine($"Jogador {parent.transform.name} Selecionado");
                                }
                                if (!setCameraOnHead && actualPlayingTest == 0) {
                                    uIL.LogCustomLine($"Path do Jogador {parent.transform.name} Ativado");
                                } else if (actualPlayingTest == 0) {
                                    uIL.LogCustomLine($"Vis�o Alternada Para {parent.transform.name}");
                                }
                            }
                        }
                        if (hit.transform.name is not "BallPrefab" && hit.transform.parent.name is not "Stadium") {
                            if (OVRInput.GetDown(OVRInput.Button.Four)) {
                                setCameraOnHead = true;
                                timeSinceLineInteraction = 0f; // Reseta o acumulador de tempo
                                SetCameraToPlayer(playerController, referencia);
                            }
                        }
                    }
                    if (hit.transform.name is "BallPrefab") { 
                        LineRenderer balllineRenderer = hit.transform.GetComponentInChildren<LineRenderer>();
                        if (balllineRenderer != null) {
                            balllineRenderer.enabled = !balllineRenderer.enabled;
                            uIL.LogCustomLine("Path da Bola Ativado");

                        }
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
                uIL.LogCustomLine("Slow Motion Off");

                isSlowMotion = false;
            } else {
                Time.timeScale = 0.33f; // slow down to 50% speed
                uIL.LogCustomLine("Slow Motion On");

                isSlowMotion = true;
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.Four)) { // desativa todos os tracers
            uIL.LogCustomLine("Paths Desativados por Comando");
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
                uIL.LogCustomLine("TP Campo");
                cameraRig.transform.parent.position = new Vector3(0, 1, 0);
            } else {
                uIL.LogCustomLine("TP Arquibancada");
                cameraRig.transform.parent.position = new Vector3(0, 17, -53);
            }
            cc.enabled = true;
            parent.GetComponent<OVRPlayerController>().enabled = true;
        }


        if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) &&
            OVRInput.GetDown(OVRInput.Button.Two) && OVRInput.GetDown(OVRInput.Button.Four)) {
            actualPlayingTest = 1;
            //NextTest();
        }

        if (jsonReader.isPlaying) {
            jsonReader.InstantiatePlayersFromFrame(jsonReader.currentFrameIndex);
        }
        jsonReader.UpdateShirtNumberRotation(oculusInteractionSampleRig.transform);
        setTutorialonRightHand();

    }

    void SetCameraToPlayer(OVRPlayerController playerController, GameObject referencia) {
        if (referencia is not null) {
            
            if(setCameraOnHead == false) {
                uIL.LogCustomLine($"Vendo o Ponto de Vista do Jogador {referenciaName}");
                uIL.LogCustomLine("Saiu da Vis�o do Jogador");
                playerController.enabled = true;
                setCameraOnHead = false;
                return;
            }
            
            oculusInteractionSampleRig.transform.position = referencia.transform.position;
            playerController.enabled = false;

            if (setCameraOnHead == true) {
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
            } else if (OVRInput.Get(OVRInput.Button.Start) || setCameraOnHead == false) {
                uIL.LogCustomLine($"Vendo o Ponto de Vista do Jogador {referenciaName}");
                uIL.LogCustomLine("Saiu da Vis�o do Jogador");
                playerController.enabled = true;
                setCameraOnHead = false;
                return;
            }
        }
    }

    void NextTest() {
        if(actualPlayingTest == 1) {
            uIL.LogCustomLine("Pr�-Treino de Movimenta��o Conclu�do");
            uML.LogCustomLine("Pr�-Treino de Movimenta��o Conclu�do");
            ResetBooleans();
            ResetTest();
            actualPlayingTest++;
        }
        else if (actualPlayingTest == 2) {
            uML.LogCustomLine("Pr�-Treino de Movimenta��o Olhando Conclu�do");
            uIL.LogCustomLine("Pr�-Treino de Movimenta��o Olhando Conclu�do");

            ResetBooleans();
            ResetTest();
            actualPlayingTest++;

        }
        else if (actualPlayingTest == 3) { //OLHA O ELSE IF...
            uML.LogCustomLine($"Cen�rio Tomada Decis�o {currentTest1} Finalizado");
            uIL.LogCustomLine($"Cen�rio Tomada Decis�o {currentTest1} Finalizado");
            currentTest1 += 1;
            isShowingTestMessages = false;

            ResetBooleans();
            if (currentTest1 >= totalTests1) {
                actualPlayingTest++;
                ResetTest();
            }
        } else if (actualPlayingTest == 4) {//OLHA O ELSE IF...
            uML.LogCustomLine($"Teste {actualPlayingTest} Finalizado");
            uIL.LogCustomLine($"Teste {actualPlayingTest} Finalizado");
            actualPlayingTest++;
            ResetBooleans();

        } else if(actualPlayingTest > totalPlayingTest) {
            EndTests();
            actualPlayingTest = 1;
        }
        
    }

    void ResetBooleans() {
        allMessagesHasBeenShown = false;
        currentTestMessages = 0;
        testStarted = false;
        testSetup = false;
        testFinished = false;
        test2isMoving = false;
        ResetMovementBooleans();
    }
    void ResetMovementBooleans() {
        LAmovDirectionN = false;
        LAmovDirectionS = false;
        LAmovDirectionL = false;
        LAmovDirectionO = false;
    }

    void ResetTest() {
        setCameraOnHead = false;
        playerController.enabled = true;
        ResetBooleans();
        currentTest1 = 0;
    }

    void EndTests() {
        HideTestMessage();
        ResetTest();
        uML.LogCustomLine("Todos os Testes Finalizados");
        uIL.LogCustomLine("Todos os Testes Finalizados");
        uML.CreateNewTestLogFile();
        uIL.CreateNewTestLogFile();
        actualPlayingTest = 0;

    }

    void ShowTestMessage(string message) {
        ActivateTestMessage();
        Transform textTransform = subtitles.transform.Find("SubtitlesText");
        TextMeshProUGUI textMesh = textTransform.GetComponent<TextMeshProUGUI>();

        textMesh.text = message;

        
        uIL.LogCustomLine(message.Replace("\n", " "));
    }

    private string[][] passDecisionTestMessages = new string[][]{
        new string[] { // pre testes (movimenta��o 1)
            "Para ver o menu dos controles\nOlhe nas arquibancadas\nOu pressione B",
            "Utilize o LA para movimentar",
            "Utilize o RA para rotacionar"
        },
        new string[] { // pre testes (movimenta��o 2)
            "Voc� dever� se movimentar pela linha do meio campo\nOlhando para a bola",
            "� muito importante que voc� acompanhe a bola\ne n�o a perca de visa"
        },
        new string[]{ // teste 1
            "Quem tocou a bola para voc�",
            "O que op��es de jogada voc� tem",
            "Qual seria sua escolha nesse lance?\n(Passe,Drible,Chute...)"
        },
        new string[]{ // teste 2
            "A partir de agora voc� assumir�\n o controle desse jogador",
            "Ele � um volante\n do time vermelho",
            "Mova-se e posicione-se\n como se fosse voc� jogando",
            "Ap�s pressionar A, a jogada continuar�"
        }
    };

    private int currentTestMessages = 0;

    void ShowTestMessages() {
        
        ActivateTestMessage();

        if (currentTestMessages == passDecisionTestMessages[actualPlayingTest -1].Length) { 
            allMessagesHasBeenShown = true;
            isShowingTestMessages = false;
            currentTestMessages = 0;
            HideTestMessage();
        } //OLHA O ELSE IF...

        else {
            if(actualPlayingTest == 1) {
                ShowTestMessage("Treinamento de Movimenta��o\n" +
                    $"{passDecisionTestMessages[actualPlayingTest - 1][currentTestMessages]}\n" +
                    $"-- Pressione A para continuar --");
                allMessagesHasBeenShown = false;
            }//OLHA O ELSE IF...
            else if (actualPlayingTest == 2) {
                ShowTestMessage("Treinamento de Movimenta��o\nOlhando para a bola\n" +
                    $"{passDecisionTestMessages[actualPlayingTest - 1][currentTestMessages]}\n" +
                    $"-- Pressione A para continuar --");
                allMessagesHasBeenShown = false;
            }
            else if (actualPlayingTest == 3) { 
                ShowTestMessage($"Cen�rio {currentTest1 + 1} de {totalTests1}\n" +
                $"Responda em Voz Alta\n" +
                $"{passDecisionTestMessages[actualPlayingTest -1][currentTestMessages]}\n" +
                $"-- Pressione A para continuar --");
                allMessagesHasBeenShown = false;
            }//OLHA O ELSE IF...
            else if(actualPlayingTest == 4) {
                ShowTestMessage($"Teste 2: Movimenta��o\n" +
                    $"{passDecisionTestMessages[actualPlayingTest - 1][currentTestMessages]}\n" +
                    $"-- Pressione A para continuar --");
                allMessagesHasBeenShown = false;
            }
        }
    }
    void ActivateTestMessage() {
        canvas.enabled = true;
    }
    void HideTestMessage() {
        
        canvas.enabled = false;

    }

}
