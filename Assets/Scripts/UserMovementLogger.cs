using System;
using System.IO;
using UnityEngine;

public class UserMovementLogger : MonoBehaviour {
    private string logFilePath;
    public UserMovementLogger() {
        CreateNewTestLogFile();
    }

    public void LogMovement(Transform transform) {
        string logEntry = $"{Time.time}, {transform.position}, {transform.rotation}";
        File.AppendAllText(logFilePath, logEntry + "\n");

    }

    public void LogCustomLine(string line) {
        string logEntry = $"{Time.time}: {line}";
        File.AppendAllText(logFilePath, logEntry + "\n");
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

            Debug.Log($"Novo arquivo de log criado: {newLogFilePath}");
        } catch (UnauthorizedAccessException e) {
            Debug.LogError($"Acesso negado ao caminho: {Application.persistentDataPath}. Detalhes: {e.Message}");
        } catch (Exception e) {
            Debug.LogError($"Erro ao criar o arquivo de log: {e.Message}");
        }
    }
}
