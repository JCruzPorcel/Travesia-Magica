using Firebase.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;


public static class FirebaseManager
{
    private const int MAX_TABLE_OF_PLAYERS = 10;

    private static DatabaseReference databaseRef = FirebaseDatabase.DefaultInstance.RootReference;


    public static async Task SaveScore(string playerName, int playerScore)
    {
        if (IsConnectedToInternet())
        {
            await AddGlobalScore(playerName, playerScore);
            await AddLocalScore(playerName, playerScore);
        }
        else
        {
            await AddLocalScore(playerName, playerScore);
        }
    }

    #region Global Score
    private static async Task AddGlobalScore(string playerName, int playerScore)
    {
        string path = $"Leaderboard/Score/{playerName}";
        var snapshot = await databaseRef.Child(path).GetValueAsync();

        // Si el jugador ya tiene puntuaciones, agregue la nueva puntuación a la lista existente
        if (snapshot.Exists)
        {
            var scoreList = snapshot.Value as List<object>;
            if (scoreList != null)
            {
                scoreList.Add(playerScore);
                await databaseRef.Child(path).SetValueAsync(scoreList);
            }
        }
        // Si el jugador no tiene puntuaciones, cree una nueva lista de puntuaciones
        else
        {
            List<int> scoreList = new List<int>();
            scoreList.Add(playerScore);
            await databaseRef.Child(path).SetValueAsync(scoreList);
        }
    }
    public static async Task<List<PlayerScore>> LoadGlobalScores()
    {
        string path = "Leaderboard/Score";
        List<PlayerScore> playerScores = new List<PlayerScore>();
        await databaseRef.Child(path).OrderByValue().LimitToLast(10).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error al cargar los puntajes globales: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                IEnumerable<DataSnapshot> scoreSnapshots = snapshot.Children.Reverse(); // Invertir la lista para que los puntajes más altos aparezcan primero

                foreach (DataSnapshot scoreSnapshot in scoreSnapshots)
                {
                    string playerName = scoreSnapshot.Key;
                    List<object> scoreList = scoreSnapshot.Value as List<object>;
                    if (scoreList != null)
                    {
                        foreach (object scoreObj in scoreList)
                        {
                            int playerScore = Convert.ToInt32(scoreObj);
                            playerScores.Add(new PlayerScore(playerName, playerScore));
                        }
                    }
                }
                playerScores.Sort();
                playerScores.Reverse();
            }
        });
        return playerScores.Take(MAX_TABLE_OF_PLAYERS).ToList();
    }
    #endregion


    #region Local Score
    private static async Task AddLocalScore(string playerName, int playerScore)
    {
        string path = Application.persistentDataPath + $"/LocalScore.dat";
        Dictionary<string, List<int>> scoreDict;
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        if (!IsConnectedToInternet())
        {
            path = Application.persistentDataPath + $"/OfflineScore.dat";
        }


        // Si el archivo de puntuaciones del jugador ya existe, cargue el diccionario de puntuaciones
        if (File.Exists(path))
        {
            using (FileStream fileStream = File.OpenRead(path))
            {
                scoreDict = (Dictionary<string, List<int>>)binaryFormatter.Deserialize(fileStream);
                fileStream.Close();
            }
        }
        // Si el archivo de puntuaciones del jugador no existe, cree un nuevo diccionario de puntuaciones
        else
        {
            scoreDict = new Dictionary<string, List<int>>();
        }

        // Agregar la nueva puntuación al diccionario existente
        if (scoreDict.ContainsKey(playerName))
        {
            scoreDict[playerName].Add(playerScore);
        }
        else
        {
            scoreDict.Add(playerName, new List<int> { playerScore });
        }

        // Guardar el diccionario actualizado de puntuaciones del jugador en el archivo
        using (FileStream fileStream = File.Create(path))
        {
            await Task.Run(() => binaryFormatter.Serialize(fileStream, scoreDict));
            fileStream.Close();
        }
    }
    public static async Task<List<PlayerScore>> LoadLocalScores()
    {
        string path = Application.persistentDataPath + $"/LocalScore.dat";
        List<PlayerScore> scores = new List<PlayerScore>();

        if (!IsConnectedToInternet())
        {
            path = Application.persistentDataPath + $"/OfflineScore.dat";
        }

        if (File.Exists(path))
        {
            Dictionary<string, List<int>> scoreDict;

            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                scoreDict = (Dictionary<string, List<int>>)await Task.Run(() => binaryFormatter.Deserialize(fileStream));
                fileStream.Close();
            }

            // Recorrer el diccionario y agregar los objetos de PlayerScore a la lista de puntajes
            foreach (KeyValuePair<string, List<int>> entry in scoreDict)
            {
                string playerName = entry.Key;
                List<int> playerScores = entry.Value;

                foreach (int score in playerScores)
                {
                    scores.Add(new PlayerScore(playerName, score));
                }
            }
        }
        else
        {
            Debug.LogError("Error al cargar los puntajes locales: ");
        }

        scores.Sort();
        scores.Reverse();

        return scores.Take(MAX_TABLE_OF_PLAYERS).ToList();
    }
    #endregion

    public static async Task SyncLocalScoresWithFirebase()
    {
        string offlineLocalScore = Application.persistentDataPath + $"/OfflineScore.dat";

        if (File.Exists(offlineLocalScore) && IsConnectedToInternet())
        {
            Dictionary<string, List<int>> localScoresDict;

            using (FileStream fileStream = new FileStream(offlineLocalScore, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                localScoresDict = (Dictionary<string, List<int>>)await Task.Run(() => binaryFormatter.Deserialize(fileStream));
                fileStream.Close();
            }

            foreach (KeyValuePair<string, List<int>> entry in localScoresDict)
            {
                string playerName = entry.Key;
                List<int> playerScores = entry.Value;

                foreach (int score in playerScores)
                {
                    await AddGlobalScore(playerName, score);
                    await AddLocalScore(playerName, score);
                }
            }

            // Eliminar el archivo de puntuaciones local después de subir los datos a Firebase
            File.Delete(offlineLocalScore);
        }
    }

    public static bool IsConnectedToInternet()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            // El dispositivo está conectado a Internet, pero no garantiza que haya acceso a la red
            // Por lo tanto, realizamos una verificación adicional para confirmar que hay conexión
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    if (ni.OperationalStatus == OperationalStatus.Up)
                    {
                        // Se detectó una interfaz de red activa, lo que indica que hay conexión a Internet
                        return true;
                    }
                }
            }
        }

        // No se detectó conexión a Internet
        return false;
    }
}
[Serializable]
public class PlayerScore : IComparable<PlayerScore>
{
    public string playerName;
    public int playerScore;

    public PlayerScore(string name, int score)
    {
        playerName = name;
        playerScore = score;
    }

    public int CompareTo(PlayerScore other)
    {
        if (other == null) return 1;
        return playerScore.CompareTo(other.playerScore);
    }
}