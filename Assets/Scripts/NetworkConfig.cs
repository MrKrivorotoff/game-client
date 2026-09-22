using UnityEngine;

[CreateAssetMenu(fileName = "NetworkConfig", menuName = "Scriptable Objects/NetworkConfig")]
public class NetworkConfig : ScriptableObject
{
    [SerializeField] public string serverBaseUrl = "https://localhost:8443";
}