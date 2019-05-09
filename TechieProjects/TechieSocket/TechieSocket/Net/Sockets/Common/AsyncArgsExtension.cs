using System;
using System.Net.Sockets;
using System.Text;

namespace TechieSocket.Net.Sockets.Common
{
    public static class AsyncArgsExtension
    {
        public static int ReadInt(this SocketAsyncEventArgs args, ref int index)
        {
            int data = BitConverter.ToInt32(args.Buffer, args.Offset + index);
            index += 4;
            return data;
        }

        public static string ReadString(this SocketAsyncEventArgs args, ref int index, int count)
        {
            string data = Encoding.UTF8.GetString(args.Buffer, args.Offset + index, count);
            index += count;
            return data;
        }

        public static string GetString(this SocketAsyncEventArgs args)
        {
            return GetString(args, Encoding.UTF8);
        }

        public static string GetString(this SocketAsyncEventArgs args, Encoding encode)
        {
            return encode.GetString(args.Buffer, args.Offset, args.BytesTransferred);
        }

        public static byte[] CopyFromArgs(this SocketAsyncEventArgs args)
        {
            byte[] buff = new byte[args.BytesTransferred];
            Buffer.BlockCopy(args.Buffer, args.Offset, buff, 0, buff.Length);
            return buff;
        }

        public static void CopyToArgs(this SocketAsyncEventArgs args, string message)
        {
            CopyToArgs(args, message, Encoding.UTF8);
        }

        public static void CopyToArgs(this SocketAsyncEventArgs args, string message, Encoding encode)
        {
            CopyToArgs(args, encode.GetBytes(message));
        }

        public static void CopyToArgs(this SocketAsyncEventArgs args, byte[] srcBuffer)
        {
            Buffer.BlockCopy(srcBuffer, 0, args.Buffer, args.Offset, srcBuffer.Length);
            args.SetBuffer(args.Offset, srcBuffer.Length);
        }
    }
}
