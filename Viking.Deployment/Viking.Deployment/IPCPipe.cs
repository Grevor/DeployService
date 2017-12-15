using System;
using System.IO;
using System.IO.Pipes;

namespace Viking.Deployment
{
    public class IPCPipe : IDisposable
    {
        private const string PipeName = "deploy_confirmation_pipe";
        private BinaryWriter Writer { get; }
        private BinaryReader Reader { get; }

        public IPCPipe(bool server)
        {
            Stream stream;
            if (server)
            {
                try
                {
                    var srv = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.WriteThrough);
                    srv.WaitForConnection();
                    stream = srv;
                } catch(Exception e)
                {
                    stream = null;
                }
            }
            else
            {
                var client = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut, PipeOptions.WriteThrough);
                client.Connect();
                stream = client;
            }
            Writer = new BinaryWriter(stream);
            Reader = new BinaryReader(stream);
        }

        public string ReadString() => Reader.ReadString();
        public bool ReadBool() => Reader.ReadBoolean();

        public void Send(string str) { Writer.Write(str); Writer.Flush(); }
        public void Send(bool boolean) => Writer.Write(boolean);

        public void Dispose()
        {
            Writer.Close();
        }
    }
}
