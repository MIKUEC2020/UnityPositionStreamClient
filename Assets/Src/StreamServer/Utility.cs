﻿using System;
using System.Collections.Generic;
using StreamServer.Model;
using Vector3 = StreamServer.Model.Vector3;
using Vector4 = StreamServer.Model.Vector4;

namespace StreamServer
{
    public class Utility
    { 
        public static List<MinimumAvatarPacket> BufferToPackets(byte[] buf)
        {
            if (buf != null && buf.Length > 0)
            {
                var numPackets = BitConverter.ToInt32(buf, 0);
                var supposedBufSize = numPackets * 48 + 16;
                if (buf.Length == supposedBufSize)
                {
                    List<MinimumAvatarPacket> packets = new List<MinimumAvatarPacket>();
                    for (int i = 0; i < numPackets; ++i)
                    {
                        var beginOffset = i * 48;
                        var idLen = buf[16 + beginOffset];
                        byte[] bStr = new byte[idLen];
                        Buffer.BlockCopy(buf, 16 + 1 + beginOffset, bStr, 0, idLen);
                        var userId = System.Text.Encoding.UTF8.GetString(bStr);
                        var x = BitConverter.ToSingle(buf,  32 + beginOffset);
                        var y = BitConverter.ToSingle(buf,  32 + sizeof(float) + beginOffset);
                        var z = BitConverter.ToSingle(buf,  32 + sizeof(float) * 2 + beginOffset);
                        var radY = BitConverter.ToSingle(buf,  32 + sizeof(float) * 3 + beginOffset);
                        var qx = BitConverter.ToSingle(buf, 48 + beginOffset);
                        var qy = BitConverter.ToSingle(buf, 48 + sizeof(float) + beginOffset);
                        var qz = BitConverter.ToSingle(buf, 48 + sizeof(float) * 2 + beginOffset);
                        var qw = BitConverter.ToSingle(buf, 48+ sizeof(float) * 3 + beginOffset);
                        MinimumAvatarPacket packet = new MinimumAvatarPacket(userId, new Vector3(x, y, z), radY, new Vector4(qx, qy, qz, qw));
                        packets.Add(packet);
                    }
                    return packets;
                }
            }
            return null;
        }

        public static byte[] PacketsToBuffer(List<MinimumAvatarPacket> packets)
        {
            byte[] buff = new byte[48 * packets.Count + 16];
            var numPackets = BitConverter.GetBytes(packets.Count);
            Buffer.BlockCopy(numPackets, 0,  buff, 0, sizeof(int));
            for (int i = 0; i < packets.Count; ++i)
            {
                var packet = packets[i];
                var beginOffset = i * 48;
                var id = System.Text.Encoding.UTF8.GetBytes(packet.PaketId);
                var bx = BitConverter.GetBytes(packet.Position.X);
                var by = BitConverter.GetBytes(packet.Position.Y);
                var bz = BitConverter.GetBytes(packet.Position.Z);
                var bRadY = BitConverter.GetBytes(packet.RadY);
                var bQx = BitConverter.GetBytes(packet.NeckRotation.X);
                var bQy = BitConverter.GetBytes(packet.NeckRotation.Y);
                var bQz = BitConverter.GetBytes(packet.NeckRotation.Z);
                var bQw = BitConverter.GetBytes(packet.NeckRotation.W);
                buff[16 + beginOffset] = (byte)id.Length;
                Buffer.BlockCopy(id, 0, buff, 16 + 1 + beginOffset, id.Length);
                Buffer.BlockCopy(bx, 0, buff, 32 + beginOffset, sizeof(float));
                Buffer.BlockCopy(by, 0, buff, 32 + sizeof(float) + beginOffset, sizeof(float));
                Buffer.BlockCopy(bz, 0, buff, 32 + sizeof(float)*2 + beginOffset, sizeof(float));
                Buffer.BlockCopy(bRadY, 0, buff, 32 + sizeof(float)*3 + beginOffset, sizeof(float));
                Buffer.BlockCopy(bQx, 0, buff, 48 + beginOffset, sizeof(float));
                Buffer.BlockCopy(bQy, 0, buff, 48 + sizeof(float) + beginOffset, sizeof(float));
                Buffer.BlockCopy(bQz, 0, buff, 48 + sizeof(float)*2 + beginOffset, sizeof(float));
                Buffer.BlockCopy(bQw, 0, buff, 48 + sizeof(float)*3 + beginOffset, sizeof(float));
            }
            return buff;
        }
    }
}
