using System;
using UnityEngine.Networking;

public sealed class PinnedCertificateHandler : CertificateHandler
{
    private readonly byte[] _expectedCertBytes;

    public PinnedCertificateHandler(byte[] expectedCertBytes)
    {
        _expectedCertBytes = expectedCertBytes ?? throw new ArgumentNullException(nameof(expectedCertBytes));
    }

    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return certificateData != null && MemoryExtensions.SequenceEqual<byte>(certificateData, _expectedCertBytes);
    }
}