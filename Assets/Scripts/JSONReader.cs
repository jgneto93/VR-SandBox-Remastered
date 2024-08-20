using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class JSONReader : MonoBehaviour {
    public Timeline timeline; // slidebar 
    public GameObject ballPrefab; 
    public GameObject playerPrefab;
    public GameObject emptyGameObject;
    public int currentFrameIndex = 1010;
    //public bool useCustomFrameLimit;
    public int totalFrames; // Número total de quadros nos arquivos JSON.
    public List<Vector3> ballPositions;
    public List<List<Person>> peopleFramesList; //Uma lista  que contém todos os frames com todos os jogadores 
    public List<GameObject> playersList;
    public bool isPlaying = false;
    private List<GameObject> jointObjects = new List<GameObject>(); //  lista para rastrear os objetos dos joints
    Tracer ballTracer;
    SetColor setColor = new SetColor();
   
    [SerializeField] private HeatMapVisual heatMapVisual;
    private Grid grid;
    public int HeatMapStrideValue = 45;
    private GameObject cylinderTeam0;
    private GameObject cylinderTeam1;
    public List<TouchData> touchDataList = new List<TouchData>();
    private int currentEventIndex = 0;

    public Dictionary<int, int> jerseyNumberToIndexMap = new Dictionary<int, int>();

    public List<GameObject> shirtNumberObjects = new();

     void LoadAllJSONData() { // Old LoadAllJsonData
        TextAsset[] textFiles = Resources.LoadAll<TextAsset>("frames/");
        // Lista para armazenar os conteúdos JSON temporariamente
        List<string> jsonFiles = new();
        foreach (TextAsset textFile in textFiles) {
            string jsonContent = textFile.text;
            jsonFiles.Add(jsonContent);
        }

        totalFrames = jsonFiles.Count;

        ballPositions = new List<Vector3>(totalFrames);
        peopleFramesList = new List<List<Person>>(totalFrames);
        
        Vector3 lastKnownBallPosition = Vector3.zero; // Para armazenar a última posição conhecida

        for (int i = 0; i < totalFrames; i++) {
            string jsonFile = jsonFiles[i];
            JsonData jsonData = JsonConvert.DeserializeObject<JsonData>(jsonFile);

            if (jsonData.ballPosition != null)
                ballPositions.Add(new Vector3(jsonData.ballPosition[0], jsonData.ballPosition[2], jsonData.ballPosition[1]));
            else
                ballPositions.Add(Vector3.zero); // mudar para pegar a posição anterior ao invés de 0

            List<Person> peopleFrame = new List<Person>();
            if (jsonData.playerTracking != null && jsonData.playerTracking.people != null) {
                foreach (var personData in jsonData.playerTracking.people) {
                    Person person = new Person {
                        centerOfMass = personData.centerOfMass,
                        heightOfGround = personData.heightOfGround,
                        jerseyNumber = personData.jerseyNumber,
                        roleId = personData.roleId,
                        teamId = personData.teamId,
                        trackId = personData.trackId
                    };
                    person.jointIds = personData.jointIds;
                    person.positions = personData.positions;
                    peopleFrame.Add(person);
                }
            }
            peopleFramesList.Add(peopleFrame);
        }
        for (int j = 0; j < totalFrames; j++) {
            Vector3 ballPosition = ballPositions[j];
            List<Person> peopleFrame = peopleFramesList[j]; // Get the corresponding frame
            foreach (Person personData in peopleFrame) { // Loop through each person in the frame    
                for (int i = 0; i < personData.positions.Count; i += 3) {
                    Vector3 jointPosition = new Vector3(personData.positions[i], personData.positions[i + 2], personData.positions[i + 1]); // Adjust the axes as needed
                    if (IsColliding(jointPosition, ballPosition, 0.2f)) { // Check if the joint position collides with the ball position
                        TouchData touchData = new TouchData {
                            frameId = j,
                            playerId = personData.jerseyNumber, // Assuming trackId is the unique identifier for a player
                            playerTeamId = personData.teamId,
                            playerRoleId = personData.roleId,
                            playerPosition = jointPosition
                        };
                        touchDataList.Add(touchData);
                    }
                }
            }
        }
    }

    void Start() {
        ballTracer = ballPrefab.GetComponentInChildren<Tracer>();
        ballTracer.lineColor = Color.yellow;

        cylinderTeam0 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinderTeam0.transform.localScale = new Vector3(0.1f, 1f, 0.1f);
        cylinderTeam1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinderTeam1.transform.localScale = new Vector3(0.1f, 1f, 0.1f);

        offsideLineTeam0 = Instantiate(offsideLineTeam0);
        offsideLineTeam1 = Instantiate(offsideLineTeam1);

        grid = new Grid(54 *2, 34 *2, 1f, Vector3.zero, Quaternion.Euler(90,-90,-90)); // Field's dimention
        heatMapVisual.SetGrid(grid);

        LoadAllJSONData();
        //ProcessAllJSONData();
        InstantiatePlayersFromFrame(0);
    }
    void FixedUpdate() {
        if (isPlaying) {
            currentFrameIndex++;
            InstantiatePlayersFromFrame(currentFrameIndex);
            Vector3 ballPosition = ballPositions[currentFrameIndex];
            ballPrefab.transform.position = ballPosition;
            ballTracer.AddVector3ToLineRenderer(ballPosition);  
            UpdateOffsideLine(currentFrameIndex);
            timeline.SetSliderValue(currentFrameIndex);
            if (currentFrameIndex >= totalFrames)
                isPlaying = false;
        }
    }

    public void TogglePlay() {
        isPlaying = !isPlaying;
    }
    public int GetFrameIndex(){
        return currentFrameIndex;
    }

    public List<GameObject> GetPlayerList() {
        return playersList;
    }

    public List<LineRenderer> GetLineRendererFromPlayerList() {
        playersList = GetPlayerList();
        // Inicializa a lista de LineRenderer
        List<LineRenderer> lineRendererList = new List<LineRenderer>();

        foreach (GameObject player in playersList) {
            // Obtém todos os componentes LineRenderer no GameObject e seus filhos
            LineRenderer[] lineRenderers = player.GetComponentsInChildren<LineRenderer>();
            // Filtra para obter apenas os LineRenderers que não são do GameObject pai
            foreach (LineRenderer lr in lineRenderers) {
                if (lr.gameObject != player) { // Verifica se o LineRenderer não está no GameObject pai
                    lineRendererList.Add(lr);
                    break; // Supondo que você queira apenas o primeiro LineRenderer encontrado que não seja do pai
                }
            }
        }

        return lineRendererList;
    }

    public void SetFrameIndex(int frame){ //Used in Timeline
        currentFrameIndex = frame;
    }
    public int GetTotalFrames(){ //Used in Timeline
        return totalFrames;
    }
    void SimplifyTouchDataList() {
        for (int i = 0; i < touchDataList.Count - 1; i++) {
            if ((touchDataList[i].frameId == touchDataList[i + 1].frameId &&
                touchDataList[i].playerId == touchDataList[i + 1].playerId &&
                touchDataList[i].playerTeamId == touchDataList[i + 1].playerTeamId) ||
                (touchDataList[i + 1].frameId == touchDataList[i].frameId + 1 &&
                touchDataList[i].playerId == touchDataList[i + 1].playerId &&
                touchDataList[i].playerTeamId == touchDataList[i + 1].playerTeamId)) {
                touchDataList.RemoveAt(i);
                i--; // Decrement i so the next element is compared with the current one
            }
        }
    }
    // 0 = default, 5 = progression, 1 = pass_start, 2 pass_end, 3 = finish, 4 = tackle 
    void DefineEvents(){
        for(int i = 0; i< touchDataList.Count -1; i++){
            if(touchDataList[i].eventType == 0){
                if(touchDataList[i].playerId == touchDataList[i+1].playerId && touchDataList[i].playerTeamId == touchDataList[i+1].playerTeamId){
                    touchDataList[i].eventType = 5;
                }
                if(touchDataList[i].playerId != touchDataList[i+1].playerId && touchDataList[i].playerTeamId == touchDataList[i+1].playerTeamId){
                    touchDataList[i].eventType = 1;
                    timeline.AddEventIconAboveSlider(touchDataList[i].frameId, 1, 2.0f);
                    touchDataList[i+1].eventType = 2;
                }
                if(touchDataList[i].playerId != touchDataList[i+1].playerId && touchDataList[i].playerTeamId != touchDataList[i+1].playerTeamId && touchDataList[i].playerRoleId == 1 && touchDataList[i+1].playerRoleId == 2){
                    bool outOfPlay = false;
                    for(int j = touchDataList[i+1].frameId; touchDataList[i].frameId < j; j--){
                        if(ballPositions[j].x > 75 || ballPositions[j].x < -75){
                            outOfPlay = true;
                            break;
                        }
                    }
                    if(outOfPlay == true){
                        touchDataList[i].eventType = 4;
                    }
                }
            }
        }
    }
 
    bool IsColliding(Vector3 jointPosition, Vector3 ballPosition, float radius) {
        float distance = Vector3.Distance(jointPosition, ballPosition);
        return distance <= radius * 2; // If the distance is less than or equal to the sum of the radii, they are colliding
    }

    public void InstantiatePlayersFromFrame(int frameIndex) {
        playerPrefab = emptyGameObject;
        List<int> connections = new List<int> { 1, 16, 0, 17, 1, 0, 16, 17, 0, 1, 5, 6, 7, 6, 5, 1, 2, 3, 4, 3, 2, 1, 11, 12, 13, 12, 11, 8, 9, 10, 9, 8, 1 };
        List<Person> framePeople = peopleFramesList[frameIndex];

        for (int playerIndex = 0; playerIndex < framePeople.Count; playerIndex++) {
            Person personData = framePeople[playerIndex];
            GameObject playerObject = playersList.Count > playerIndex ? playersList[playerIndex] : null;

            if (playerObject == null) {

                playerObject = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                playerObject.name = "playerObject " + personData.jerseyNumber;

                jerseyNumberToIndexMap[personData.jerseyNumber] = playerIndex;

                playersList.Add(playerObject);
                jointObjects.Add(playerObject);

                // Create a new GameObject for the tracer and add it as a child of the playerObject
                GameObject tracerObject = new GameObject("PathRenderer");
                tracerObject.transform.SetParent(playerObject.transform);
                Tracer tracer = tracerObject.AddComponent<Tracer>();
                // Set the color of the tracer
                tracer.lineColor = setColor.GetColor(personData.roleId, personData.teamId, playerIndex);

                GameObject tshirtNumber = new GameObject("ShirtNumber");
                tshirtNumber.transform.SetParent(playerObject.transform);
                TextMesh number = tshirtNumber.AddComponent<TextMesh>();
                number.text = personData.jerseyNumber.ToString();
                number.fontSize = 8;
                number.color = setColor.GetColor(personData.roleId, personData.teamId);
                shirtNumberObjects.Add(tshirtNumber);
            }

            Vector3 position = new Vector3(personData.centerOfMass[0], personData.centerOfMass[2], personData.centerOfMass[1]);
            UpdateOrInstantiateCircleAtPlayerPosition(playerObject, position, setColor.GetColor(personData.roleId, personData.teamId));

            Tracer playerTracer = playerObject.transform.Find("PathRenderer").GetComponentInChildren<Tracer>();
            playerTracer.AddVector3ToLineRenderer(position);

            Transform shirtNumberTransform = playerObject.transform.Find("ShirtNumber");
            shirtNumberTransform.position = position + new Vector3(0, 2, 0);

            for (int i = 0; i < personData.positions.Count; i += 3) {
                Vector3 jointPosition = new Vector3(personData.positions[i], personData.positions[i + 2], personData.positions[i + 1]); // Adjust the axes as needed
                GameObject jointObject = playerObject.transform.Find("JointObject_" + i / 3)?.gameObject;
                if (jointObject == null) {
                    jointObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    jointObject.transform.localScale = new Vector3(0.075f, 0.075f, 0.075f); // Sphere size
                    jointObject.transform.position = jointPosition;
                    jointObject.name = "JointObject_" + i / 3;
                    jointObject.transform.parent = playerObject.transform;

                    // Adiciona um Rigidbody
                    Rigidbody jointRigidbody = jointObject.AddComponent<Rigidbody>();
                    jointRigidbody.isKinematic = true;

                    SphereCollider sphereCollider = jointObject.AddComponent<SphereCollider>();
                    sphereCollider.radius = 1.0f;
                    jointObject.tag = "Joint";
                    // Set color using the GetColor function
                    setColor.SetObjectColor(jointObject, setColor.GetColor(personData.roleId, personData.teamId, playerIndex));
                } else {
                    jointObject.transform.position = jointPosition;
                }
            }
            UpdateCameras(playerObject, personData);
            
            
            
            LineRenderer lineRenderer = playerObject.GetComponent<LineRenderer>();
            if (lineRenderer == null) {
                lineRenderer = playerObject.AddComponent<LineRenderer>();
                lineRenderer.startWidth = 0.05f;
                lineRenderer.endWidth = 0.05f;
            }
            lineRenderer.positionCount = 0;
            // Set the position count before the loop
            lineRenderer.positionCount = connections.Count;



            for (int connectionIndex = 0; connectionIndex < connections.Count; connectionIndex++) {
                int jointIndex = connections[connectionIndex];
                Transform jointTransform = playerObject.transform.Find("JointObject_" + jointIndex);
                GameObject jointObject = jointTransform.gameObject;
                lineRenderer.SetPosition(connectionIndex, jointObject.transform.position);
                setColor.SetObjectColor(lineRenderer, setColor.GetColor(personData.roleId, personData.teamId));
            }

            for (int i = connections.Count; i < lineRenderer.positionCount; i++) {
                lineRenderer.SetPosition(i, playerObject.transform.position);
            }

        }
    
    }
       
    public int GetPlayerCount() {
        return playersList.Count;
    }
    public GameObject GetPlayer(int jerseyNumber) {
        if (jerseyNumberToIndexMap.TryGetValue(jerseyNumber, out int playerIndex)) {
            // Verifica se o índice está dentro do intervalo da lista
            if (playerIndex >= 0 && playerIndex < playersList.Count) {
                return playersList[playerIndex];
            }
        }
        Debug.LogError($"Jogador com o jerseyNumber {jerseyNumber} não encontrado.");
        return null;
    }

    public Vector3 GetPlayerTrasnformFromId(int playerId) {
        Debug.Log(playerId);
        return playersList[playerId].transform.position;
    }

    public Quaternion GetPlayerRotationFromId(int playerId) {
        Debug.Log(playerId);
        return playersList[playerId].transform.rotation;
    }

    void UpdateCameras(GameObject playerObject, Person personData) {
        Camera playerCamera = playerObject.GetComponentInChildren<Camera>();
        Vector3 nariz = new Vector3(personData.positions[0], personData.positions[2], personData.positions[1]);
        Vector3 neck = new Vector3(personData.positions[3], personData.positions[2], personData.positions[4]);
        nariz.y = playerObject.transform.position.y;
        if (playerCamera is not null) {
            playerCamera.targetDisplay = 4;
            playerCamera.transform.position = neck;
            playerCamera.transform.LookAt(nariz);
        }
        playerObject.transform.LookAt(nariz);
    }

    public Person GetPersonFromPlayerObject(GameObject playerObject, int frameIndex) {
        string[] splitName = playerObject.name.Split(' ');

        if (splitName.Length < 2) {
            Debug.LogError("Invalid playerObject name: " + playerObject.name);
            return null;
        }
        if (!int.TryParse(splitName[1], out int jerseyNumber)) {
            Debug.LogError("Invalid jerseyNumber in playerObject name: " + playerObject.name);
            return null;
        }

        // Busca pela lista de pessoas no frame especificado pelo JerseyNumber
        Person personData = peopleFramesList[frameIndex].FirstOrDefault(person => person.jerseyNumber == jerseyNumber);

        if (personData == null) {
            Debug.LogError("Person data not found for jerseyNumber: " + jerseyNumber);
        }

        return personData;
    }

    public void UpdateShirtNumberRotation(Transform user) {
        foreach (GameObject shirt in shirtNumberObjects) {
            shirt.transform.LookAt(user.position);
            shirt.transform.Rotate(0, 180, 0); 
        }
    }

    private void UpdateOrInstantiateCircleAtPlayerPosition(GameObject playerObject, Vector3 playerPosition, Color color) {
        // Try to find the existing circle
        GameObject circleObject = playerObject.transform.Find("PlayerPositionCircle")?.gameObject;

        // If the circle does not exist, create a new one
        if (circleObject == null) {
            circleObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            circleObject.transform.localScale = new Vector3(1f, 0.01f, 1f);
            circleObject.transform.SetParent(playerObject.transform);
            circleObject.name = "PlayerPositionCircle";
            circleObject.GetComponent<Renderer>().material.color = color;
        }
        // Update the position of the circle
        playerPosition.y = 0.1f;
        Vector3 circlePosition = new Vector3(playerPosition.x, 0, playerPosition.z);
        circleObject.transform.position = circlePosition;
    }

    private Vector3 FindClosestLegalLimb(Person personData, int teamId) {
        Vector3 closestLimb = Vector3.zero; // Inicialização padrão, pode precisar de ajuste
        bool hasValidLimb = false; // Para verificar se encontramos um limb válido

        for (int i = 0; i < personData.positions.Count; i += 3) {
            if (i == 9 || i == 12 || i == 18 || i == 21) {
                continue;
            }
            Vector3 jointPosition = new Vector3(personData.positions[i], personData.positions[i + 2], personData.positions[i + 1]);
            if (!hasValidLimb) {
                closestLimb = jointPosition; // Inicializa com o primeiro limb válido
                hasValidLimb = true;
            } else if ((teamId == 0 && jointPosition.x > closestLimb.x) || (teamId == 1 && jointPosition.x < closestLimb.x)) {
                closestLimb = jointPosition;
            }
        }
        return closestLimb; // Retorna o limb mais atrasado com base no teamId
    }


    public GameObject offsideLineTeam0;
    public GameObject offsideLineTeam1;

    List<(int, Person)> teamPlayers0 = new List<(int, Person)>();
    List<(int, Person)> teamPlayers1 = new List<(int, Person)>();

    public void UpdateOffsideLine(int currentFrameIndex) {

        teamPlayers0.Clear();
        teamPlayers1.Clear();
        // 1. Dividir os jogadores pelos times e coletar suas posições
        foreach (Person personData in peopleFramesList[currentFrameIndex]) {
            // Supondo que centerOfMass seja [X, Z, Y]
            Vector3 playerPosition = new Vector3(personData.centerOfMass[0], personData.centerOfMass[2], personData.centerOfMass[1]);

            if (personData.teamId == 0)
                teamPlayers0.Add((personData.jerseyNumber, personData));
            else if (personData.teamId == 1)
                teamPlayers1.Add((personData.jerseyNumber, personData));
        }

        // 2. Ordenar as listas pela posição X do centro de massa
        teamPlayers0.Sort((p1, p2) => p1.Item2.centerOfMass[0].CompareTo(p2.Item2.centerOfMass[0]));
        teamPlayers1.Sort((p1, p2) => p1.Item2.centerOfMass[0].CompareTo(p2.Item2.centerOfMass[0]));

        // 3. Encontrar o penúltimo jogador do time 0 e o segundo jogador do time 1
        var secondLastPlayer0 = teamPlayers0[teamPlayers0.Count - 2];
        var secondPlayer1 = teamPlayers1[1];

        // 4. Encontrar o membro legal mais atrasado para cada um desses jogadores
        Vector3 closestLimb0 = FindClosestLegalLimb(secondLastPlayer0.Item2, 0); // Passa o objeto Person diretamente
        Vector3 closestLimb1 = FindClosestLegalLimb(secondPlayer1.Item2, 1); // Passa o objeto Person diretamente



        // Atualiza a posição dos cilindros para a posição dos closestLimb correspondentes
        cylinderTeam0.transform.position = new Vector3(closestLimb0.x, closestLimb0.y / 2, closestLimb0.z);
        cylinderTeam1.transform.position = new Vector3(closestLimb1.x, closestLimb1.y / 2, closestLimb1.z);
        cylinderTeam0.transform.localScale = new Vector3(cylinderTeam0.transform.localScale.x, closestLimb0.y / 2, cylinderTeam0.transform.localScale.z);
        cylinderTeam1.transform.localScale = new Vector3(cylinderTeam1.transform.localScale.x, closestLimb1.y / 2, cylinderTeam1.transform.localScale.z);

        offsideLineTeam0.transform.position = new Vector3(closestLimb0.x, .0005f, offsideLineTeam0.transform.position.z);
        offsideLineTeam1.transform.position = new Vector3(closestLimb1.x, .0005f, offsideLineTeam1.transform.position.z);

        // Cores para cada time e para a linha padrão
        Color team0Color = Color.red;
        Color team1Color = Color.blue;
        Color defaultLineColor = Color.white;

        // Verifica se há um jogador do time 1 à frente do jogador mais distante do time 0
        if (offsideLineTeam0.transform.position.x < teamPlayers1.Last().Item2.centerOfMass[0]) {
            // Muda a cor da linha e do cilindro do time 0 para a cor do time 1
            offsideLineTeam0.GetComponent<Renderer>().material.color = team1Color;
            cylinderTeam0.GetComponent<Renderer>().material.color = team1Color;
        } else {
            // Caso contrário, muda a cor da linha e do cilindro do time 0 para a cor padrão
            offsideLineTeam0.GetComponent<Renderer>().material.color = defaultLineColor;
            cylinderTeam0.GetComponent<Renderer>().material.color = defaultLineColor;
        }

        // Verifica se há um jogador do time 0 à frente do jogador mais distante do time 1
        if (offsideLineTeam1.transform.position.x > teamPlayers0.First().Item2.centerOfMass[0]) {
            // Muda a cor da linha e do cilindro do time 1 para a cor do time 0
            offsideLineTeam1.GetComponent<Renderer>().material.color = team0Color;
            cylinderTeam1.GetComponent<Renderer>().material.color = team0Color;
        } else {
            // Caso contrário, muda a cor da linha e do cilindro do time 1 para a cor padrão
            offsideLineTeam1.GetComponent<Renderer>().material.color = defaultLineColor;
            cylinderTeam1.GetComponent<Renderer>().material.color = defaultLineColor;
        }
    }
    

    public void SetPlayerHeatMap(GameObject playerObject = null, bool clearGrid = true){
        if (clearGrid == true){
            grid.ClearGrid();
        }
       
        if(playerObject is null) {
            return;
        }

        if(playerObject is not null || playerObject.name is not "Stadium" || playerObject.transform.parent.name is not "Stadium"){
            for(int i = 0; i <= totalFrames - HeatMapStrideValue; i += HeatMapStrideValue){
                Person personData = GetPersonFromPlayerObject(playerObject, i);
                grid.AddValue(new Vector3(personData.centerOfMass[0], personData.centerOfMass[2], personData.centerOfMass[1]), 5, 2, 5);
            }
        }
    }

    public TouchData GetNextEventClosestToFrame(int frameIndex, int eventNumber) {
        TouchData nextEvent = null;
        int eventCount = 0;

        for (int i = currentEventIndex; i < touchDataList.Count; i++) {
            TouchData touchData = touchDataList[i];
            if (touchData.frameId > frameIndex) {
                if (eventCount == eventNumber) {
                    nextEvent = touchData;
                    currentEventIndex = i; // Atualiza o índice do evento atual
                    break;
                }
                eventCount++;
            }
        }

        return nextEvent;
    }
    public TouchData GetPreviousEventClosestToFrame(int frameIndex, int eventNumber) {
        TouchData previousEvent = null;
        int eventCount = 0;
        currentEventIndex = touchDataList.Count();
        for (int i = currentEventIndex -1; i >= 0; i--) {
            TouchData touchData = touchDataList[i];
            if (touchData.frameId < frameIndex) {
                if (eventCount == eventNumber) {
                    previousEvent = touchData;
                    currentEventIndex = i; // Atualiza o índice do evento atual
                    break;
                }
                eventCount++;
            }
        }

        return previousEvent;
    }

    /*
      void LoadAllJSONData() { // Old LoadAllJsonData
        string[] jsonFiles = Directory.GetFiles(jsonFolderPath, "*.json");
        if (!useCustomFrameLimit) { 
            totalFrames = jsonFiles.Length;
        }
        ballPositions = new List<Vector3>(totalFrames);
        peopleFramesList = new List<List<Person>>(totalFrames);

        for (int i = 0; i < totalFrames; i++) {
            string jsonFile = jsonFiles[i];
            string jsonContent = File.ReadAllText(jsonFile);
            JsonData jsonData = JsonConvert.DeserializeObject<JsonData>(jsonContent);

            if (jsonData.ballPosition != null) 
                ballPositions.Add(new Vector3(jsonData.ballPosition[0], jsonData.ballPosition[2], jsonData.ballPosition[1]));
            else 
                ballPositions.Add(Vector3.zero); // mudar para pegar a posição anterior ao invés de 0

            List<Person> peopleFrame = new List<Person>();
            if (jsonData.playerTracking != null && jsonData.playerTracking.people != null) {
                foreach (var personData in jsonData.playerTracking.people) {
                    Person person = new Person {
                        centerOfMass = personData.centerOfMass,
                        heightOfGround = personData.heightOfGround,
                        jerseyNumber = personData.jerseyNumber,
                        roleId = personData.roleId,
                        teamId = personData.teamId,
                        trackId = personData.trackId
                    };
                    person.jointIds = personData.jointIds;
                    person.positions = personData.positions;
                    peopleFrame.Add(person);
                }
            }
            peopleFramesList.Add(peopleFrame);
            
        }
    }

    /*  void ProcessAllJSONData(){ // Called in Start()
    
        SimplifyTouchDataList();
        DefineEvents();
        //CreatePassMap();
    }
*/

    /*

     void CreatePassMap(){
         Dictionary<int, int> playerIdToIndex = new Dictionary<int, int>();
         Dictionary<int, Vector3> playerPositionsSum = new Dictionary<int, Vector3>();
         Dictionary<int, int> playerPositionsCount = new Dictionary<int, int>();
         List<int[]> passMap = new List<int[]>();
         // Get the unique player IDs with eventType == 1
         foreach (var touchData in touchDataList){
             if (touchData.eventType == 1 && !playerIdToIndex.ContainsKey(touchData.playerId)){
                 playerIdToIndex.Add(touchData.playerId, playerIdToIndex.Count);
                 playerPositionsSum[touchData.playerId] = Vector3.zero;
                 playerPositionsCount[touchData.playerId] = 0;
             }
             if (playerIdToIndex.ContainsKey(touchData.playerId)){
                 playerPositionsSum[touchData.playerId] += touchData.playerPosition;
                 playerPositionsCount[touchData.playerId]++;
             }
         }
         Dictionary<int, Vector3> playerAveragePositions = new Dictionary<int, Vector3>();
         foreach (var playerId in playerIdToIndex.Keys){
             playerAveragePositions[playerId] = playerPositionsSum[playerId] / playerPositionsCount[playerId];
         }
         int totalPeople = playerIdToIndex.Count;
         // For each unique player ID...
         foreach (var playerId in playerIdToIndex.Keys){
             // Initialize a new array for this player
             int[] playerPassMap = new int[totalPeople];
             // For each touchData...
             for (int j = 0; j < touchDataList.Count - 1; j++){
                 if (touchDataList[j].eventType == 1 && touchDataList[j + 1].eventType == 2 && touchDataList[j].playerId == playerId){
                     int playerYIndex;
                     if (playerIdToIndex.TryGetValue(touchDataList[j + 1].playerId, out playerYIndex)){
                         playerPassMap[playerYIndex]++;
                     }
                 }
             }
             passMap.Add(playerPassMap);
         }
         GeneratePassGraph(playerIdToIndex, passMap, playerAveragePositions, touchDataList);
         PlotPassMap();
     }
    */
    /*
    void PlotPassMap(){

        GameObject passPlot = new GameObject();
        for (int i = 0; i < touchDataList.Count - 1; i++) {
            if (touchDataList[i].eventType == 1 && touchDataList[i+1].eventType == 2) {
                GameObject arrow = DrawArrow(touchDataList[i], touchDataList[i + 1]);
                arrow.transform.parent = passPlot.transform;
                i += 2;
            }
        }
        passPlot.SetActive(false);
    }

    public GameObject DrawArrow(TouchData start, TouchData end) {
        // Cria um cilindro para representar a seta
        GameObject arrow = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Vector3 ballStartPos = ballPositions[start.frameId];
        Vector3 ballEndPos = ballPositions[end.frameId];

        float arrowLength = Vector3.Distance(ballStartPos, ballEndPos);
        arrow.transform.localScale = new Vector3(0.1f, arrowLength / 2, 0.1f); // Ajusta o tamanho do cilindro com base na distância entre start e end
        arrow.transform.position = (ballStartPos + ballEndPos) / 2;
        arrow.transform.up = ballEndPos - ballStartPos;

        // Cria dois cilindros menores para representar a ponta da seta
        float tipLength = arrowLength * 0.1f; // Faz o tamanho da ponta ser 10% do tamanho da seta
        Vector3 tipPosition = ballEndPos - (arrow.transform.up * tipLength / 2); // Move a ponta para trás ao longo da direção da seta

        GameObject arrowTip1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        arrowTip1.transform.localScale = new Vector3(0.1f, tipLength, 0.1f);
        arrowTip1.transform.position = tipPosition;
        arrowTip1.transform.up = Quaternion.Euler(0, -45, 0) * (ballEndPos - ballStartPos);

        GameObject arrowTip2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        arrowTip2.transform.localScale = new Vector3(0.05f, tipLength, 0.05f);
        arrowTip2.transform.position = tipPosition;
        arrowTip2.transform.up = Quaternion.Euler(0, 45, 0) * (ballEndPos - ballStartPos);

        Color color = setColor.GetColor(start.playerRoleId, start.playerTeamId, start.playerId);
        // Define o material da seta com base nas informações do jogador de início
        Renderer arrowRenderer = arrow.GetComponent<Renderer>();
        arrowRenderer.material.color = color;

        Renderer arrowTip1Renderer = arrowTip1.GetComponent<Renderer>();
        arrowTip1Renderer.material.color = color;

        Renderer arrowTip2Renderer = arrowTip2.GetComponent<Renderer>();
        arrowTip2Renderer.material.color = color;

        arrowTip2.transform.parent = arrow.transform;
        arrowTip1.transform.parent = arrow.transform;
        arrow.SetActive(false);
        return arrow;
    }


    void GeneratePassGraph(Dictionary<int, int> playerIdToIndex, List<int[]> passMap, Dictionary<int, Vector3> playerAveragePositions, List<TouchData> touchDataList){
        if (playerIdToIndex == null || passMap == null || playerAveragePositions == null || touchDataList == null) return;

        GameObject passGraph = new GameObject("PassGraph");
        for(int i = 0; i < passMap.Count; i++){
            for(int j = 0; j < passMap[i].Length; j++){
                if(passMap[i][j] > 0){
                    // Create a new GameObject for each line
                    GameObject lineObj = new GameObject("Line_" + i + "_" + j);
                    lineObj.transform.parent = passGraph.transform;

                    // Add a LineRenderer to the GameObject
                    LineRenderer line = lineObj.AddComponent<LineRenderer>();

                    // Set the width of the line
                    line.startWidth = passMap[i][j] / 2f;
                    line.endWidth = passMap[i][j] / 2f;

                    // Set the number of points to 2
                    line.positionCount = 2;

                    // Set the start and end points of the line
                    int playerId1 = playerIdToIndex.FirstOrDefault(x => x.Value == i).Key;
                    int playerId2 = playerIdToIndex.FirstOrDefault(x => x.Value == j).Key;

                    // Get the average positions for the players
                    Vector3 player1Position = playerAveragePositions[playerId1];
                    Vector3 player2Position = playerAveragePositions[playerId2];

                    line.SetPosition(0, player1Position);
                    line.SetPosition(1, player2Position);

                    // Get the teamId and roleId from the TouchData for the players
                    TouchData player1Data = touchDataList.FirstOrDefault(x => x.playerId == playerId1);
                    TouchData player2Data = touchDataList.FirstOrDefault(x => x.playerId == playerId2);

                    if (player1Data != null && player2Data != null){
                        // Set the color of the line using the GetColor function
                        line.material.color = setColor.GetColor(player1Data.playerRoleId, player1Data.playerTeamId, player1Data.playerId);
                    }
                }
            }
        }
        passGraph.SetActive(false);
    }

*/



}
