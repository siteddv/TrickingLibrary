using IdentityServer4;

namespace TrickingLibrary.API
{
    public struct TrickingLibraryConstants
    {
        public struct Policies
        {
            public const string User = IdentityServerConstants.LocalApi.PolicyName;
            public const string Mod = nameof(Mod);
        }

        public struct Roles
        {
            public const string Mod = nameof(Mod);
        }
        
        public struct IdentityResources
        {
            public const string RoleScope = "role";
        }

        public struct Claims
        {
            public const string Role = "role";
        }
    }
}