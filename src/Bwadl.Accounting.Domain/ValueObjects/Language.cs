namespace Bwadl.Accounting.Domain.ValueObjects;

public enum Language
{
    English,
    Arabic
}

public static class LanguageExtensions
{
    public static string ToCode(this Language language) => language switch
    {
        Language.English => "en",
        Language.Arabic => "ar",
        _ => "en"
    };

    public static Language FromCode(string code) => code?.ToLower() switch
    {
        "ar" => Language.Arabic,
        "en" => Language.English,
        _ => Language.English
    };
}
