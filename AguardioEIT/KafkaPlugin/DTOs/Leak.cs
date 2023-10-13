using Avro;
using Avro.Specific;

namespace KafkaPlugin.DTOs;

public partial class Leak : ISpecificRecord
{
    public static Schema _SCHEMA = Avro.Schema.Parse(
        @"{
	        ""namespace"": ""git.avro"",
	        ""type"": ""record"",
	        ""name"": ""Leak"",
	        ""fields"": [
		        {""name"": ""DataRaw_id"",  ""type"": ""string""},
		        {""name"": ""DCreated"", ""type"": ""string""},
		        {""name"": ""DReported"", ""type"": ""string""},
		        {""name"": ""DLifeTimeUseCount"", ""type"": ""string""},
		        {""name"": ""LeakLevel_id"", ""type"": ""string""},
		        {""name"": ""Sensor_id"", ""type"": ""string""},
		        {""name"": ""DTemperatureOut"", ""type"": ""string""},
		        {""name"": ""DTemperatureIn"", ""type"": ""string""}
	        ]
        }");

    private string _DataRaw_id;
    private string _DCreated;
    private string _DReported;
    private string _DLifeTimeUseCount;
    private string _LeakLevel_id;
    private string _Sensor_id;
    private string _DTemperatureOut;
    private string _DTemperatureIn;

    public virtual Schema Schema
    {
        get { return Leak._SCHEMA; }
    }

    public string DataRaw_id
    {
        get { return this._DataRaw_id; }
        set { this._DataRaw_id = value; }
    }

    public string DCreated
    {
        get { return this._DCreated; }
        set { this._DCreated = value; }
    }

    public string DReported
    {
        get { return this._DReported; }
        set { this._DReported = value; }
    }

    public string DLifeTimeUseCount
    {
        get { return this._DLifeTimeUseCount; }
        set { this._DLifeTimeUseCount = value; }
    }

    public string LeakLevel_id
    {
        get { return this._LeakLevel_id; }
        set { this._LeakLevel_id = value; }
    }

    public string Sensor_id
    {
        get { return this._Sensor_id; }
        set { this._Sensor_id = value; }
    }

    public string DTemperatureOut
    {
        get { return this._DTemperatureOut; }
        set { this._DTemperatureOut = value; }
    }

    public string DTemperatureIn
    {
        get { return this._DTemperatureIn; }
        set { this._DTemperatureIn = value; }
    }

    public virtual object Get(int fieldPos)
    {
        switch (fieldPos)
        {
            case 0: return this.DataRaw_id;
            case 1: return this.DCreated;
            case 2: return this.DReported;
            case 3: return this.DLifeTimeUseCount;
            case 4: return this.LeakLevel_id;
            case 5: return this.Sensor_id;
            case 6: return this.DTemperatureOut;
            case 7: return this.DTemperatureIn;

            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
        }
    }

    public virtual void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0:
                this.DataRaw_id = (System.String)fieldValue;
                break;
            case 1:
                this.DCreated = (System.String)fieldValue;
                break;
            case 2:
                this.DReported = (System.String)fieldValue;
                break;
            case 3:
                this.DLifeTimeUseCount = (System.String)fieldValue;
                break;
            case 4:
                this.LeakLevel_id = (System.String)fieldValue;
                break;
            case 5:
                this.Sensor_id = (System.String)fieldValue;
                break;
            case 6:
                this.DTemperatureOut = (System.String)fieldValue;
                break;
            case 7:
                this.DTemperatureIn = (System.String)fieldValue;
                break;

            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }
}