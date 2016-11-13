using System;
using System.Linq;
using System.Security;
using System.Security.Claims;

namespace coazure.Services
{
    public class PrincipalHelper
    {
        private const string AzureActiveDirectory = "aad";
        private const string AzureActiveDirectoryFullNameClaim = "name";

        private readonly ClaimsPrincipal _principal;

        public PrincipalHelper(ClaimsPrincipal principal)
        {
            _principal = principal;
        }

        public bool IsAuthenticated
        {
            get
            {
                CheckIdentity();
                return _principal.Identity.IsAuthenticated;
            }
        }

        public bool IsAzureActiveDirectoryUser => IsAuthenticated && _principal.Identity.AuthenticationType == AzureActiveDirectory;

        public string AuthenticationType => _principal.Identity.AuthenticationType;

        public string GetFullName()
        {
            CheckIdentity();
            if (_principal.HasClaim(c => c.Type == AzureActiveDirectoryFullNameClaim))
            {
                return _principal.FindFirst(AzureActiveDirectoryFullNameClaim).Value;
            }
            if (_principal.HasClaim(c => c.Type == ClaimTypes.Name))
            {
                return _principal.FindFirst(ClaimTypes.Name).Value;
            }
            return _principal.Identity.Name;
        }

        public void DoIf(Action<PrincipalHelper> authenticated, Action notAuthenticated)
        {
            if (IsAuthenticated)
            {
                authenticated(this);
            }
            else
            {
                notAuthenticated();
            }
        }

        public dynamic MakeSerializable()
        {
            var identity = _principal.Identity;
            var claims = _principal.Claims;

            var result = new
            {
                Identity = new
                {
                    identity.Name,
                    identity.AuthenticationType,
                    identity.IsAuthenticated
                },
                Claims = claims.Select(x =>
                    new
                    {
                        x.Type,
                        x.Value,
                        x.Issuer,
                        x.OriginalIssuer
                    }).ToArray()
            };
            return result;
        }

        private void CheckIdentity()
        {
            if (_principal == null)
                throw new SecurityException("No principal.");
            if (_principal.Identity == null)
                throw new SecurityException("No identity");
        }
    }
}