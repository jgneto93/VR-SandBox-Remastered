using System;
using System.IO;
using UnityEngine;

public class UserInputLogger : MonoBehaviour {
    private string logFilePath;
    private string lastMessage = "";
    public UserInputLogger() {
        CreateNewTestLogFile();
    }
 
    public void LogCustomLine(string line) {
        if(lastMessage != line && line != "") {
            string logEntry = $"{Time.time}: {line.Replace("\n", " ")}";
            File.AppendAllText(logFilePath, logEntry + "\n");
            lastMessage = line;
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
