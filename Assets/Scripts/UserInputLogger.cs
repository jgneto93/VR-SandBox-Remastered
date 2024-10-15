using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UserInputLogger : MonoBehaviour {
    private string logFilePath;
    private Dictionary<int, string> lastMessagesByType;
    private Dictionary<int, string> types;

    public UserInputLogger() {
        CreateNewTestLogFile();
        InitializeTypes();
        lastMessagesByType = new Dictionary<int, string>();
    }

    private void InitializeTypes() {
        types = new Dictionary<int, string> {
            { 1, "MENSAGEM" },
            { 2, "COMANDO" },
            { 3, "INFO" }
        };
    }

    public void LogCustomLine(int type, string line) {
        if (line != "" && (!lastMessagesByType.ContainsKey(type) || lastMessagesByType[type] != line)) {
            string typeString = types.ContainsKey(type) ? types[type] : "UNKNOWN";
            string logEntry = $"{Time.unscaledTime}, {typeString}, {line.Replace("\n", " ")}";
            File.AppendAllText(logFilePath, logEntry + "\n");
            lastMessagesByType[type] = line;
        }
    }

    public void CreateNewTestLogFile() {
        try {
            // Conta todos os arquivos na pasta
            string[] files = Directory.GetFiles(Application.persistentDataPath);
            int fileCount = files.Length;

            // Subtrai 1 (arquivo de controle) e divide por 2 (arquivos do UserMovementLogger)
            int logFileIndex = ((fileCount) / 2) + 1;

            // Cria o novo arquivo de log
            string newLogFilePath = Path.Combine(Application.persistentDataPath, $"UserInputLog{logFileIndex}.txt");
            File.Create(newLogFilePath).Dispose();

            logFilePath = newLogFilePath;

            Debug.Log($"Novo arquivo de log criado: {newLogFilePath}");

        } catch (UnauthorizedAccessException e) {
            Debug.LogError($"Acesso negado ao caminho: {Application.persistentDataPath}. Detalhes: {e.Message}");
        } catch (Exception e) {
            Debug.LogError($"Erro ao criar o arquivo de log: {e.Message}");
        }
    }
}
