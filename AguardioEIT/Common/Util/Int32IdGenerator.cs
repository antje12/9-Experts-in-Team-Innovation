using MongoDB.Bson.Serialization;

namespace Common.Util;

public class Int32IdGenerator : IIdGenerator
{
    /**
     * Not a good solution, but it works for now.
     */
    public object GenerateId(object container, object document)
    {
        Random random = new();
        return random.Next(int.MaxValue);
    }

    public bool IsEmpty(object id)
    {
        return (int)id == 0;
    }
}