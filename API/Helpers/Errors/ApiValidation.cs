namespace API.Helpers.Errors;

public class ApiValidation : ApiResponse
{
    public IEnumerable<string> Errors { get; set; }

    public ApiValidation() : base(400)
    {
    }
}
