using UnityEngine;
using Firebase.Database;

public class ScoreManager : MonoBehaviour
{

    string userID;
    DatabaseReference dbReference;


    private void Start()
    {
        userID = SystemInfo.deviceUniqueIdentifier;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

   public void CreateUser()
    {
        dbReference.Child("Score").Child(userID).SetRawJsonValueAsync("Hola");
    }
}
