using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    private List<string> selectedHighOptions;
    private List<string> selectedLowOptions;
    private bool isHighSelection;
    private bool isLowSelection;
    private float taskStartTime;
    private float taskEndTime;

    private bool taskInProgress = false;

    public OVRInput.Button startButton = OVRInput.Button.One; // Botão de início da tarefa
    public OVRInput.Button endButton = OVRInput.Button.Two;   // Botão de fim da tarefa

    void Update()
    {
        // Verifica se a tarefa ainda não foi iniciada e o botão de início foi pressionado
        if (!taskInProgress && OVRInput.GetDown(startButton))
        {
            StartTask();
        }

        // Verifica se a tarefa está em andamento e o botão de fim foi pressionado
        if (taskInProgress && OVRInput.GetDown(endButton))
        {
            EndTask();
        }
    }

    public void StartTask()
    {
        taskInProgress = true;
        taskStartTime = Time.time;
        selectedHighOptions = new List<string>();
        selectedLowOptions = new List<string>();
        isHighSelection = false;
        isLowSelection = false;
        Debug.Log("Tarefa iniciada.");
    }

    public void EndTask()
    {
        if (!taskInProgress)
        {
            Debug.LogWarning("Nenhuma tarefa em andamento para encerrar.");
            return;
        }

        taskEndTime = Time.time;
        float taskTime = taskEndTime - taskStartTime;

        // Criar um objeto TaskResultData para armazenar os dados da tarefa
        TaskResultData taskData = new TaskResultData(taskStartTime, taskEndTime, taskTime, selectedHighOptions, selectedLowOptions);

        string csvFilePath = Application.dataPath + "/Resultados/tarefa_resultado.csv";
        SaveDataToCSV(taskData, csvFilePath);

        taskInProgress = false;
        Debug.Log("Tarefa finalizada. Resultados salvos em " + csvFilePath);
    }

    public void OnButtonClick(string buttonName)
    {
        // Lógica para o botão selecionado.
        Debug.Log("Botão " + buttonName + " foi selecionado.");

        if (!taskInProgress)
        {
            Debug.LogWarning("A tarefa ainda não foi iniciada.");
            return;
        }

        // Verifica se o botão High foi selecionado anteriormente
        if (isHighSelection)
        {
            AddSelectedHighOption(buttonName);
        }
        // Verifica se o botão Low foi selecionado anteriormente
        else if (isLowSelection)
        {
            AddSelectedLowOption(buttonName);
        }
    }

    public void OnButtonClickHigh()
    {
        // Define que as próximas respostas selecionadas serão do grupo "High"
        isHighSelection = true;
        isLowSelection = false;
        Debug.Log("Botão High ativado - selecione os 4 pontos mais altos");
    }

    public void OnButtonClickLow()
    {
        // Define que as próximas respostas selecionadas serão do grupo "Low"
        isHighSelection = false;
        isLowSelection = true;
        Debug.Log("Botão Low ativado - selecione os 4 pontos mais baixos");
    }

    private void AddSelectedHighOption(string option)
    {
        if (!selectedHighOptions.Contains(option))
        {
            selectedHighOptions.Add(option);

            if (selectedHighOptions.Count > 4)
            {
                selectedHighOptions.RemoveAt(0);
            }
        }
    }

    private void AddSelectedLowOption(string option)
    {
        if (!selectedLowOptions.Contains(option))
        {
            selectedLowOptions.Add(option);

            if (selectedLowOptions.Count > 4)
            {
                selectedLowOptions.RemoveAt(0);
            }
        }
    }

    private void SaveDataToCSV(TaskResultData data, string filePath)
    {
        using (StreamWriter file = new StreamWriter(filePath, true))
        {
            // Escreve os cabeçalhos
            file.WriteLine("Tempo de Início,Tempo Final,Tempo Total da Tarefa,Respostas High,Respostas Low");

            // Escreve os dados da tarefa
            file.WriteLine(data.startTime + "," + data.endTime + "," + data.taskTime + "," +
                           string.Join(",", data.highResponses.ToArray()) + "," +
                           string.Join(",", data.lowResponses.ToArray()));
        }
    }
}

[System.Serializable]
public class TaskResultData
{
    public float startTime;
    public float endTime;
    public float taskTime;
    public List<string> highResponses;
    public List<string> lowResponses;

    public TaskResultData(float startTime, float endTime, float taskTime, List<string> highResponses, List<string> lowResponses)
    {
        this.startTime = startTime;
        this.endTime = endTime;
        this.taskTime = taskTime;
        this.highResponses = highResponses;
        this.lowResponses = lowResponses;
    }
}
