using System;
using System.IO;
using UnityEngine;

public class UserMovementLogger : MonoBehaviour {
    private string logFilePath;
    private string lastLog = "";
    private Transform lastTransform;
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private const float positionThreshold = 0.01f; // Limiar para mudanças de posição
    private const float rotationThreshold = 0.01f; // Limiar para mudanças de rotação

    public UserMovementLogger() {
        CreateNewTestLogFile();
    }

    public void LogMovement(Transform transform) {
        if (lastTransform != transform ||
            Vector3.Distance(lastPosition, transform.position) > positionThreshold ||
            Quaternion.Angle(lastRotation, transform.rotation) > rotationThreshold) {

            string logEntry = $"{Time.unscaledTime}, {transform.position}, {transform.rotation}";
            File.AppendAllText(logFilePath, logEntry + "\n");
            lastTransform = transform;
            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }
    }

    public void LogCustomLine(string line) {
        if (lastLog != line && line != "") {
            string logEntry = $"{Time.unscaledTime}: {line.Replace("\n", " ")}";
            File.AppendAllText(logFilePath, logEntry + "\n");
            lastLog = line;
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

            Debug.Log($"Novo arquivo de log criado: {newLogFilePath}");
        } catch (UnauthorizedAccessException e) {
            Debug.LogError($"Acesso negado ao caminho: {Application.persistentDataPath}. Detalhes: {e.Message}");
        } catch (Exception e) {
            Debug.LogError($"Erro ao criar o arquivo de log: {e.Message}");
        }
    }
}
