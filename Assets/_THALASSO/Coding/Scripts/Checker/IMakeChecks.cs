public interface IMakeChecks
{
    public bool IsActive { get; set; }

    void Check();
}
