using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

namespace Jay.IO
{
    public abstract class TextWriter : IDisposable, IAsyncDisposable
    {
        /// <inheritdoc />
        public virtual void Dispose()
        {
            // Do nothing
        }

        /// <inheritdoc />
        public virtual ValueTask DisposeAsync()
        {
            // Do nothing
            return ValueTask.CompletedTask;
        }
    }

    public class PipeTextWriter : TextWriter
    {
        private readonly PipeWriter _writer;

        /// <inheritdoc />
        public PipeTextWriter(PipeWriter writer)
        {
            _writer = writer;
        }

        public PipeTextWriter Write(string text)
        {
            MemoryPool<byte> thing = MemoryPool<byte>.Shared;
            using (var owner = thing.Rent(text.Length * sizeof(char)))
            {
                var count = Encoding.UTF8.GetBytes(text, owner.Memory.Span);
                _writer.Write(owner.Memory[..count].Span);
            }

            throw new NotImplementedException();
        }
        
        /// <inheritdoc />
        public override void Dispose()
        {
            _writer.Complete();
        }

        /// <inheritdoc />
        public override ValueTask DisposeAsync()
        {
            return _writer.CompleteAsync();
        }
    }
}