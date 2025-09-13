using System;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private string fullPath;
    private bool encryptData;
    private string codeWord = "iloveducks";

    public FileDataHandler(string dataDirPath, string dataFileName, bool encryptData)
    {
        fullPath = Path.Combine(dataDirPath, dataFileName);
        this.encryptData = encryptData;
    }

    public void SaveData(GameData gameData)
    {
        try
        {
            // Create directory if it doesn't exit
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // Convert gamedata to JSON string
            string dataToSave = JsonUtility.ToJson(gameData, true);

            if(encryptData)
                dataToSave = EncryptDecrypt(dataToSave);

            // Open/create a new file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                // Write the JSON text to the file
                using (StreamWriter write = new StreamWriter(stream))
                {
                    write.Write(dataToSave);
                }
            }
        }


        catch (Exception e)
        {
            // Log any error that happens
            Debug.LogError("Error on trying to save data to file: " + fullPath + "\n" + e);
        }
    }

    public GameData LoadData()
    {
        GameData loadData = null;

        // Check if the save file exists
        if(File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";

                // Open the file
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    // Read file's text content
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if (encryptData)
                    dataToLoad = EncryptDecrypt(dataToLoad);

                // Convert the JSON string back into GameData
                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }



            catch (Exception e)
            {
                // Log any error that happens
                Debug.LogError("Error on trying to load data from file: " + fullPath + "\n" + e);
            }
        }

        return loadData;
    }

    public void DeleteSave()
    {
        if(File.Exists(fullPath))
            File.Delete(fullPath);
    }

    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";

        for(int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ codeWord[i % codeWord.Length]);
        }

        return modifiedData;
    }
}
