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

Producer = AvroProducer(producer_config, default_value_schema=avro.load("./shower.avsc"))


data = []
file_data = open("./Aguardio Shower Sensor Data.csv")
for line in file_data: 
    str = line.split(';')
    data.append({
        "DataRawId": str[0],
        "DCreated": fake.date_time_between(start_date='-5y', end_date='now').strftime("%d/%m/%Y %H.%M.%S"),
        "DReported": fake.date_time_between(start_date='-5y', end_date='now').strftime("%d/%m/%Y %H.%M.%S"),
        "SensorId": str[3], 
        "DShowerState": str[4],
        "DTemperature": str[5],
        "DHumidity": str[6],
        "DBattery": str[7]
        })


while(True):
    try:

        Producer.produce(topic  = "shower", 
        value = data[random.randint(1,len(data))]
        )

        Producer.flush()
        time.sleep(0.05)
    except Exception as error:
        print("something went wrong: ", error)
        break

