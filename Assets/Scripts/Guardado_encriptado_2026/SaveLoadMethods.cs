using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class SaveLoadMethods
{
    private const string EncryptionKey = "TuClaveSecreta123"; // Clave para encriptar

    // Método para guardar con encriptación
    public static void SaveAllData(GameManager gm, PlayerControl pc)
    {
        SaveData saveData = new SaveData(gm, pc);
        string json = JsonUtility.ToJson(saveData, true);
        string encrypted = EncryptString(json, EncryptionKey);
        string dataPath = Application.persistentDataPath + "/savedata.save";
        File.WriteAllText(dataPath, encrypted);
    }

    // Método para cargar con desencriptación
    public static SaveData LoadAllData()
    {
        string dataPath = Application.persistentDataPath + "/savedata.save";

        if (File.Exists(dataPath))
        {
            try
            {
                string encryptedData = File.ReadAllText(dataPath);
                string decryptedJson = DecryptString(encryptedData, EncryptionKey);
                SaveData saveData = JsonUtility.FromJson<SaveData>(decryptedJson);
                return saveData;
            }
            catch (Exception e)
            {
                Debug.LogError("Error al cargar datos: " + e.Message);
                return null;
            }
        }
        else
        {
            Debug.LogError("No se encontró archivo de guardado");
            return null;
        }
    }


    // Método para borrar fichero de guardado
    public static void DeleteSaveData()
    {
        string dataPath = Application.persistentDataPath + "/savedata.save";

        if (File.Exists(dataPath))
        {
            File.Delete(dataPath);
            Debug.Log("Archivo de guardado eliminado correctamente");
        }
        else
        {
            Debug.LogWarning("No existe ningún archivo de guardado para eliminar");
        }
    }

    // Encriptación AES
    private static string EncryptString(string plainText, string key)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.GenerateIV();

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                // Guardar el IV al inicio del stream
                msEncrypt.Write(aes.IV, 0, aes.IV.Length);

                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                }

                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    // Desencriptación AES
    private static string DecryptString(string cipherText, string key)
    {
        byte[] fullCipher = Convert.FromBase64String(cipherText);
        byte[] keyBytes = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;

            // Extraer el IV del inicio (primeros 16 bytes)
            byte[] iv = new byte[aes.IV.Length];
            byte[] cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            aes.IV = iv;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipher))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}
