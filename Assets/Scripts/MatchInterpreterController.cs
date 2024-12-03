using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using System.Collections;
public class MatchInterpreterController : MonoBehaviour {
    public JSONReader jsonReader; // Reference to the JSONReader script
    public Camera mainCamera; // Reference to the main camera
    private Transform rightController; // Reference to the right controller
    private Transform leftController; // Reference to the right controller
    private LineRenderer laserLineRenderer; // Reference to the LineRenderer component
    private bool isOnField = true;
    public Timeline timeline;

    public GameObject leftHandSubtitles;
    public GameObject rightHandSubtitles;

    public GameObject test3Arrow;

    public GameObject leftHandButtons;
    public GameObject rightHandButtons;


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

    private bool logDetailed = false;
    private float timeSinceLastLog = 0f;

    private LineRenderer lastLinerenderer = null;
    private bool setCameraOnHead = false;
    GameObject referencia = null;
    string referenciaName = "";

    private List<LineRenderer> lrList;
    OVRPlayerController playerController;

    private int currentTest1 = 0;
    private int totalTests1 = 3;
    bool registredStart = false;


    private bool allMessagesHasBeenShown = false;
    private bool isShowingTestMessages = false;
    private bool isShowingAMessage = false;
    
    public GameObject selectionMarkerPrefab;
    private List<GameObject> selectionmarkers = new(); 

    private int actualPlayingTest = 0;
    private int totalPlayingTest = 8;

    private bool testSetup = false;
    private bool testStarted = false;
    private bool testFinished = false;

    private bool test2isMoving = false;

    private bool LAmovDirectionN = false, LAmovDirectionS = false, LAmovDirectionL = false, LAmovDirectionO = false;
    private bool rotationTest = false;
    GameObject cameraRig;
    Canvas canvas;
    private float previousTimeScale = 1f;

    private float inicialTime;

    private GameObject playerObject;

    private int[] frameNumbers = {
        1123, 640, 9336, 9066, 3589,
        7490, 7038,  6879, 4492,2250
    };

    private string[] playerObjects = {
        "playerObject 6609", "playerObject 10046", "playerObject 14013","playerObject 12011", "playerObject 14013",
        "playerObject 3120", "playerObject 5124", "playerObject 16126", "playerObject 5124",  "playerObject 3120"
    };

    void InstantiateSelectionMarker(GameObject target) {
        bool markerExists = false;
        for (int i = 0; i < selectionmarkers.Count; i++) {
            if (selectionmarkers[i].transform.parent == target.transform) {
                Destroy(selectionmarkers[i]);
                selectionmarkers.RemoveAt(i);
                markerExists = true;
                break; 
            }
        }
        if (!markerExists && selectionMarkerPrefab != null) {
            Vector3 markerPosition = jsonReader.GetCenterOfMassFromPlayerObject(target) + Vector3.up * 1.1f;
            GameObject marker = Instantiate(selectionMarkerPrefab, markerPosition, Quaternion.identity);
            marker.transform.SetParent(target.transform);
            selectionmarkers.Add(marker);
            uIL.LogCustomLine(2, currentFrame,$"Jogador Selecionado Para o Teste: {target.transform.name}");
        } else if (selectionMarkerPrefab == null) {
            Debug.LogWarning("Prefab de indicador visual nao atribuido.");
        }
    }
    void RemoveAllSelectionMarkers() {
        foreach (GameObject marker in selectionmarkers) {
            Destroy(marker);
        }
        selectionmarkers.Clear();
    }

