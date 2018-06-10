namespace ImageValidation
{
    public class SizeExceeded
    {
        public SizeExceeded(int maxSize)
        {
            MaxSize = maxSize;
        }

        public int MaxSize { get; }
    }
}