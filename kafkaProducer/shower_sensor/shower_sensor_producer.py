from kafka import KafkaProducer
import json
import time
import random

data = []
file_data = open("./Aguardio Shower Sensor Data.csv")
for line in file_data: 
    str = line.split(';')
    data.append({
        "DataRawId": str[0],
        "DCreated": str[1],
        "DReported": str[2],
        "SensorId": str[3], 
        "DShowerState": str[4],
        "DTemperature": str[5],
        "DHumidity": str[6],
        "DBattery": str[7]
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