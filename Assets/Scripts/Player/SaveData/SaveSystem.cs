using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem 
{
    public static void SaveData(UserStatus user, UpgradeItem attackUp, UpgradeItem critDmgUp, UpgradeItem deffenseUp, UpgradeItem healthUp, UpgradeItem luckUp)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new SaveData(user, attackUp, critDmgUp, deffenseUp, healthUp, luckUp);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SaveData LoadData()
    {
        string path = Application.persistentDataPath + "/player.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;

            return data;
        }
        else
        {
            return null;
        }
    }
}
    