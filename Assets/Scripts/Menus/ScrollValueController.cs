using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class ScrollValueController : MonoBehaviour
{
    public Slider[] sliders; // Array para armazenar todas as sliders
    public Text[] valueTexts; // Array para armazenar os objetos de texto que exibirão os valores
    public OVRInput.Button startButton = OVRInput.Button.One; // Botão de início da tarefa
    public OVRInput.Button endButton = OVRInput.Button.Two; // Botão de fim da tarefa

    // Valor mínimo e máximo para as sliders
    private float stepSize = 10f;

    private bool taskStarted = false;
    private float taskStartTime = 0f;
    private List<float> sliderResponses; // Lista para armazenar as respostas dos sliders

    private void Awake()
    {
        sliderResponses = new List<float>();
    }

    // Método chamado quando qualquer slider for movida
    public void OnScrollValueChanged(int sliderIndex)
    {
        float sliderValue = sliders[sliderIndex].value;
        sliderValue = RoundToNearest(sliderValue, stepSize); 
        valueTexts[sliderIndex].text = sliderValue.ToString(); // Exibe o valor selecionado no objeto de texto correspondente

        // Atualiza o valor na lista sliderResponses somente quando a tarefa está em andamento
        if (taskStarted)
        {
            sliderResponses[sliderIndex] = sliderValue;
        }
    }

    private void Update()
    {
        // Verifica se o botão de início foi pressionado
        if (!taskStarted && OVRInput.GetDown(startButton))
        {
            taskStarted = true;
            taskStartTime = Time.time;
            Debug.Log("Tarefa iniciada!");
            sliderResponses.Clear(); // Limpa a lista de respostas dos sliders ao iniciar a tarefa
            foreach (Slider slider in sliders)
            {
                sliderResponses.Add(slider.value); // Armazena o valor inicial dos sliders como resposta
            }
        }

        // Verifica se o botão de fim foi pressionado
        if (taskStarted && OVRInput.GetDown(endButton))
        {
            taskStarted = false;
            SaveData(); // Salva as informações da tarefa
            Debug.Log("Tarefa encerrada!");
        }
    }

    // Método para salvar os dados da tarefa
    private void SaveData()
    {
        // Cria a linha de dados da tarefa com tempo de início, tempo final e respostas dos sliders
        float taskEndTime = Time.time;
        StringBuilder taskData = new StringBuilder();
        taskData.Append($"Tempo de Início:,{taskStartTime}");
        taskData.Append($",Tempo Final:,{taskEndTime}");

        // Adiciona as respostas dos sliders (em incrementos de 10 unidades) à linha de dados
        for (int i = 0; i < sliders.Length; i++)
        {
            float sliderValue = RoundToNearest(sliderResponses[i], stepSize);
            taskData.Append($",Resposta Slider {i+1}:,{sliderValue}");
        }

        // Escreve as informações no arquivo CSV
        string folderPath = Application.dataPath + "/Resultados";
        string fileName = "task_spot.csv";
        string filePath = Path.Combine(folderPath, fileName);

        // Verifica se a pasta "Resultados" já existe, senão cria a pasta
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Se o arquivo não existir, cria o cabeçalho do CSV
        if (!File.Exists(filePath))
        {
            string header = "Tarefa,Tempo de Início,Tempo Final";
            for (int i = 0; i < sliders.Length; i++)
            {
                header += $",Resposta Slider {i+1}";
            }
            File.WriteAllText(filePath, header + Environment.NewLine);
        }

        // Adiciona a linha de dados à lista de tarefas
        File.AppendAllText(filePath, taskData.ToString() + Environment.NewLine);
    }

    // Método para arredondar para o valor mais próximo com o incremento desejado
    private float RoundToNearest(float value, float step)
    {
        return Mathf.Round(value / step) * step;
    }
}
