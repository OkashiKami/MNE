using System;
using System.Diagnostics.Tracing;
using System.IO;

namespace MNE
{
    public enum PacketHeader : int
    {
        NoHeader = -1,
        Welcome
    }
    public class Packet : IDisposable
    {
        private MemoryStream stream;
        private BinaryWriter write;
        private BinaryReader reader;

        public Packet()
        {
            stream = new MemoryStream();
            reader = new BinaryReader(stream);
            write = new BinaryWriter(stream);
        }
        public Packet(byte[] data)
        {
            stream = new MemoryStream(data);
            reader = new BinaryReader(stream);
            write = new BinaryWriter(stream);

        }


        public static implicit operator byte[](Packet packet) { packet.stream.Position = 0; return packet.stream.ToArray(); }

        public Packet Write(PacketHeader value) { write.Write((int)value); return this; }
        public Packet Write(short value) { write.Write(value); return this; }
        public Packet Write(int value) { write.Write(value); return this; }
        public Packet Write(long value) { write.Write(value); return this; }
        public Packet Write(float value) { write.Write(value); return this; }
        public Packet Write(bool value) { write.Write(value); return this; }
        public Packet Write(string value) {write.Write(value); return this; }
        public Packet Write(byte[] value) { write.Write(value.Length); write.Write(value); return this; }
        public Packet Write(char[] value) { write.Write(value.Length); write.Write(value); return this; }


        public PacketHeader ReadHeader => (PacketHeader)reader.ReadInt32();
        public short ReadShot => reader.ReadInt16();
        public int ReadInt => reader.ReadInt32();
        public long ReadLong => reader.ReadInt64();
        public float ReadFloat => reader.ReadSingle();
        public bool ReadBool => reader.ReadBoolean();
        public string ReadString => reader.ReadString();
        public byte[] ReadBytes => reader.ReadBytes(reader.ReadInt32());
        public char[] ReadChars => reader.ReadChars(reader.ReadInt32());

        public void Dispose()
        {
            try { reader.Dispose(); } catch { }
            try { write.Dispose(); } catch { }
        }
    }
}