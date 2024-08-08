using UnityEngine;
using System;

using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;

public class MatchInterpreterController : MonoBehaviour {
    public JSONReader jsonReader; // Reference to the JSONReader script
    public Camera mainCamera; // Reference to the main camera
   // private Transform rightController; // Reference to the right controller
    private Transform leftController; // Reference to the right controller
    private LineRenderer laserLineRenderer; // Reference to the LineRenderer component
    private bool isOnField = true;
    public Timeline timeline;
  
    public bool newVisualization = true;
    public RenderTexture CurrentRenderTexture;
    public RenderTexture NextRenderTexture;
    private bool isSlowMotion = false;
    int currentFrame = 0;
    
    private GameObject oculusInteractionSampleRig;
    private UserMovementLogger uML;
    private UserInputLogger uIL;
    private float timeSinceLastLog = 0f; // Variável para acumular o tempo
    private float timeSinceLineTrigger = 0f; // Variável para acumular o tempo

    private bool setCameraOnHead = false;
    GameObject referencia = null;
    private List<LineRenderer> lrList;
    OVRPlayerController playerController;
    private bool test1Setup = false;
    private bool test1Started = false;
    private bool test1Finished = false;
    void Start() {
        // rightController = GameObject.Find("OVRCameraRig/TrackingSpace/RightHandAnchor").transform;
        leftController = GameObject.Find("OVRCameraRig/TrackingSpace/LeftHandAnchor").transform;
        // Create a new GameObject for the laser line
        GameObject laserLineObject = new("LaserLine");
        // Add a LineRenderer component to the new GameObject
        laserLineRenderer = laserLineObject.AddComponent<LineRenderer>();
        // Configure the LineRenderer
        laserLineRenderer.startWidth = 0.01f; // Set the start width of the line
        laserLineRenderer.endWidth = 0.01f; // Set the end width of the line
        laserLineRenderer.material = new Material(Shader.Find("Unlit/Color")) {color = Color.red};
        laserLineRenderer.positionCount = 2; // Set the number of positions in the line
        oculusInteractionSampleRig = GameObject.Find("OculusInteractionSampleRig");
        lrList = jsonReader.GetLineRendererFromPlayerList();
        playerController = oculusInteractionSampleRig.GetComponent<OVRPlayerController>();

        uML = new();
        uIL = new();
    }

