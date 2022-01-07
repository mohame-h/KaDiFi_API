namespace KaDiFi.Entities
{
    public class Enums
    {
    }

    public enum UserRoles
    {
        Admin = 0,
        User = 1,
        Annonymous = 2
    }

    public enum ErrorKeyTypes
    {
        ServerError,
        FormValidationError,
        SavingError,
        AuthenticatingError,
        TokenError,

    }

    public enum FormFieldTypes
    {
        FirstName,
        LastName,
        Age,
        Email,
        Password,

    }

    public enum MediaTypes
    {
        Photo = 0,
        Image = 1,
        Video = 2,
        GIF = 3,

    }

    public enum MediaCategories
    {
        Series = 0,
        Cartoons = 1,
        Movies = 2,
        Sports = 3,
        Songs = 4,

    }
    public enum PeriodTypes
    {
        All = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Yearly = 4,
    }



}
