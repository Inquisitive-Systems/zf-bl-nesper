namespace ZF.BL.Nesper.Msmq
{
    public class Lz4
    {
        public byte[] Compress(byte[] input)
        {
            return LZ4.LZ4Codec.Wrap(input);
        }

        public byte[] Decompress(byte[] input)
        {
            return LZ4.LZ4Codec.Unwrap(input);
        }
    }
}