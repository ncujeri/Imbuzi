namespace ImbuziSmart.Shared.ValueObjects;

public record Photo(string FileName, string ContentType, string Base64Data, DateTime TakenAt);
