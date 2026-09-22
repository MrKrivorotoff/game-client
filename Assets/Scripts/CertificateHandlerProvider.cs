using UnityEngine;
using UnityEngine.Networking;

public abstract class CertificateHandlerProvider : ScriptableObject
{
    public abstract CertificateHandler CertificateHandler { get; }

    public abstract bool DisposeHandlerOnRequestDispose { get; }
}