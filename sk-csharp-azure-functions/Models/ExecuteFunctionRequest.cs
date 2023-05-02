// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace Models;

#pragma warning disable CA1812
internal class ExecuteFunctionRequest
{
    [JsonPropertyName("variables")]
    public IEnumerable<ExecuteFunctionVariable> Variables { get; set; } = Enumerable.Empty<ExecuteFunctionVariable>();
}

internal class ExecuteFunctionVariable
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}