    void Update() {
        currentFrame = jsonReader.GetFrameIndex();

        timeSinceLastLog += Time.deltaTime;
        timeSinceLineTrigger += Time.deltaTime;

        if (test1Setup) {
            jsonReader.SetFrameIndex(1950);
            test1Setup = false; 
            test1Started = true;
            referencia = GameObject.Find("playerObject 3").GetComponentInChildren<Camera>().gameObject;
            SetCameraToPlayer(playerController, referencia);

        }
        if (test1Started && currentFrame < 2250 && !test1Finished) {
            Time.timeScale = .375f;
        }

        if (test1Finished && currentFrame >= 2300) {
            
            ResetTest();

        }

        // Verifica se um segundo se passou
        if (timeSinceLastLog >= 1f) {
            setCameraOnHead = true;
            uML.LogMovement(oculusInteractionSampleRig.transform);
            timeSinceLastLog = 0f; // Reseta o acumulador de tempo
        }
        
        if (playerController.enabled is false && setCameraOnHead == true) {
            SetCameraToPlayer(playerController, referencia);
            uIL.LogUserInput("Vendo o Ponto de Vista do Jogador");
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
            uIL.LogUserInput("Pause/Unpause");
            jsonReader.TogglePlay(); 
        }


        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger)) {
            if (timeSinceLineTrigger >= .175f) {
                Ray ray = new(leftController.position, leftController.forward);
                laserLineRenderer.SetPosition(0, leftController.position); // Set the start position of the laser line
                if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.parent.name != "Stadium") {
                    Transform parent = hit.transform.parent;
                    laserLineRenderer.SetPosition(1, hit.point); // Set the end position of the laser line to the hit point

                    jsonReader.SetPlayerHeatMap(parent.gameObject, true);
                    referencia = parent.GetComponentInChildren<Camera>().gameObject;

                    if (parent != null) {
                        Tracer tracer = parent.GetComponentInChildren<Tracer>();
                        if (tracer != null) {
                            LineRenderer lineRenderer = tracer.GetComponent<LineRenderer>();
                            if (lineRenderer != null) {
                                lineRenderer.enabled = !lineRenderer.enabled;
                                uIL.LogUserInput("Path do Jogador Ativado");

                            }
                        }

                        if (parent.GetComponentInChildren<Camera>().gameObject != null || hit.transform.parent.name is not "BallPrefab" || hit.transform.parent.name is not "Stadium") {
                            if (OVRInput.GetDown(OVRInput.Button.Four)) {
                                setCameraOnHead = true;
                                timeSinceLineTrigger = 0f; // Reseta o acumulador de tempo
                                SetCameraToPlayer(playerController, referencia);
                            }
                        }

                    } else {
                        Transform pathRenderer = hit.transform.Find("PathRenderer");
                        LineRenderer lineRenderer = pathRenderer.GetComponent<LineRenderer>();
                        if (lineRenderer != null) {
                            lineRenderer.enabled = !lineRenderer.enabled;
                            uIL.LogUserInput("Path da Bola Ativado");

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
            foreach(LineRenderer lr in lrList) {
                lr.enabled = false;
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.Start)) { // teleport para arquibancada / campo
            if(playerController.enabled == false) {
                playerController.enabled = true;
            }
            GameObject cameraRig = GameObject.Find("OVRCameraRig");
            Transform parent = cameraRig.transform.parent;
            isOnField = !isOnField;
            parent.GetComponent<OVRPlayerController>().enabled = false;
            CharacterController cc = parent.GetComponent<CharacterController>();
            cc.enabled = false;
            if (isOnField) {
                uIL.LogUserInput("TP campo");
                cameraRig.transform.parent.position = new Vector3(0, 1, 0);
            } else {
                uIL.LogUserInput("TP arquibancada");
                cameraRig.transform.parent.position = new Vector3(0, 17, -53);
            }
            cc.enabled = true;
            parent.GetComponent<OVRPlayerController>().enabled = true;
        }

        if (jsonReader.isPlaying) {
            jsonReader.InstantiatePlayersFromFrame(jsonReader.currentFrameIndex);
        }

        if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) &&
            OVRInput.Get(OVRInput.Button.Two) && OVRInput.Get(OVRInput.Button.Four)) {
            StartTest(oculusInteractionSampleRig);
        }

    }

    void SetCameraToPlayer(OVRPlayerController playerController, GameObject referencia) {
        oculusInteractionSampleRig.transform.position = referencia.transform.position;

        playerController.enabled = false;
        if (OVRInput.Get(OVRInput.Button.Start)) {
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

    void ResetTest() {


        uML.LogCustomLine("Teste Finalizado e Resetado");
        uIL.LogCustomLine("Teste Finalizado e Resetado");
        uML.CreateNewTestLogFile();
        uIL.CreateNewTestLogFile();
    }

    void StartTest(GameObject oculusInteractionSampleRig) {
        //TestDrill1(oculusInteractionSampleRig);
        test1Setup= true;
        uIL.LogCustomLine("Teste Iniciado por Comando");
    }

}

/*  void TestDrill1(GameObject oculusInteractionSampleRig) {
      OVRPlayerController playerController = oculusInteractionSampleRig.GetComponent<OVRPlayerController>();
      GameObject playerObject = GameObject.Find("playerObject 5"); // acha o alvo do teste
      jsonReader.SetFrameIndex(590);
      if (currentFrame >= 600 && testControllHasStarted == false) {
          playerController.enabled = false; // trava o usuário momentâneamente até ele se preparar
          testControllOnFreeze = true;
          Time.timeScale = 0.33f; // slow down to 50% speed
          isSlowMotion = true;
      }

      if (testControllOnFreeze) {
          oculusInteractionSampleRig.transform.position = playerObject.transform.position;

          if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x > 0.5f) {
              // Rotate the Camera Rig to the right
              oculusInteractionSampleRig.transform.Rotate(0, 10, 0);
          }
          if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x < -0.5f) {
              // Rotate the Camera Rig to the left
              oculusInteractionSampleRig.transform.Rotate(0, -10, 0);
          }
      }

      if (currentFrame >= 966 && testControllHasStarted == false) { // >=966 pq as vezes assincronismo acontece
          uIL.LogCustomLine("------- Teste Iniciado -------");
          // Tela inicial de teste
          jsonReader.TogglePlay(); //pausa a cena, pois assume-se que para chegar no frame 966, a cena ja esta rodando...
          testControllHasStarted = true; //para controle
          // playerObject.SetActive(false); // desativa o jogador que estava no lugar dele pra n dar problema de colisão
          oculusInteractionSampleRig.transform.position = playerObject.transform.position; // move o usuário pro jogador
          oculusInteractionSampleRig.transform.rotation = playerObject.transform.rotation;
          testControllOnFreeze = false;
      }

      if (OVRInput.GetDown(OVRInput.Button.One) && testControllisMoving == false) {
          jsonReader.TogglePlay();

          if (testControllHasStarted == true) {
              playerController.enabled = true; // libera o usuário para se mover
              playerObject.SetActive(false);

              uIL.LogCustomLine("Tempo de Análise Concluído");

              testControllisMoving = true;
              Time.timeScale = 1; // slow down to 50% speed
              isSlowMotion = false;
          }
      }

      if (testControllisMoving == true && testControllHasEnded == false) {

          if (testControllLastFrame != currentFrame) {
              uIL.LogCustomLine(jsonReader.GetFrameIndex() + "," + oculusInteractionSampleRig.transform.position + "," + playerObject.transform.position);
              testControllLastFrame = currentFrame;
          }
      }

      if (jsonReader.GetFrameIndex() >= 1100 && testControllHasEnded == false) {
          testControllHasEnded = true;
          testControllisMoving = false;
          jsonReader.TogglePlay();
          uIL.LogCustomLine("------- Teste Finalizado -------");
      }
  }
*/
