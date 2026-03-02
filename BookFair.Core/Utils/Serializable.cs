namespace BookFair.Core.Utils
{
    public interface Serializable
    {
        string[] ToCSV();
        void FromCSV(string[] values);
    }
}
