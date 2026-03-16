using System.IO;
using UnityEngine;
using System;

[Serializable]
public class GameData
{
    public bool[] levelsCompleted = new bool[Enum.GetValues(typeof(LevelID)).Length];
}

public static class SaveSystem
{
    private static string filePath = Path.Combine(Application.persistentDataPath, "save.dat");
    private static string secretKey = "POMPOM_SECRETKEY";

    // -----------------------------
    // Guardar datos
    // -----------------------------
    public static void Save(GameData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data);
            string encrypted = EncryptDecrypt(json);
            File.WriteAllText(filePath, encrypted);
        }
        catch (Exception e)
        {
            Debug.LogError("Error guardando datos: " + e.Message);
        }
    }

    // -----------------------------
    // Cargar datos
    // -----------------------------
    public static GameData Load()
    {
        try
        {
            if (!File.Exists(filePath))
                return new GameData(); // si no existe, devolvemos datos nuevos

            string encrypted = File.ReadAllText(filePath);
            string json = EncryptDecrypt(encrypted);

            GameData data = JsonUtility.FromJson<GameData>(json);

            // Si por alguna razón el JSON no contiene datos válidos
            if (data == null || data.levelsCompleted == null)
                throw new Exception("Save corrupto o incompleto");

            return data;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Save corrupto. Regenerando archivo nuevo. Error: " + e.Message);

            GameData newData = new GameData();
            Save(newData);
            return newData;
        }
        
    }

    // -----------------------------
    // Cifrado XOR simple
    // -----------------------------
    private static string EncryptDecrypt(string text)
    {
        char[] result = new char[text.Length];

        for (int i = 0; i < text.Length; i++)
            result[i] = (char)(text[i] ^ secretKey[i % secretKey.Length]);

        return new string(result);
    }
}
