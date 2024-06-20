// Custom JSON Converter
using Microsoft.PackageGraph.MicrosoftUpdate.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;

public class CustomSoftwareUpdateConverter : JsonConverter<SoftwareUpdate>
{
    public override SoftwareUpdate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException("Deserialization is not implemented");
    }

    public override void Write(Utf8JsonWriter writer, SoftwareUpdate value, JsonSerializerOptions options)
    {
        // Implement custom serialization logic here
        writer.WriteStartObject();

        // Write basic properties
        writer.WriteString("Id", value.Id.ToString());
        writer.WriteString("Title", value.Title ?? ""); // Ensure Title is not null
        writer.WriteString("Description", value.Description ?? ""); // Ensure Description is not null
        writer.WriteString("SupportUrl", value.SupportUrl ?? ""); // Ensure SupportUrl is not null
        writer.WriteString("KBArticleId", value.KBArticleId ?? ""); // Ensure KBArticleId is not null
        writer.WriteString("OsUpgrade", value.OsUpgrade ?? "");
        

        // Write Files array
        writer.WriteStartArray("Files");
        foreach (var file in value.Files)
        {
            writer.WriteStartObject();
            writer.WriteString("FileName", file.FileName ?? ""); // Ensure FileName is not null
            writer.WriteNumber("Size", file.Size);
            // Add more properties as needed
            writer.WriteEndObject();
        }
        writer.WriteEndArray();

        // Write Categories array dynamically based on update properties
        var categories = GetCategories(value);
        writer.WriteStartArray("Categories");
        foreach (var category in categories)
        {
            writer.WriteStringValue(category);
        }
        writer.WriteEndArray();

        // Handle other properties and custom serialization logic here

        writer.WriteEndObject();
    }

    private List<string> GetCategories(SoftwareUpdate update)
    {
        List<string> categories = new List<string>();

        // Example: Check if update is a security update
        if (update.Title.Contains("Security", StringComparison.OrdinalIgnoreCase) ||
            update.Description.Contains("Security", StringComparison.OrdinalIgnoreCase))
        {
            categories.Add("Security");
        }

        // Example: Check if update is for an operating system upgrade
        if (update.OsUpgrade != null &&
            (update.OsUpgrade.Contains("Windows 10", StringComparison.OrdinalIgnoreCase) ||
             update.OsUpgrade.Contains("Windows Server", StringComparison.OrdinalIgnoreCase)))
        {
            categories.Add("Operating System");
        }

        // Add more conditions based on other properties of the SoftwareUpdate class as needed

        return categories;
    }


}