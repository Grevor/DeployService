using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viking.Deployment
{
    public class IPCPipe
    {
        private const string PipeName = "";
        private BinaryWriter Writer { get; }
        private BinaryReader Reader { get; }

        public IPCPipe(bool server)
        {
            Stream stream;
            if (server)
                stream = new NamedPipeServerStream(PipeName, PipeDirection.InOut);
            else
                stream = new NamedPipeClientStream(".", PipeName);

            Writer = new BinaryWriter(stream);
            Reader = new BinaryReader(stream);
        }

        public string ReadString() => Reader.ReadString();
    }
}
