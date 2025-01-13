public interface IAmChecker
{
    public bool IsActive { get; set; }

    void Check();
}
