using System.Text.Json;

namespace AlmightyShogun.RemoteCommands;

public class RemoteCommandPayload
{
    public string Command { get; set; }
    public JsonElement Data { get; set; }
}