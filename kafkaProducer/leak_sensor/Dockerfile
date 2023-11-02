FROM python:3.11

WORKDIR /usr/app/src

COPY . ./

RUN pip install kafka-python
RUN pip install confluent_kafka
RUN pip install avro
RUN pip install urllib3
RUN pip install requests


CMD ["python", "./leak_sensor_producer.py"]
