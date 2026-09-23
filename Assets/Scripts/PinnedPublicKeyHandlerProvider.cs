using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "PinnedPublicKeyHandlerProvider",
    menuName = "Scriptable Objects/PinnedPublicKeyHandlerProvider")]
public sealed class PinnedPublicKeyHandlerProvider : CachedCertificateHandlerProvider
{
    [SerializeField] public byte[] publicKeyHash;

    protected override CertificateHandler CreateCertificateHandler()
    {
        return new PinnedPublicKeyHandler(publicKeyHash);
    }
}