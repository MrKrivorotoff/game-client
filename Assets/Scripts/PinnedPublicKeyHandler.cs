using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.Assertions;
using UnityEngine.Networking;

public sealed class PinnedPublicKeyHandler : CertificateHandler
{
    private readonly byte[] _expectedKeyHash;

    public PinnedPublicKeyHandler(byte[] expectedKeyHash)
    {
        Assert.IsNotNull(expectedKeyHash);
        Assert.AreEqual(32, expectedKeyHash.Length);
        _expectedKeyHash = expectedKeyHash;
    }

    protected override bool ValidateCertificate(byte[] certificateData)
    {
        using var certificate = new X509Certificate2(certificateData);
        using var sha256 = SHA256.Create();
        Span<byte> keyHash = stackalloc byte[32];
        return sha256.TryComputeHash(certificate.GetPublicKey(), keyHash, out var bytesWritten) &&
               bytesWritten == 32 &&
               CryptographicOperations.FixedTimeEquals(keyHash, _expectedKeyHash);
    }
}