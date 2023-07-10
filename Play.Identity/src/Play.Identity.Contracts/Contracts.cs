namespace Play.Identity.Contracts;

public class Contracts
{
    public record DebitGil(Guid UserId, int Amount, Guid CorrelationId);
    public record GilDebited(Guid CorrelationId);
}
