using System;
using UnityEngine.Assertions;
using UnityEngine.Networking;

public sealed class PinnedCertificateHandler : CertificateHandler
{
    private readonly byte[] _expectedCertificateData;

    public PinnedCertificateHandler(byte[] expectedCertBytes)
    {
        Assert.IsNotNull(expectedCertBytes);
        _expectedCertificateData = expectedCertBytes;
    }

    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return _expectedCertificateData.AsSpan().SequenceEqual(certificateData);
    }
}