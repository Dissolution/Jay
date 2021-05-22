using System.IO;
using System.Text;

namespace Jay
{
    public static class StreamExtensions
    {
        
        /// <summary>
        /// Determines a stream's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the encoding fails.
        /// </summary>
        /// <param name="stream">The stream to analyze.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncoding(this Stream stream)
        {
            // Read the BOM
            var bom = new byte[4];
            //Save our current position
            var position = stream.Position;
            try
            {
                //Return to origin if we have to
                if (position != 0L)
                    stream.Seek(0L, SeekOrigin.Begin);
                //Read the byte order mark
                stream.Read(bom, 0, 4);
				
                // Analyze the BOM
#pragma warning disable 618
                #pragma warning disable SYSLIB0001
                if (bom[0] == 0x2B && bom[1] == 0x2F && bom[2] == 0x76)
                    return Encoding.UTF7;
                #pragma warning restore SYSLIB0001
#pragma warning restore 618
                if (bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
                    return Encoding.UTF8;
                if (bom[0] == 0xFF && bom[1] == 0xFE)
                    return Encoding.Unicode; //UTF-16LE
                if (bom[0] == 0xFE && bom[1] == 0xFF)
                    return Encoding.BigEndianUnicode; //UTF-16BE
                if (bom[0] == 0x0 && bom[1] == 0x0 && bom[2] == 0xFE && bom[3] == 0xFF)
                    return Encoding.UTF32;

                //Attempt to use StreamReader, falls back to ASCII
                if (stream.Position != 0L)
                    stream.Seek(0L, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream, Encoding.ASCII, true))
                {
                    reader.Peek(); //has to peek or it won't have read the bom
                    return reader.CurrentEncoding;
                }
            }
            finally
            {
                //Return to position
                if (stream.Position != position)
                    stream.Seek(position, SeekOrigin.Begin);
            }
        }
    }
}