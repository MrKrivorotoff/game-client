using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "PinnedCertificateHandlerProvider",
    menuName = "Scriptable Objects/PinnedCertificateHandlerProvider")]
public sealed class PinnedCertificateHandlerProvider : CertificateHandlerProvider
{
    [SerializeField] [Tooltip("Path to the certificate file, relative to the StreamingAssets directory.")]
    public string certificateRelativePath;

    private Lazy<CertificateHandler> _lazyCertificateHandler;

    public override CertificateHandler CertificateHandler => _lazyCertificateHandler.Value;

    public override bool DisposeHandlerOnRequestDispose => false;

    public void OnEnable()
    {
        _lazyCertificateHandler = new Lazy<CertificateHandler>(() =>
        {
            var certificatePath = Path.Combine(Application.streamingAssetsPath, certificateRelativePath);
            return new PinnedCertificateHandler(File.ReadAllBytes(certificatePath));
        });
    }

    public void OnDisable()
    {
        if (_lazyCertificateHandler is { IsValueCreated: true })
            _lazyCertificateHandler.Value.Dispose();
    }
}