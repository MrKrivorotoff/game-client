using System;
using UnityEngine.Networking;

public sealed class PinnedCertificateHandler : CertificateHandler
{
    private readonly byte[] _expectedCertificateData;

    public PinnedCertificateHandler(byte[] expectedCertBytes)
    {
        _expectedCertificateData = expectedCertBytes ?? throw new ArgumentNullException(nameof(expectedCertBytes));
    }

    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return _expectedCertificateData.AsSpan().SequenceEqual(certificateData);
    }
}