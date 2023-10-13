using Avro;
using Avro.Specific;

namespace KafkaPlugin.DTOs;

public partial class Shower : ISpecificRecord
{
    public static Schema _SCHEMA = Avro.Schema.Parse(
        @"{
	        ""namespace"": ""git.avro"",
	        ""type"": ""record"",
	        ""name"": ""Shower"",
	        ""fields"": [
		        {""name"": ""DataRawId"",  ""type"": ""string""},
		        {""name"": ""DCreated"", ""type"": ""string""},
		        {""name"": ""DReported"", ""type"": ""string""},
		        {""name"": ""SensorId"", ""type"": ""string""},
		        {""name"": ""DShowerState"", ""type"": ""string""},
		        {""name"": ""DTemperature"", ""type"": ""string""},
		        {""name"": ""DHumidity"", ""type"": ""string""},
		        {""name"": ""DBattery"", ""type"": ""string""}
	        ]
        }");

    private string _DataRawId;
    private string _DCreated;
    private string _DReported;
    private string _SensorId;
    private string _DShowerState;
    private string _DTemperature;
    private string _DHumidity;
    private string _DBattery;

    public virtual Schema Schema
    {
        get { return Shower._SCHEMA; }
    }

    public string DataRawId
    {
        get { return this._DataRawId; }
        set { this._DataRawId = value; }
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

    public string SensorId
    {
        get { return this._SensorId; }
        set { this._SensorId = value; }
    }

    public string DShowerState
    {
        get { return this._DShowerState; }
        set { this._DShowerState = value; }
    }

    public string DTemperature
    {
        get { return this._DTemperature; }
        set { this._DTemperature = value; }
    }

    public string DHumidity
    {
        get { return this._DHumidity; }
        set { this._DHumidity = value; }
    }

    public string DBattery
    {
        get { return this._DBattery; }
        set { this._DBattery = value; }
    }

    public virtual object Get(int fieldPos)
    {
        switch (fieldPos)
        {
            case 0: return this.DataRawId;
            case 1: return this.DCreated;
            case 2: return this.DReported;
            case 3: return this.SensorId;
            case 4: return this.DShowerState;
            case 5: return this.DTemperature;
            case 6: return this.DHumidity;
            case 7: return this.DBattery;

            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
        }
    }

    public virtual void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0:
                this.DataRawId = (System.String)fieldValue;
                break;
            case 1:
                this.DCreated = (System.String)fieldValue;
                break;
            case 2:
                this.DReported = (System.String)fieldValue;
                break;
            case 3:
                this.SensorId = (System.String)fieldValue;
                break;
            case 4:
                this.DShowerState = (System.String)fieldValue;
                break;
            case 5:
                this.DTemperature = (System.String)fieldValue;
                break;
            case 6:
                this.DHumidity = (System.String)fieldValue;
                break;
            case 7:
                this.DBattery = (System.String)fieldValue;
                break;

            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        }
    }
}