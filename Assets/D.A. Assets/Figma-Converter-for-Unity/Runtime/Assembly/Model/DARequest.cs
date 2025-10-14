using UnityEngine;

public struct DARequest
{
    public RequestName Name { get; set; }
    public string Query { get; set; }
    public RequestType RequestType { get; set; }
    public RequestHeader RequestHeader { get; set; }
    public WWWForm WWWForm { get; set; }
}

public struct RequestHeader
{
    public string Name { get; set; }
    public string Value { get; set; }
}

public enum RequestType
{
    Get,
    Post,
    GetFile,
}

public enum RequestName
{
    None,
    Project,
}