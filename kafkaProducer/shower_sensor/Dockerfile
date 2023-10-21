FROM python:3.11

WORKDIR /usr/app/src

COPY . ./

RUN pip install kafka-python

CMD ["python", "./shower_sensor_producer.py"]