using MongoDB.Bson.Serialization;

namespace Common.Util;

public class Int32IdGenerator : IIdGenerator
{
    public object GenerateId(object container, object document)
    {
        return (int)DateTime.Now.Ticks;
    }

    public bool IsEmpty(object id)
    {
        return (int)id == 0;
    }
}