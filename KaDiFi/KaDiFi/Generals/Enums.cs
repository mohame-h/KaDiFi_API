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
        ParamError,

    }

    public enum FormFieldTypes
    {
        Name,
        Age,
        Email,
        Password,
        VerificationCode,
        FreezingPeriod,
        MediaId,

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
    public enum MediaReactTypes
    {
        None = 0,
        Like = 1,
        Dislike = 2,

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
