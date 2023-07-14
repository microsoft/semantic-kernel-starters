// Copyright (c) Microsoft. All rights reserved.

namespace Models;

#pragma warning disable CA1056
public class AiPluginSettings
{
    public string SchemaVersion { get; set; } = "v1";
    public string NameForModel { get; set; } = string.Empty;
    public string NameForHuman { get; set; } = string.Empty;

    public string DescriptionForModel { get; set; } = string.Empty;

    public string DescriptionForHuman { get; set; } = string.Empty;

    public AuthModel Auth { get; set; } = new AuthModel();

    public ApiModel Api { get; set; } = new ApiModel();


    public string LogoUrl { get; set; } = string.Empty;

    public string ContactEmail { get; set; } = string.Empty;

    public string LegalInfoUrl { get; set; } = string.Empty;

    public class AuthModel
    {
        public string Type { get; set; } = string.Empty;

        public string AuthorizationType { get; set; } = string.Empty;
    }

    public class ApiModel
    {
        public string Type { get; set; } = "openapi";

        public string Url { get; set; } = string.Empty;

        public bool HasUserAuthentication { get; set; } = false;
    }
}
