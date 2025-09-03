namespace ASP_32.Services.Kdf
{
    // Key Derivation Functions Service by sec.5 RFC 2898
    public interface IKdfService
    {
        String Dk(String password, String salt);
    }
}
