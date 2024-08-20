using System;
using System.IO;
using UnityEngine;

public class UserMovementLogger : MonoBehaviour {
    private string logFilePath;
    private Quaternion lastTransform;
    private float timeSinceLastFileCreation = 1f;
    private string lastInput = "";
    public UserMovementLogger() {
        CreateNewTestLogFile();
        timeSinceLastFileCreation = 0f;
    }

    void Update() {
        // Atualiza o tempo decorrido
        timeSinceLastFileCreation += Time.deltaTime;
    }

    public void LogMovement(Transform transform) {
        if (lastTransform == null) {
            lastTransform = transform.rotation;
        }
        if (lastTransform != transform.rotation) {
            string logEntry = $"{Time.time}, {transform.position}, {transform.rotation}";
            File.AppendAllText(logFilePath, logEntry + "\n");
        }
        lastTransform = transform.rotation;
    }

    public void LogCustomLine(string line) {
        if(line != lastInput) { 
            string logEntry = $"{Time.time}: {line}";
            File.AppendAllText(logFilePath, logEntry + "\n");
            lastInput = line; 
        }
    }

    public void CreateNewTestLogFile() {
        try {
            // Conta todos os arquivos na pasta
            string[] files = Directory.GetFiles(Application.persistentDataPath);
            int fileCount = files.Length;

            // Subtrai 1 (arquivo de controle) e divide por 2 (arquivos do UserInputLogger)
            int logFileIndex = ((fileCount) / 2) + 1;

            // Cria o novo arquivo de log
            string newLogFilePath = Path.Combine(Application.persistentDataPath, $"UserMovementLog{logFileIndex}.txt");
            File.Create(newLogFilePath).Dispose();

            // Atualiza o logFilePath com o novo caminho
            logFilePath = newLogFilePath;

            // Reseta o tempo desde a última criação de arquivo
            timeSinceLastFileCreation = 0f;

            Debug.Log($"Novo arquivo de log criado: {newLogFilePath}");
        } catch (UnauthorizedAccessException e) {
            Debug.LogError($"Acesso negado ao caminho: {Application.persistentDataPath}. Detalhes: {e.Message}");
        } catch (Exception e) {
            Debug.LogError($"Erro ao criar o arquivo de log: {e.Message}");
        }
    }
}
