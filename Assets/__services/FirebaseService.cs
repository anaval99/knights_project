using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

// fb singleton service
public class FirebaseService
{
    public static FirebaseService _instance;

    public static FirebaseService Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new FirebaseService();
                Debug.Log("FirebaseService instance created.");
            }
            return _instance;
        }
    }

    public Task IsInitialized { get; private set; } = null;

    private Firebase.FirebaseApp firebaseApp { get; set; }

    // Private constructor to prevent instantiation
    private FirebaseService()
    {
        this.IsInitialized = this.Init();
        // Initialize Firebase here if needed
        Debug.Log("FirebaseService initialized.");
    }


    public async Task Init()
    {
        int maxTries = 10;
        int tries = 0;
        var status = Firebase.DependencyStatus.UnavailableOther;
        while (status != Firebase.DependencyStatus.Available)
        {
            tries++;
            status = await Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
            if (tries >= maxTries)
            {
                break;
            }
        }
        if (status == Firebase.DependencyStatus.Available)
        {
            this.firebaseApp = Firebase.FirebaseApp.DefaultInstance;
            return;
        }
        else
        {
            Debug.LogError($"Firebase dependencies are not available: {status}");
            throw new System.Exception("Firebase dependencies are not available.");
        }
    }

    // Example method to demonstrate functionality
    public void LogEvent(string eventName)
    {
        Debug.Log($"Event logged: {eventName}");
        // Add Firebase event logging logic here
    }

    public async Task Login(string email, string password)
    {
        var auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
        if (result != null)
        {
            Debug.Log($"User logged in successfully: {result.User.Email} with UID: {result.User.UserId}");
        }
        else
        {
            Debug.LogError("Login failed.");
        }
    }

    public async Task SaveSingle<T>(string path, T data) where T : IFirebaseDoc
    {
        string uid = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        var collectionRef = this.GetCollectionRef(path);
        var doc = collectionRef.Document(uid);
        data.UserId = uid; // Ensure UserId is set
        await doc.SetAsync(data);
        Debug.Log($"Document saved at path: {path} with UID: {uid}");
    }

    public async Task<T> GetSingle<T>(string path, string firebaseUid = null)
    {
        string uid = firebaseUid ?? Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        var collectionRef = this.GetCollectionRef(path);
        var doc = collectionRef.Document(uid);
        var snapShot = await doc.GetSnapshotAsync();
        if (snapShot.Exists)
        {
            return snapShot.ConvertTo<T>();
        }
        else
        {
            Debug.LogWarning($"No document found at path: {path} for UID: {uid}");
            return default(T);
        }
    }

    public async Task SaveSingleToList<T>(string path, T data) where T : IFirebaseDoc
    {
        string uid = Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        data.UserId = uid; // sender
        var collectionRef = this.GetCollectionRef(path);
        var doc = collectionRef.Document(Guid.NewGuid().ToString());
        await doc.SetAsync(data);
        Debug.Log($"Document saved to list at path: {path} with new GUID for UID: {uid}");
    }

    public string GetUserId()
    {
        var auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        var user = auth.CurrentUser;
        if (user == null || user.UserId == null)
        {
            return null;
        }

        return user.UserId;
    }

    public void Logout()
    {
        var auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.SignOut();
    }

    public Firebase.Firestore.CollectionReference GetCollectionRef(string path)
    {
        string pathPrefix = "__dev__"; // todo, use config to set this later
        var db = Firebase.Firestore.FirebaseFirestore.DefaultInstance;
        return db.Collection(pathPrefix + path);
    }

    public async Task<System.Collections.Generic.List<T>> QueryMany<T>(string path, Func<Firebase.Firestore.Query, Firebase.Firestore.Query> queryBuilder)
    {
        var collectionRef = this.GetCollectionRef(path);
        var query = queryBuilder(collectionRef);
        var querySnapshot = await query.GetSnapshotAsync();
        var results = new System.Collections.Generic.List<T>();
        foreach (var doc in querySnapshot.Documents)
        {
            if (doc.Exists)
            {
                results.Add(doc.ConvertTo<T>());
            }
        }
        return results;
    }
}
