namespace AlmightyShogun.RemoteCommands;

[AttributeUsage(AttributeTargets.Class)]
public class RemoteCommandAttribute(string name, string description = "") : Attribute
{
    public required string Name { get; init; } = name;
    public string Description { get; set; } = description;
}
