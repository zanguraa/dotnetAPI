namespace DotnetApi.Dtos
{
    public partial class UserForLoginConfirmationDto
    {
        byte[] PasswordHash { get; set; }
        byte[] PasswordSalt { get; set; }

        public UserForLoginConfirmationDto()
        {
            if (PasswordHash == null)
            {
                PasswordHash = new byte[0];
            }
            if (PasswordSalt == null)
            {
                PasswordSalt = new byte[0];
            }

        }
    }
}