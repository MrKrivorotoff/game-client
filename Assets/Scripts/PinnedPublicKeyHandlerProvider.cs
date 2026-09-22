using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "PinnedPublicKeyHandlerProvider",
    menuName = "Scriptable Objects/PinnedPublicKeyHandlerProvider")]
public sealed class PinnedPublicKeyHandlerProvider : CachedCertificateHandlerProvider
{
    [SerializeField] [Tooltip("Path to the certificate file, relative to the StreamingAssets directory.")]
    public string certificateRelativePath;

    protected override CertificateHandler CreateCertificateHandler()
    {
        return new PinnedPublicKeyHandler(GetPublicKeyHash());
    }

    private byte[] GetPublicKeyHash()
    {
        var certificatePath = Path.Combine(Application.streamingAssetsPath, certificateRelativePath);
        using var certificate = X509Certificate.CreateFromCertFile(certificatePath);
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(certificate.GetPublicKey());
    }
}