    void Start() {
        rightController = GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor").transform;
        leftController = GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor").transform;
        GameObject laserLineObject = new("LaserLine");
        laserLineRenderer = laserLineObject.AddComponent<LineRenderer>();
        laserLineRenderer.startWidth = 0.01f; 
        laserLineRenderer.endWidth = 0.01f; 
        laserLineRenderer.material = new Material(Shader.Find("Unlit/Color")) { color = Color.red };
        laserLineRenderer.positionCount = 2; 
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


    void PauseUnpause() {
        if (!isPaused) {
            uIL.LogCustomLine(2,currentFrame, "A-Pause");
            previousTimeScale = Time.timeScale;
            StartCoroutine(SmoothTimeScaleChange(Time.timeScale, 0f, false));
        } else {
            uIL.LogCustomLine(2,currentFrame, "A-Resume");
            StartCoroutine(SmoothTimeScaleChange(Time.timeScale, previousTimeScale, true));
        }

        jsonReader.TogglePlay();
        isPaused = !isPaused;
    }

    IEnumerator SmoothTimeScaleChange(float from, float to, bool isUnpausing) {
        float duration = .5f; // Duration of the transition
        float elapsed = 0f;

        while (elapsed < duration) {
            elapsed += Time.unscaledDeltaTime; // Use unscaledDeltaTime to run when timeScale is 0
            float t = elapsed / duration;
            float timeScale;
            if (isUnpausing) {
                if (t < 0.5f)
                    timeScale = Mathf.Lerp(from, from + 0.1f, t * 2);
                else
                    timeScale = Mathf.Lerp(from + 0.1f, to, (t - 0.5f) * 2);
            } else {
                if (t < 0.5f)
                    timeScale = Mathf.Lerp(from, from * 0.5f, t * 2);
                else
                    timeScale = Mathf.Lerp(from * 0.5f, to, (t - 0.5f) * 2);
            }
            Time.timeScale = timeScale;
            yield return null;
        }
        Time.timeScale = to;
    }

    void DisablePlayerForTest() {
        if (playerObject != null) {
            playerObject.SetActive(false);
            Debug.Log($"{playerObject.name} desativado para teste);");
        } else {
            Debug.LogWarning($"{playerObject.name} nao encontrado ao desativar.");
        }
    }

    void EnablePlayerAfterTest() {
        if (playerObject != null) {
            playerObject.SetActive(true);
            Debug.Log($"{playerObject.name} reativado.");
        } else {
            Debug.LogWarning($"{playerObject.name} nao encontrado ao reativar.");
        }
    }

    void LogMovement() {
        uML.LogMovement(currentFrame, centerEye.transform);
    }

    void Update() {
        currentFrame = jsonReader.GetFrameIndex();
        
        timeSinceLastLog += Time.unscaledDeltaTime;

        if (timeSinceLastLog >= (logDetailed ? 0.05f : 0.025f)) {
            LogMovement();
            timeSinceLastLog = 0f; 
        }

        if (actualPlayingTest != 0) {
            if (actualPlayingTest == 1) {
                if (testSetup == false) {
                    if (!isPaused) 
                        PauseUnpause();
                    ShowTestMessages();
                    if (allMessagesHasBeenShown) {
                        rotationTest = false;
                        ResetMovementBooleans();
                        testSetup = true;
                        testStarted = true;
                        uML.LogCustomLine($"Treinamento 1 Iniciado");
                        uIL.LogCustomLine(3,currentFrame, $"Treinamento 1 Iniciado");
                    }
                }
                if (testStarted == true && testFinished == false) {
                    if (!rotationTest) {
                        ShowTestMessage($"{(LAmovDirectionS == false ? "Mova-se utilizando o controle esquerdo para Tras\n" : "")}" +
                            $"{(LAmovDirectionL == false ? "Mova-se utilizando o controle esquerdo para Direita\n" : "")}" +
                            $"{(LAmovDirectionN == false ? "Mova-se utilizando o controle esquerdo para Frente\n" : "")}" +
                            $"{(LAmovDirectionO == false ? "Mova-se utilizando o controle esquerdo para Esquerda\n" : "")}");
                        if (LAmovDirectionS && LAmovDirectionO && LAmovDirectionN && LAmovDirectionL) {
                            rotationTest = true;
                            ResetMovementBooleans();
                        }
                    }
                    if (rotationTest) {
                        ShowTestMessage($"{(LAmovDirectionL == false ? "Gire utilizando o controle para Direita\n" : "")}" +
                                        $"{(LAmovDirectionO == false ? "Gire utilizando o controle para Esquerda\n" : "")}");
                        if (LAmovDirectionO && LAmovDirectionL) {
                            ShowTestMessage($"Treinamento de movimentacao finalizado\n" +
                                            "--Quando Preparado Pressione A--");
                            testFinished = true;
                            uML.LogCustomLine($"Treinamento 1 Finalizado");
                            uIL.LogCustomLine(3,currentFrame, $"Treinamento 1 Finalizado");
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
                if (testSetup == false) {
                    jsonReader.SetFrameIndex(frameNumbers[currentTest1] - 450);
                    referencia = GameObject.Find(playerObjects[currentTest1]).GetComponentInChildren<Camera>().gameObject;
                    referenciaName = GameObject.Find(playerObjects[currentTest1]).gameObject.name;
                    setCameraOnHead = true;
                    SetCameraToPlayer(playerController, referencia);
                    testSetup = true;
                }
                if (testSetup && !testStarted) {
                    if (!isPaused) {
                        PauseUnpause();
                        testStarted = true;
                        ShowTestMessage(//$"Teste {currentTest1 + 1} de {totalTests1} Iniciado\n" +
                                "Nesse instante voce vai assistir um replay\nPelo ponto de vista de um jogador\nNo momento apenas aguarde\n" +
                                "Ao Pressionar A a cena vai continuar\n" +
                                "--Pressione A--");
                    }
                    logDetailed = true;
                }
                if(currentFrame > (frameNumbers[currentTest1] - (450 + 1)) && !registredStart && !testFinished && testStarted) {
                    uML.LogCustomLine($"Cenario Tomada Decisao {currentTest1} Iniciado");
                    uIL.LogCustomLine(3,currentFrame, $"Cenario Tomada Decisao {currentTest1} Iniciado");  
                    registredStart = true; 
                }
                if (currentFrame >= (frameNumbers[currentTest1] - 3) && currentFrame <= (frameNumbers[currentTest1] + 3) && !testFinished) {
                    HideTestMessage();
                    if (!isPaused) {
                        PauseUnpause();
                    }
                    ShowTestMessages();
                    if (allMessagesHasBeenShown && isPaused) {
                        HideTestMessage();
                        PauseUnpause();
                        testFinished = true;
                        uML.LogCustomLine($"Cenario Tomada Decisao {currentTest1} Finalizado");
                        Vector3 ballPos = jsonReader.GetBallPosition();
                        uIL.LogCustomLine(4,currentFrame, $"Posição da Bola: {ballPos}");
                        uIL.LogCustomLine(3,currentFrame, $"Cenario Tomada Decisao {currentTest1} Finalizado");
                        logDetailed = false;
                        registredStart = false;
                    }
                }
                if (currentFrame >= (frameNumbers[currentTest1] + 200) && testFinished) {
                    NextTest();
                }
            }
            if (actualPlayingTest == 3) {
                // x (-40 <-> 40)
                if (testSetup == false) {
                    playerController.enabled = false;
                    oculusInteractionSampleRig.transform.position = new Vector3(0, 1, -40); // Canto da linha do meio campo
                    oculusInteractionSampleRig.transform.rotation = new Quaternion(0, 0, 0, 0);
                    jsonReader.SetFrameIndex(10);
                    test3Arrow.SetActive(true);
                    ShowTestMessages();

                    if (allMessagesHasBeenShown) {
                        testSetup = true;
                        testStarted = true;
                        playerController.enabled = true;
                        uML.LogCustomLine($"Teste {actualPlayingTest} Iniciado");
                        uIL.LogCustomLine(3,currentFrame, $"Teste {actualPlayingTest} Iniciado");
                    }
                }
                if (testStarted && testSetup) {
                    if (isPaused)
                        PauseUnpause();
                    if (oculusInteractionSampleRig.transform.position.z >= 35) {
                        uML.LogCustomLine($"Teste {actualPlayingTest} Finalizado");
                        uIL.LogCustomLine(3,currentFrame, $"Teste {actualPlayingTest} Finalizado");
                        ShowTestMessage("Treinamento Concluido\nPrepare-se ate se sentir confortavel\nAo pressionar A os testes iniciarao");
                    }
                    if (OVRInput.GetDown(OVRInput.Button.One) && oculusInteractionSampleRig.transform.position.z >= 35) {
                        testFinished = true;
                    }
                }
                if (testFinished) {
                    test3Arrow.SetActive(false);
                    if (!isPaused)
                        PauseUnpause();
                    NextTest();
                }
            }
            if (actualPlayingTest == 4) {
                if (testSetup == false && testStarted == false) {
                    jsonReader.SetFrameIndex(590);
                    playerObject = GameObject.Find("playerObject 5124");
                    referencia = GameObject.Find("playerObject 5124").GetComponentInChildren<Camera>().gameObject;
                    if(!isPaused)
                        PauseUnpause();
                    setCameraOnHead = true;
                    SetCameraToPlayer(playerController, referencia);
                    isSlowMotion = true;
                    ShowTestMessage("Teste 2 (Movimentacao) Iniciado\n" +
                                     "Voce fara o papel de um jogador\n" +
                                     "Pressione A para continuar e apenas AGUARDE...");
                    testSetup = true;

                } else if (currentFrame >= 966 && testStarted == false) {
                    HideTestMessage();
                    if (!isPaused) {
                        PauseUnpause();
                        DisablePlayerForTest();
                    }
                    ShowTestMessages();
                    if (allMessagesHasBeenShown && isPaused) {
                        PauseUnpause();
                        testStarted = true;
                        uML.LogCustomLine($"Teste {actualPlayingTest} Movimentacao 1 Iniciado");
                        uIL.LogCustomLine(3,currentFrame, $"Teste {actualPlayingTest} Movimentacao 1 Iniciado");
                        logDetailed = true;

                    }
                }
                if (testStarted == true && test2isMoving == false) {
                    if (testStarted == true) {
                        if (playerController.enabled == false) {
                            playerController.enabled = true;
                        }
                        setCameraOnHead = false;
                        playerController.enabled = true;
                        test2isMoving = true;
                    }
                } else if (jsonReader.GetFrameIndex() >= 1250 && testFinished == false) {
                    EnablePlayerAfterTest();
                    ShowTestMessage("Teste Movimentacao 1 Concluido\nPressione A para prosseguir");
                    uML.LogCustomLine($"Teste {actualPlayingTest} Movimentacao 1 Finalizado");
                    uIL.LogCustomLine(3,currentFrame, $"Teste {actualPlayingTest} Movimentacao 1 Finalizado");
                    logDetailed = false;

                    if (!isPaused) {
                        PauseUnpause();
                    }
                    if (isPaused) {
                        testFinished = true;
                    }
                    NextTest();
                }
            }
            if (actualPlayingTest == 5) {
                if (testSetup == false && testStarted == false) {
                    jsonReader.SetFrameIndex(4500);
                    referencia = GameObject.Find("playerObject 10046").GetComponentInChildren<Camera>().gameObject;
                    if (!isPaused)
                        PauseUnpause();
                    setCameraOnHead = true;
                    SetCameraToPlayer(playerController, referencia);
                    isSlowMotion = true;
                    ShowTestMessage("Teste de Movimentacao 2 Iniciado\n" +
                                      "Voce fara o papel de um jogador\n" +
                                      "Pressione A para continuar e AGUARDE...");
                    testSetup = true;

                } else if (currentFrame >= 5000 && testStarted == false) {
                    HideTestMessage();
                    if (!isPaused) {
                        PauseUnpause();
                        DisablePlayerForTest();
                    }
                    ShowTestMessages();
                    if (allMessagesHasBeenShown && isPaused) {
                        PauseUnpause();
                        testStarted = true;
                        uML.LogCustomLine($"Teste {actualPlayingTest} Movimentacao 2 Iniciado");
                        uIL.LogCustomLine(3,currentFrame, $"Teste {actualPlayingTest} Movimentacao 2 Iniciado");
                        logDetailed = true;

                    }
                }
                if (testStarted == true && test2isMoving == false) {
                    if (testStarted == true) {
                        if (playerController.enabled == false) {
                            playerController.enabled = true;
                        }
                        setCameraOnHead = false;
                        playerController.enabled = true;
                        test2isMoving = true;
                    }
                } else if (jsonReader.GetFrameIndex() >= 5500 && testFinished == false) {
                    EnablePlayerAfterTest();
                    ShowTestMessage("Teste Movimentacao 2 Concluido\nPressione A para prosseguir");
                    uML.LogCustomLine($"Teste {actualPlayingTest} Movimentacao 2 Finalizado");
                    uIL.LogCustomLine(3,currentFrame, $"Teste {actualPlayingTest} Movimentacao 2 Finalizado");
                    logDetailed = false;

                    if (!isPaused) {
                        PauseUnpause();
                    }
                    if (isPaused) {
                        testFinished = true;
                    }
                    NextTest();
                }
            }
            if (actualPlayingTest == 6) {
                if (testSetup == false && testStarted == false) {
                    jsonReader.SetFrameIndex(5300);
                    referencia = GameObject.Find("playerObject 27137").GetComponentInChildren<Camera>().gameObject;
                    if (!isPaused)
                        PauseUnpause();
                    setCameraOnHead = true;
                    SetCameraToPlayer(playerController, referencia);
                    isSlowMotion = true;
                    ShowTestMessage("Teste de Movimentacao 3 Iniciado\n" +
                                     "Voce fara o papel de um jogador\n" +
                                     "Pressione A para continuar e AGUARDE...");
                    testSetup = true;

                } else if (currentFrame >= 5750 && testStarted == false) {
                    HideTestMessage();
                    if (!isPaused) {
                        PauseUnpause();
                        DisablePlayerForTest();
                    }
                    ShowTestMessages();
                    if (allMessagesHasBeenShown && isPaused) {
                        PauseUnpause();
                        testStarted = true;
                        uML.LogCustomLine($"Teste {actualPlayingTest} Movimentacao 3 Inciado");
                        uIL.LogCustomLine(3,currentFrame, $"Teste {actualPlayingTest} Movimentacao 3 Inciado");
                        logDetailed = true;
                    }
                }
                if (testStarted == true && test2isMoving == false) {
                    if (testStarted == true) {
                        if (playerController.enabled == false) {
                            playerController.enabled = true;
                        }
                        setCameraOnHead = false;
                        playerController.enabled = true;
                        test2isMoving = true;
                    }
                } else if (jsonReader.GetFrameIndex() >= 6250 && testFinished == false) {
                    EnablePlayerAfterTest();
                    ShowTestMessage("Teste Movimentacao 3 Concluido\nPressione A para prosseguir");
                    uML.LogCustomLine($"Teste {actualPlayingTest} Movimentacao 3 Finalizado");
                    uIL.LogCustomLine(3,currentFrame, $"Teste {actualPlayingTest} Movimentacao 3 Finalizado");
                    logDetailed = false;

                    if (!isPaused) {
                        PauseUnpause();
                    }
                    if (isPaused) {
                        testFinished = true;
                    }
                    NextTest();
                }
            }
            if (actualPlayingTest == 7) { // Pre testes cenerio livre
                if (testSetup == false) {
                    if (!isPaused)
                        PauseUnpause();
                    ShowTestMessages();
                    if (allMessagesHasBeenShown) {
                        rotationTest = false;
                        ResetMovementBooleans();
                        testSetup = true;
                        testStarted = true;
                        uML.LogCustomLine($"Teste {actualPlayingTest} Iniciado");
                        uIL.LogCustomLine(3,currentFrame, $"Teste {actualPlayingTest} Iniciado");
                    }

                }
                if (testStarted == true && testFinished == false) {
                    if(rotationTest == false) { 
                        if (LAmovDirectionL == false) {
                            ShowTestMessage("Utilize o LIT para para ativar a\ntrajetoria de varios jogadores");
                        }
                        if (LAmovDirectionL == true && LAmovDirectionO == false) {
                            ShowTestMessage("Utilize o LIT + LHT para ativar a\ntrajetoria de apenas um jogador");
                        }
                        if (LAmovDirectionL == true && LAmovDirectionO == true && LAmovDirectionN == false) {
                            if(isPaused)
                                PauseUnpause();
                            ShowTestMessage("Utilize o LIT + Y para visualizar\no ponto de vista de um jogador");
                        }
                        if (LAmovDirectionL == true && LAmovDirectionO == true && LAmovDirectionN == true && LAmovDirectionS == false) {
                            ShowTestMessage("Alterne entre os pontos de vistas\ndos jogadoresn utilizando o LIT\nUtilize o Botao Start Para Sair da Visao do Jogador\ne Alternar entre a Arquibancada e o Campo");
                        }
                        if (LAmovDirectionL == true && LAmovDirectionO == true && LAmovDirectionN == true && LAmovDirectionS == true) {
                            ResetMovementBooleans();
                            rotationTest = true;
                        }
                    }
                    if (rotationTest == true) {
                        if (LAmovDirectionL == false) {
                            ShowTestMessage("Utilize o Y para apagar as trajetorias");
                        }
                        if (LAmovDirectionL == true && LAmovDirectionO == false) {
                            ShowTestMessage("Utilize o botao X para\nAtivar/Desativar a camera lenta");
                        } 
                        if (LAmovDirectionL == true && LAmovDirectionO == true && LAmovDirectionN == false) {
                            ShowTestMessage("Utilize o RHT + RA para\nAvancar ou Retroceder o tempo\nNo meio da sua tela e possivel ver\na barra de progresso do replay");
                        }
                        if (LAmovDirectionL == true && LAmovDirectionO == true && LAmovDirectionN == true) {
                            ShowTestMessage("Utilize o botao A para\nPausar / Resumir  a cena");

                            testFinished = true;
                            uML.LogCustomLine($"Teste {actualPlayingTest} Finalizado");
                            uIL.LogCustomLine(3,currentFrame,$"Teste {actualPlayingTest} Iniciado");
                        }
                    }
                }
                if (testFinished == true) {
                    NextTest();
                }
            }
            if (actualPlayingTest == 8) {
                if(testSetup == false) {
                    if (!isPaused)
                        PauseUnpause();
                    inicialTime = Time.unscaledTime;
                    jsonReader.SetFrameIndex(2200);
                    uIL.LogCustomLine(3,currentFrame, $"Teste {actualPlayingTest} Iniciado");
                    uML.LogCustomLine($"Teste {actualPlayingTest} Iniciado");
                    ShowTestMessage("Teste 3\n Uso Livre da Ferramenta\nPor 2 minutos voce estara livre para usar a ferramenta\n da forma que mais te agradar");
                    PauseUnpause();
                    testStarted = true;
                    testSetup = true;
                }
                if (testStarted == true && testFinished == false) {
                    if (Time.unscaledTime - inicialTime > 120.0f) {
                        testFinished = true;
                        uML.LogCustomLine($"Teste {actualPlayingTest} Finalizado");
                        uIL.LogCustomLine(3,currentFrame, $"Teste {actualPlayingTest} Finalizado");
                        ShowTestMessage("Testes Finalizados!\nVoce Pode Retirar o Oculus Agora!");
                    }
                }
                if(testFinished == true && isShowingAMessage == false) {
                    NextTest();
                }
            }
        }
        
        if (OVRInput.GetDown(OVRInput.Button.Two)) {
            rightHandSubtitles.SetActive(!rightHandSubtitles.activeSelf);
            leftHandSubtitles.SetActive(!leftHandSubtitles.activeSelf);
            leftHandButtons.SetActive(!leftHandButtons.activeSelf);
            rightHandButtons.SetActive(!rightHandButtons.activeSelf);
        }

        if (playerController.enabled is false && setCameraOnHead == true) {
            SetCameraToPlayer(playerController, referencia);
        }

        if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y > 0.2f) {
            timeline.SetSliderValue(timeline.GetSliderValue() + (int)Math.Round(25 * OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y));
            uIL.LogCustomLine(2,currentFrame,"Tempo Avancado");
        }
        if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y < -0.2f) {
            if (actualPlayingTest == 7 && testStarted == true && testFinished == false && LAmovDirectionO == true && LAmovDirectionO == true && rotationTest == true) {
                LAmovDirectionN = true;
            }
            timeline.SetSliderValue(timeline.GetSliderValue() + (int)Math.Round(25 * OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y));
            uIL.LogCustomLine(2,currentFrame, "Tempo Retrocedido");
        }

        if(actualPlayingTest == 1 && testStarted == true && testFinished == false) {
            if (!rotationTest) { //Se n eh o de rotacao eh o de movimentacao
                if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x > 0.9f) {
                    if (!LAmovDirectionL) {
                        LAmovDirectionL = true;
                    }
                }
                if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x < -0.9f) {
                    if (!LAmovDirectionO) {
                        LAmovDirectionO = true;
                    }
                }
                if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y > 0.9f) {
                    if (!LAmovDirectionN) {
                        LAmovDirectionN = true;
                    }
                }
                if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y < -0.9f) {
                    if (!LAmovDirectionS) {
                        LAmovDirectionS = true;
                    }
                }
            }
            else if (rotationTest) {  //se eh o de rotacao
                if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x > 0.9f) {
                    if (!LAmovDirectionL) {
                        LAmovDirectionL = true;
                    }
                }
                if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x < -0.9f) {
                    if (!LAmovDirectionO) {
                        LAmovDirectionO = true;
                    }
                }
            }
        }
        

        if (OVRInput.GetDown(OVRInput.Button.One)) {
            if (actualPlayingTest == 7 && testStarted == true && testFinished == false && LAmovDirectionN == true && LAmovDirectionO == true && LAmovDirectionL == true && rotationTest == true) {
                LAmovDirectionS = true;
            }
            if (actualPlayingTest != 0 && (isShowingTestMessages)) {
                ShowTestMessages();
                if (actualPlayingTest == 2 && LAmovDirectionL) {
                    currentTestMessages += 1;
                    LAmovDirectionL = false;
                    RemoveAllSelectionMarkers();
                } else if (actualPlayingTest != 2) {
                    currentTestMessages += 1;
                }
            } else if (isShowingAMessage) {
                HideTestMessage();
                PauseUnpause();
            } else {
                PauseUnpause();
            }
        }

        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger)) {
            laserLineRenderer.enabled = true;
            laserLineRenderer.SetPosition(0, leftController.position);
            Ray ray = new Ray(leftController.position, leftController.forward);
            int layerMask = LayerMask.GetMask("Default");
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)){
                laserLineRenderer.SetPosition(1, hit.point);

                //if (timeSinceLineInteraction >= .5f) {
                    if (hit.transform.parent is not null && hit.transform.tag != "NoHit" && hit.transform.parent.tag != "NoHit") { 
                        Transform parent = hit.transform.parent;
                        laserLineRenderer.SetPosition(1, hit.point);
                    
                        GameObject oldreferencia = referencia;
                                        
                        if (actualPlayingTest != 2 && actualPlayingTest != 3) {
                            jsonReader.SetPlayerHeatMap(parent.gameObject, true);
                            referencia = parent.GetComponentInChildren<Camera>().gameObject;
                            referenciaName = parent.gameObject.name;
                        }
                        if (actualPlayingTest == 7 && testStarted == true && testFinished == false && rotationTest == false) {
                            LAmovDirectionL = true;//Teste 5 de ativar multiplas uma trajetoria
                        }
                        if (actualPlayingTest == 2) {
                            if (!LAmovDirectionL)
                                LAmovDirectionL = true;
                            if (parent != null) { 
                                if(currentTestMessages == 0) {
                                    RemoveAllSelectionMarkers();
                                    InstantiateSelectionMarker(parent.gameObject);
                                }
                                else if(currentTestMessages == 1) {
                                    InstantiateSelectionMarker(parent.gameObject);
                                } 
                                else if(currentTestMessages == 2) {
                                    RemoveAllSelectionMarkers();
                                    InstantiateSelectionMarker(parent.gameObject);
                                }
                            }
                        }

                        if (parent != null) {
                            Tracer tracer = parent.GetComponentInChildren<Tracer>();
                            if (tracer != null) {
                                LineRenderer lineRenderer = tracer.GetComponent<LineRenderer>();
                                if (lineRenderer != null && actualPlayingTest != 2) {
                                    if (lastLinerenderer is null) {
                                        lastLinerenderer = lineRenderer;
                                    }
                                    if (!OVRInput.Get(OVRInput.Button.PrimaryHandTrigger)) { //Ativar 1 soh
                                        foreach (LineRenderer lr in lrList) {
                                            lr.enabled = false;
                                            lr.positionCount = 0;
                                            if (actualPlayingTest == 7 && testStarted == true && testFinished == false && LAmovDirectionL == true && rotationTest == false) {
                                                LAmovDirectionO = true;//Teste 1 de ativar uma trajetoria
                                            }
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
                                        uIL.LogCustomLine(4, currentFrame, $"Jogador {parent.transform.name} Selecionado; {parent.transform.position}; {parent.transform.rotation}");
                                    }
                                    if (!setCameraOnHead && actualPlayingTest == 0) {
                                        uIL.LogCustomLine(2, currentFrame, $"Path do Jogador {parent.transform.name} Ativado");
                                    } else if (actualPlayingTest == 0) {
                                        uIL.LogCustomLine(2, currentFrame, $"Visao Alternada Para {parent.transform.name}");
                                    }
                                }
                            }
                            if (hit.transform.name is not "BallPrefab" && hit.transform.parent.name is not "Stadium") {
                                if (OVRInput.GetDown(OVRInput.Button.Four)) {
                                    if(actualPlayingTest == 7 && testStarted == true && testFinished == false && LAmovDirectionO == true && LAmovDirectionO == true && rotationTest == false) {
                                        LAmovDirectionN = true;
                                    }
                                    setCameraOnHead = true;
                                    SetCameraToPlayer(playerController, referencia);
                                }
                            }
                        }
                        if (hit.transform.name is "BallPrefab") { 
                            LineRenderer balllineRenderer = hit.transform.GetComponentInChildren<LineRenderer>();
                            if (balllineRenderer != null) {
                                balllineRenderer.enabled = !balllineRenderer.enabled;
                                uIL.LogCustomLine(2, currentFrame,"Path da Bola Ativado");

                            }
                        }
                    //}
                } else {
                    laserLineRenderer.SetPosition(1, ray.GetPoint(100)); 
                    jsonReader.SetPlayerHeatMap(null, true);
                }
                laserLineRenderer.enabled = true;
                //timeSinceLineInteraction = 0f;
            } 
            else {
                laserLineRenderer.SetPosition(1, ray.GetPoint(100));
            }
        } else {
            laserLineRenderer.SetPosition(1, leftController.forward * 100);
            laserLineRenderer.enabled = false;
        }

        if (OVRInput.GetDown(OVRInput.Button.Three)) {
            if (actualPlayingTest == 7 && testStarted == true && testFinished == false && LAmovDirectionL == true && rotationTest == true) {
                LAmovDirectionO = true;//Teste 1, 2 de Slowmo
            }
            if (isSlowMotion) {
                Time.timeScale = 1f; // restore normal speed
                uIL.LogCustomLine(2,currentFrame, "Slow Motion Off");

                isSlowMotion = false;
            } else {
                Time.timeScale = 0.33f; // slow down to 50% speed
                uIL.LogCustomLine(2,currentFrame, "Slow Motion On");

                isSlowMotion = true;
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.Four)) { // desativa todos os tracers
            if (actualPlayingTest == 7 && testStarted == true && testFinished == false && rotationTest == true) {
                LAmovDirectionL = true;//Teste 1,1 de pause-resume
            }
            uIL.LogCustomLine(2,currentFrame, "Paths Desativados por Comando");
            foreach (LineRenderer lr in lrList) {
                lr.enabled = false;
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.Start)) { // teleport para arquibancada / campo
            if (playerController.enabled == false) {
                playerController.enabled = true;
            }
            if (actualPlayingTest == 7 && testStarted == true && testFinished == false && LAmovDirectionN == true && LAmovDirectionO == true && LAmovDirectionL == true && rotationTest == false) {
                LAmovDirectionS = true;
            }
            Transform parent = cameraRig.transform.parent;
            isOnField = !isOnField;
            parent.GetComponent<OVRPlayerController>().enabled = false;
            CharacterController cc = parent.GetComponent<CharacterController>();
            cc.enabled = false;
            if (isOnField) {
                uIL.LogCustomLine(2,currentFrame, "TP Campo");
                cameraRig.transform.parent.position = new Vector3(0, 1, 0);
            } else {
                uIL.LogCustomLine(2,currentFrame, "TP Arquibancada");
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
        
        

    }
    void SetCameraToPlayer(OVRPlayerController playerController, GameObject referencia) {
        if (referencia is not null) {
            
            if(setCameraOnHead == false) {
                uIL.LogCustomLine(2,currentFrame, $"Vendo o Ponto de Vista do Jogador {referenciaName}");
                uIL.LogCustomLine(2, currentFrame,"Saiu da Visao do Jogador");
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
                uIL.LogCustomLine(2,currentFrame, $"Vendo o Ponto de Vista do Jogador {referenciaName}");
                uIL.LogCustomLine(2,currentFrame, "Saiu da Visao do Jogador");
                playerController.enabled = true;
                setCameraOnHead = false;
                return;
            }
        }
    }

    void NextTest() {
        if(actualPlayingTest == 1) {
            ResetBooleans();
            ResetTest();
            actualPlayingTest++;
        }
        
        else if (actualPlayingTest == 2) { //OLHA O ELSE IF...
            currentTest1 += 1;
            isShowingTestMessages = false;

            ResetBooleans();
            if (currentTest1 >= totalTests1) {
                actualPlayingTest++;
                ResetTest();
            }
        } 
        else if (actualPlayingTest == 3) {
            ResetBooleans();
            ResetTest();
            actualPlayingTest++;

        } 
        else if (actualPlayingTest == 4) {//OLHA O ELSE IF...
            actualPlayingTest++;
            ResetBooleans();

        } else if (actualPlayingTest == 5) {//OLHA O ELSE IF...
            actualPlayingTest++;
            ResetBooleans();

        } else if (actualPlayingTest == 6) {//OLHA O ELSE IF...
            
            actualPlayingTest++;
            ResetBooleans();

        } else if (actualPlayingTest == 7) {//OLHA O ELSE IF...

            actualPlayingTest++;
            ResetBooleans();

        } else if(actualPlayingTest >= totalPlayingTest) {
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
        uML.LogCustomLine("Todos os Testes Finalizados!");
        uIL.LogCustomLine(3,currentFrame, "Todos os Testes Finalizados!");
        uML.CreateNewTestLogFile();
        uIL.CreateNewTestLogFile();
        actualPlayingTest = 0;

    }

    void ShowTestMessage(string message) {
        ActivateTestMessage();
        Transform textTransform = subtitles.transform.Find("SubtitlesText");
        TextMeshProUGUI textMesh = textTransform.GetComponent<TextMeshProUGUI>();

        textMesh.text = message;
        
        uIL.LogCustomLine(1,currentFrame, message.Replace("\n", " "));
    }

    private string[][] passDecisionTestMessages = new string[][]{
        new string[] { // pre testes (movimentacao 1)
            "Testes Iniciados!\n",
            "Essa ferramenta fornece formas novas de\nvisualizar o REPLAY de uma partida de futebol\n",
            "A finalidade dela nao e ser um jogo\n",
            "Para ver as instrucoes de uso\nOlhe nas arquibancadas\nOu aperte B"
        },
        new string[]{ // teste 1
            "Quem tocou a bola para voce\nEscolha UM",
            "Quais possoveis opcoes de passe voce tem\nEscolhe VARIOS",
            "Qual seria sua decisao de passe nesse lance\nEscolha UM"
        },
        new string[] { // pre testes (movimentacao 2)
            "Esse treinamento tem o intuito de\nacostuma-lo com a movimentacao da ferramenta",
            "Voce devera atravessar o campo\nDe uma lateral para a outra\nSem sair da linha do meio campo\nOlhando para a bola",
            "E muito importante que voce acompanhe a bola\nsem a perder de vista"
        },
        new string[]{ // teste 2 - Depois do trigger
            "A partir de agora voce assumira\n o controle desse jogador",
            "Ele e um volante\n do time vermelho",
            "Mova-se e posicione-se\n como se fosse voce jogando",
            "Apos apertar o botao A, a jogada continuara"
        },
        new string[]{ // teste 2 - Depois do trigger
            "A partir de agora voce assumira\n o controle desse jogador",
            "Ele e um atacante\n do time azul",
            "Mova-se e posicione-se\n como se fosse voce jogando",
            "Apos apertar o botao A, a jogada continuara"
        },
        new string[]{ // teste 2 - Depois do trigger
            "A partir de agora voce assumira\n o controle desse jogador",
            "Ele e um atacante\n do time vermelho",
            "Mova-se e posicione-se\n como se fosse voce jogando",
            "Apos apertar o botao A, a jogada continuara"
        },
        new string[]{ // teste 2
            "O intuito deste teste e instruilo sobre as\ndiversas funcionalidades do sistema",
            "Apos esse treinamento voce tera um momento\npara usar o sistema como preferir",
            "Procure na arquibancada pela\nimagem com as instrucoes de uso"
        }
    };

    private int currentTestMessages = 0;

    void ShowTestMessages() {
        
        ActivateTestMessage();
        isShowingTestMessages = true;

        if (currentTestMessages == passDecisionTestMessages[actualPlayingTest -1].Length) { 
            allMessagesHasBeenShown = true;
            isShowingAMessage = false;
            isShowingTestMessages = false;
            currentTestMessages = 0;
            HideTestMessage();
        } //OLHA O ELSE IF...

        else {
            if(actualPlayingTest == 1) {
                ShowTestMessage(//"Treinamento de Movimentacao\n" +
                    $"{passDecisionTestMessages[actualPlayingTest - 1][currentTestMessages]}\n" +
                    $"-- Pressione A para continuar --");
                allMessagesHasBeenShown = false;
            }//OLHA O ELSE IF...
            
            else if (actualPlayingTest == 2) { 
                ShowTestMessage(//$"Cenario {currentTest1 + 1} de {totalTests1}\n" +
                $"APONTE SEGURANDO\no gatilho indicador do dedo esquerdo\n" +
                $"{passDecisionTestMessages[actualPlayingTest -1][currentTestMessages]}\n" +
                $"-- Pressione A para continuar --");
                allMessagesHasBeenShown = false;
            }//OLHA O ELSE IF...
            else if (actualPlayingTest == 3) {
                ShowTestMessage(//"Treinamento de Movimentacao 2\n" +
                    $"{passDecisionTestMessages[actualPlayingTest - 1][currentTestMessages]}\n" +
                    $"-- Pressione A para continuar --");
                allMessagesHasBeenShown = false;
            } 
            else if(actualPlayingTest == 4) {
                ShowTestMessage(//"Teste 2: Movimentacao\n" +
                    $"{passDecisionTestMessages[actualPlayingTest - 1][currentTestMessages]}\n" +
                    $"-- Pressione A para continuar --");
                allMessagesHasBeenShown = false;
            } 
            else if (actualPlayingTest == 5) {
                ShowTestMessage(//"Teste 2: Movimentacao\n" +
                    $"{passDecisionTestMessages[actualPlayingTest - 1][currentTestMessages]}\n" +
                    $"-- Pressione A para continuar --");
                allMessagesHasBeenShown = false;
            } else if (actualPlayingTest == 6) {
                ShowTestMessage(//"Teste 2: Movimentacao\n" +
                    $"{passDecisionTestMessages[actualPlayingTest - 1][currentTestMessages]}\n" +
                    $"-- Pressione A para continuar --");
                allMessagesHasBeenShown = false;
            } else if (actualPlayingTest == 7) {
                ShowTestMessage(//$"Treinamento: Comandos\n" +
                    $"{passDecisionTestMessages[actualPlayingTest - 1][currentTestMessages]}\n" +
                    $"-- Pressione A para continuar --");
                allMessagesHasBeenShown = false;
            }

        }
    }
    void ActivateTestMessage() {
        canvas.enabled = true;
        isShowingAMessage = true;
    }
    void HideTestMessage() {
        canvas.enabled = false;
        isShowingAMessage = false;
    }

}
