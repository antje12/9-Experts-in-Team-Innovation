from confluent_kafka.avro import AvroProducer
from confluent_kafka import avro 
import time
import random
from faker import Faker
from datetime import datetime

fake = Faker()

producer_config = {
    "bootstrap.servers":"kafka-1:9092",
    "schema.registry.url":"http://schema-registry:8081"
}

Producer = AvroProducer(producer_config, default_value_schema=avro.load("./leak.avsc"))

data = []
file_data = open("./Aguardio Leak Sensor Data.csv") 
for line in file_data: 
    str = line.split(',')
    data.append({
        "DataRaw_id": str[0],
        "DCreated": fake.date_time_between(start_date='-5y', end_date='now').strftime("%Y-%m-%d %H:%M:%S"),
        "DReported": fake.date_time_between(start_date='-5y', end_date='now').strftime("%Y-%m-%d %H:%M:%S"),
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
