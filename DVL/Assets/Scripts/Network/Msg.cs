using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

public class Msg
{
    public MsgOpcode opcode; //Message header, instruction to the receiver, indicates which data should be included
    public int writeOffset; //Tracks current buffer size when appending data
    public int readOffset = 0;  //Tracks size of already read data when parsing
    public byte[] data; //Serialized data buffer

    //Constructor for messages to send
    public Msg (MsgOpcode _opcode, int size)
    {
        opcode = _opcode;
        writeOffset = 0;
        readOffset = 0;
        data = new byte[size];
    }

    //Constructor to rebuild received messages
    public Msg(byte[] serialized)
    {
        Deserialize(serialized);
    }

    //Combine header and body in one stream to send over the network
    public byte[] Serialize()
    {
        int size = data.Length  + 2 * sizeof(int);
        byte[] serialized = new byte[size];
        Buffer.BlockCopy(BitConverter.GetBytes((int)opcode), 0, serialized, 0, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(writeOffset), 0, serialized, 4, 4);
        Buffer.BlockCopy(data, 0, serialized, 8, data.Length);
        return serialized;
    }

    //Rebuild structure from received bytes
    public void Deserialize(byte[] serialized)
    {
        opcode = (MsgOpcode)BitConverter.ToInt32(serialized, 0);
        writeOffset = BitConverter.ToInt32(serialized, 4);
        data = new byte[serialized.Length - 2 * sizeof(int)];
        Buffer.BlockCopy(serialized, 8, data, 0, data.Length);
    }

    public void Write(int value)
    {
        Buffer.BlockCopy(BitConverter.GetBytes(value), 0, data, writeOffset, sizeof(int));
        writeOffset += sizeof(int);
    }

    public void Write(float value)
    {
        Buffer.BlockCopy(BitConverter.GetBytes(value), 0, data, writeOffset, sizeof(float));
        writeOffset += sizeof(float);
    }

    public void Write(Vector3 value)
    {
        Write(value.x);
        Write(value.y);
        Write(value.z);
    }

    public void Write(string value)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(value);
        int bufferSize = buffer.Length;
        Write(bufferSize);
        Buffer.BlockCopy(buffer, 0, data, writeOffset, bufferSize);
        writeOffset += bufferSize;
    }

    public int ReadInt()
    {
        int value = BitConverter.ToInt32(data, readOffset);
        readOffset += sizeof(int);
        return value;
    }

    public float ReadFloat()
    {
        float value = BitConverter.ToSingle(data, readOffset);
        readOffset += sizeof(float);
        return value;
    }

    public Vector3 ReadVector3()
    {
        return new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
    }

    public string ReadString()
    {
        int bufferSize = ReadInt();
        string value = Encoding.Unicode.GetString(data, readOffset, bufferSize);
        readOffset += bufferSize;
        return value;
    }
}
