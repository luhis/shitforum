namespace ThumbNailer
{
    public interface IThumbNailer
    {
        bool IsSettingValid();

        byte[] Make(byte[] input, string extension);
    }
}
