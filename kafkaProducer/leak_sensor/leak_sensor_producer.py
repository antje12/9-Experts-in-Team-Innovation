from kafka import KafkaProducer
import json
import time
import random
import os

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
        "DTempertureIn": str[7]
        })

producer = KafkaProducer(
    bootstrap_servers='192.168.1.71:9092',
    value_serializer=lambda v: json.dumps(v).encode('ascii')
)

while(True):
    try:
        producer.send('test',value=data[random.randint(1,len(data))])
        producer.flush()
        time.sleep(1)
    except:
        print("something went wrong")
        break


producer.flush()