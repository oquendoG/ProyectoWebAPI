namespace API.Helpers;

public class JWT
{
    public string Key { get; set; }

    /// <summary>
    /// Quien emite el json web token
    /// </summary>
    public string Issuer { get; set; }

    /// <summary>
    /// Para quien se emite el json web token
    /// </summary>
    public string Audience { get; set; }

    public double DurationInMinutes { get; set; }
}
