FROM python:3.11

WORKDIR /usr/app/src

COPY . ./

RUN pip install kafka-python

CMD ["python", "./leak_sensor_producer.py"]