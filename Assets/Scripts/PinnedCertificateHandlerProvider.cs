using System.IO;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "PinnedCertificateHandlerProvider",
    menuName = "Scriptable Objects/PinnedCertificateHandlerProvider")]
public sealed class PinnedCertificateHandlerProvider : CachedCertificateHandlerProvider
{
    [SerializeField] [Tooltip("Path to the certificate file, relative to the StreamingAssets directory.")]
    public string certificateRelativePath;

    protected override CertificateHandler CreateCertificateHandler()
    {
        var certificatePath = Path.Combine(Application.streamingAssetsPath, certificateRelativePath);
        return new PinnedCertificateHandler(File.ReadAllBytes(certificatePath));
    }
}