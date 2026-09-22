using System;
using UnityEngine.Networking;

public abstract class CachedCertificateHandlerProvider : CertificateHandlerProvider
{
    private Lazy<CertificateHandler> _lazyCertificateHandler;

    public override CertificateHandler CertificateHandler => _lazyCertificateHandler.Value;

    public override bool DisposeHandlerOnRequestDispose => false;
    
    public void OnEnable()
    {
        _lazyCertificateHandler = new Lazy<CertificateHandler>(CreateCertificateHandler);
    }

    protected abstract CertificateHandler CreateCertificateHandler();
    
    public void OnDisable()
    {
        if (_lazyCertificateHandler is { IsValueCreated: true })
            _lazyCertificateHandler.Value.Dispose();
    }
}