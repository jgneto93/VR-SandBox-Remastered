using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TaskTimer : MonoBehaviour
{
    private float taskStartTime;
    private float taskEndTime;
    private bool isTiming;
    private int taskCount;

    private List<TaskData> taskDataList = new List<TaskData>();
    private string folderPath;
    private string fileNameBase = "task_visualization";
    private string fileExtension = ".csv";

    // Referência ao script MenuController.
    public MenuController menuController;

    void Start()
    {
        // Define o caminho da pasta "Resultados" dentro da pasta "Assets" do projeto.
        folderPath = Application.dataPath + "/Resultados";

        // Obtenha a referência ao script MenuController.
        menuController = FindObjectOfType<MenuController>();

        // Inicializa o cronômetro geral.
        StartGeneralTimer();
    }

    void Update()
    {
        // Verifica se o botão A do controle Oculus foi pressionado para iniciar ou finalizar uma tarefa.
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            if (isTiming)
            {
                // Se o cronômetro estiver em andamento e o botão A for pressionado, finaliza a tarefa atual.
                EndCurrentTask();
            }
            else
            {
                // Se o cronômetro não estiver em andamento e o botão A for pressionado, inicia uma nova tarefa.
                StartNewTask();
            }
        }

        // Verifica se o botão B do controle Oculus foi pressionado para finalizar a tarefa atual.
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            EndCurrentTask();
        }
    }

    void StartGeneralTimer()
    {
        isTiming = false;
        taskStartTime = 0f;
        taskEndTime = 0f;
        taskCount = 0;
    }

    void StartNewTask()
    {
        // Incrementa o contador de tarefas e define o nome da nova tarefa.
        taskCount++;
        string currentTaskName = "Tarefa" + taskCount;

        Debug.Log("Iniciando Tarefa: " + currentTaskName);

        // Define o início da tarefa atual.
        taskStartTime = Time.time;
        isTiming = true;
    }

    void EndCurrentTask()
    {
        if (!isTiming)
        {
            return;
        }

        isTiming = false;
        taskEndTime = Time.time;
        float totalTaskTime = taskEndTime - taskStartTime;

        string currentTaskName = "Tarefa" + taskCount;
        Debug.Log("Tarefa " + currentTaskName + " finalizada. Tempo total: " + totalTaskTime + " segundos");

        // Obtenha a resposta selecionada pelo usuário no menu. 
        string respostaSelecionada = menuController.GetSelectedOption();
        Debug.Log("Tarefa " + currentTaskName + ",Resposta," + respostaSelecionada);

        // Adiciona os dados da tarefa à lista de dados da tarefa.
        TaskData taskData = new TaskData(currentTaskName, respostaSelecionada, taskStartTime, taskEndTime);
        taskDataList.Add(taskData);

        // Limpa os tempos da tarefa atual.
        taskStartTime = 0f;
        taskEndTime = 0f;

        // Salva os dados das tarefas no arquivo CSV.
        SaveTaskDataToCSV();
    }

    void SaveTaskDataToCSV()
    {
        // Verifica se a pasta "Resultados" existe. Se não existir, cria a pasta.
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Monta o caminho completo do arquivo CSV.
        string fileName = fileNameBase + fileExtension;
        string fullPath = Path.Combine(folderPath, fileName);

        // Verifica se o arquivo já existe. Se existir, cria um novo arquivo com um número incremental no nome.
        int counter = 1;
        while (File.Exists(fullPath))
        {
            fileName = fileNameBase + counter.ToString() + fileExtension;
            fullPath = Path.Combine(folderPath, fileName);
            counter++;
        }

        // Cria uma lista de strings para conter todas as linhas do arquivo CSV.
        List<string> csvLines = new List<string>();

        // Adiciona o cabeçalho ao arquivo CSV.
        csvLines.Add("Tarefa,Resposta,Tempo de Início,Tempo de Fim,Tempo Total");

        // Adiciona os dados de cada tarefa à lista de linhas do CSV.
        foreach (TaskData taskData in taskDataList)
        {
            float totalTaskTime = taskData.endTime - taskData.startTime;
            string csvLine = taskData.taskName + "," + taskData.resposta + "," + taskData.startTime + "," + taskData.endTime + "," + totalTaskTime;
            csvLines.Add(csvLine);
        }

        // Grava os dados no arquivo CSV.
        File.WriteAllLines(fullPath, csvLines.ToArray());
    }
}

// Estrutura para armazenar os dados de cada tarefa.
public struct TaskData
{
    public string taskName;
    public string resposta;
    public float startTime;
    public float endTime;

    public TaskData(string name, string response, float start, float end)
    {
        taskName = name;
        resposta = response;
        startTime = start;
        endTime = end;
    }
}
