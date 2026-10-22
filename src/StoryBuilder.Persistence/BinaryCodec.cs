using System.Buffers.Binary;
using System.Text;
using TheSingularityWorkshop.FSM_Serialization;

namespace StoryBuilder.Persistence;

internal static class BinaryCodec
{
    internal const int MaxStringBytes = 4 * 1024 * 1024;
    internal const int MaxCollectionCount = 100_000;
    private static readonly UTF8Encoding StrictUtf8 = new(false, true);

    internal static void WriteInt32(IBinaryStream stream, int value)
    {
        Span<byte> bytes = stackalloc byte[sizeof(int)];
        BinaryPrimitives.WriteInt32LittleEndian(bytes, value);
        stream.Write(bytes);
    }

    internal static int ReadInt32(IBinaryStream stream)
    {
        Span<byte> bytes = stackalloc byte[sizeof(int)];
        ReadExactly(stream, bytes);
        return BinaryPrimitives.ReadInt32LittleEndian(bytes);
    }

    internal static void WriteInt64(IBinaryStream stream, long value)
    {
        Span<byte> bytes = stackalloc byte[sizeof(long)];
        BinaryPrimitives.WriteInt64LittleEndian(bytes, value);
        stream.Write(bytes);
    }

    internal static long ReadInt64(IBinaryStream stream)
    {
        Span<byte> bytes = stackalloc byte[sizeof(long)];
        ReadExactly(stream, bytes);
        return BinaryPrimitives.ReadInt64LittleEndian(bytes);
    }

    internal static void WriteDouble(IBinaryStream stream, double value) =>
        WriteInt64(stream, BitConverter.DoubleToInt64Bits(value));

    internal static double ReadDouble(IBinaryStream stream) =>
        BitConverter.Int64BitsToDouble(ReadInt64(stream));

    internal static void WriteBoolean(IBinaryStream stream, bool value) =>
        stream.Write([value ? (byte)1 : (byte)0]);

    internal static bool ReadBoolean(IBinaryStream stream)
    {
        Span<byte> bytes = stackalloc byte[1];
        ReadExactly(stream, bytes);
        return bytes[0] switch
        {
            0 => false,
            1 => true,
            _ => throw new InvalidDataException("A Boolean field must be encoded as 0 or 1.")
        };
    }

    internal static void WriteString(IBinaryStream stream, string? value)
    {
        if (value is null)
        {
            WriteInt32(stream, -1);
            return;
        }

        var bytes = StrictUtf8.GetBytes(value);
        if (bytes.Length > MaxStringBytes)
            throw new InvalidDataException($"String exceeds the {MaxStringBytes}-byte persistence limit.");

        WriteInt32(stream, bytes.Length);
        stream.Write(bytes);
    }

    internal static string? ReadString(IBinaryStream stream)
    {
        var length = ReadInt32(stream);
        if (length == -1)
            return null;
        if (length < 0 || length > MaxStringBytes)
            throw new InvalidDataException($"Invalid string length: {length}.");

        var bytes = new byte[length];
        ReadExactly(stream, bytes);
        return StrictUtf8.GetString(bytes);
    }

    internal static string ReadRequiredString(IBinaryStream stream, string field) =>
        ReadString(stream) ?? throw new InvalidDataException($"Required field '{field}' was null.");

    internal static void WriteGuid(IBinaryStream stream, Guid value) =>
        stream.Write(value.ToByteArray());

    internal static Guid ReadGuid(IBinaryStream stream)
    {
        Span<byte> bytes = stackalloc byte[16];
        ReadExactly(stream, bytes);
        return new Guid(bytes);
    }

    internal static void WriteNullableGuid(IBinaryStream stream, Guid? value)
    {
        WriteBoolean(stream, value.HasValue);
        if (value.HasValue)
            WriteGuid(stream, value.Value);
    }

    internal static Guid? ReadNullableGuid(IBinaryStream stream) =>
        ReadBoolean(stream) ? ReadGuid(stream) : null;

    internal static void WriteDateTimeOffset(IBinaryStream stream, DateTimeOffset value)
    {
        WriteInt64(stream, value.Ticks);
        WriteInt64(stream, value.Offset.Ticks);
    }

    internal static DateTimeOffset ReadDateTimeOffset(IBinaryStream stream)
    {
        var ticks = ReadInt64(stream);
        var offsetTicks = ReadInt64(stream);
        try
        {
            return new DateTimeOffset(ticks, TimeSpan.FromTicks(offsetTicks));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            throw new InvalidDataException("Invalid DateTimeOffset in workspace payload.", ex);
        }
    }

    internal static void WriteEnum<TEnum>(IBinaryStream stream, TEnum value) where TEnum : struct, Enum =>
        WriteInt32(stream, Convert.ToInt32(value, System.Globalization.CultureInfo.InvariantCulture));

    internal static TEnum ReadEnum<TEnum>(IBinaryStream stream) where TEnum : struct, Enum
    {
        var raw = ReadInt32(stream);
        if (!Enum.IsDefined(typeof(TEnum), raw))
            throw new InvalidDataException($"Invalid {typeof(TEnum).Name} value: {raw}.");
        return (TEnum)Enum.ToObject(typeof(TEnum), raw);
    }

    internal static void WriteStringList(IBinaryStream stream, IReadOnlyList<string> values)
    {
        WriteCount(stream, values.Count);
        foreach (var value in values)
            WriteString(stream, value);
    }

    internal static List<string> ReadStringList(IBinaryStream stream)
    {
        var count = ReadCount(stream);
        var values = new List<string>(count);
        for (var i = 0; i < count; i++)
            values.Add(ReadRequiredString(stream, "string list item"));
        return values;
    }

    internal static void WriteGuidList(IBinaryStream stream, IReadOnlyList<Guid> values)
    {
        WriteCount(stream, values.Count);
        foreach (var value in values)
            WriteGuid(stream, value);
    }

    internal static List<Guid> ReadGuidList(IBinaryStream stream)
    {
        var count = ReadCount(stream);
        var values = new List<Guid>(count);
        for (var i = 0; i < count; i++)
            values.Add(ReadGuid(stream));
        return values;
    }

    internal static void WriteCount(IBinaryStream stream, int count)
    {
        if (count < 0 || count > MaxCollectionCount)
            throw new InvalidDataException($"Collection count is outside the supported range: {count}.");
        WriteInt32(stream, count);
    }

    internal static int ReadCount(IBinaryStream stream)
    {
        var count = ReadInt32(stream);
        if (count < 0 || count > MaxCollectionCount)
            throw new InvalidDataException($"Collection count is outside the supported range: {count}.");
        return count;
    }

    internal static void ReadExactly(IBinaryStream stream, Span<byte> buffer)
    {
        var offset = 0;
        while (offset < buffer.Length)
        {
            var read = stream.Read(buffer[offset..]);
            if (read <= 0)
                throw new EndOfStreamException("The binary workspace payload ended unexpectedly.");
            offset += read;
        }
    }

    internal static void WriteList<T>(IBinaryStream stream, IReadOnlyList<T> values, Action<IBinaryStream, T> writeItem)
    {
        WriteCount(stream, values.Count);
        foreach (var value in values)
            writeItem(stream, value);
    }

    internal static List<T> ReadList<T>(IBinaryStream stream, Func<IBinaryStream, T> readItem)
    {
        var count = ReadCount(stream);
        var values = new List<T>(count);
        for (var i = 0; i < count; i++)
            values.Add(readItem(stream));
        return values;
    }
}
