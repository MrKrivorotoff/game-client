using UnityEngine;

[CreateAssetMenu(fileName = "NetworkConfig", menuName = "Scriptable Objects/NetworkConfig")]
public class NetworkConfig : ScriptableObject
{
    public string serverBaseUrl = "https://localhost:8443";
}