public enum PetitionStatusMessagge
{
    OK,
    TIME_OUT,
    MAX_CONNECTION,
    UNKNOWN_ERROR,
}

public class PetitionStatus
{
    public PetitionStatus()
    {
        success = true;
        message = PetitionStatusMessagge.OK;
    }

    public bool success;
    public PetitionStatusMessagge message;
}