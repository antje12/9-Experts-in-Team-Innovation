from confluent_kafka.avro import AvroProducer
from confluent_kafka import avro 
import time
import random

producer_config = {
    "bootstrap.servers":"kafka-1:9092",
    "schema.registry.url":"http://localhost:8081"
}

Producer = AvroProducer(producer_config, default_value_schema=avro.load("../../Kafka/Avro/leak.avsc"))

data = []
file_data = open("./Aguardio Leak Sensor Data.csv") 
for line in file_data: 
    str = line.split(',')
    data.append({
        "DataRaw_id": str[0],
        "DCreated": str[1],
        "DReported": str[2],
        "DLifeTimeUseCount": str[3], 
        "LeakLevel_id": str[4],
        "Sensor_id": str[5],
        "DTemperatureOut": str[6],
        "DTemperatureIn": str[7]
        })

while(True):
    try:

        Producer.produce(topic  ="leak", 
        value = data[random.randint(1,len(data))]
        )

        Producer.flush()
        time.sleep(1)
    except Exception as error:
        print("something went wrong: ", error)
        break
