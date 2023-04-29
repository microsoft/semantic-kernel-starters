// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace Models;

internal class ExecuteFunctionResponse
{
    [JsonPropertyName("response")]
    public string? Response { get; set; }
}
