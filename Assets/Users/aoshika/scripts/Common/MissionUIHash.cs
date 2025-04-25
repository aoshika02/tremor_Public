using System.Security.Cryptography;

public class MissionUIHash 
{
    public SHA256 MissionUIHashID { get; set; }
    public MissionUIHash()
    {
        MissionUIHashID = SHA256.Create();
    }
}