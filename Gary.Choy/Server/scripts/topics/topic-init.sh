#!/bin/bash
# set -x

echo -e "\nStarting Topic Init... Waiting for Rest Proxy"
while [ "`curl -s http://rest-proxy:8082/brokers`" != "{\"brokers\":[1,2,3]}" ]
do 
    echo -e $(date) "Waiting for all three Brokers"
    sleep 5
done  

echo -e "\nCreating topics"

kafka-topics --bootstrap-server broker1:29092,broker2:29093,broker3:29094 --delete --if-exists --topic test_topic
kafka-topics --bootstrap-server broker1:29092,broker2:29093,broker3:29094 --create --if-not-exists --topic test_topic --replication-factor 3 --partitions 10

kafka-topics --bootstrap-server broker1:29092,broker2:29093,broker3:29094 --delete --if-exists --topic capability_topic
kafka-topics --bootstrap-server broker1:29092,broker2:29093,broker3:29094 --create --if-not-exists --topic capability_topic --replication-factor 3 --partitions 10

kafka-topics --bootstrap-server broker1:29092,broker2:29093,broker3:29094 --delete --if-exists --topic location_topic
kafka-topics --bootstrap-server broker1:29092,broker2:29093,broker3:29094 --create --if-not-exists --topic location_topic --replication-factor 3 --partitions 10

kafka-topics --bootstrap-server broker1:29092,broker2:29093,broker3:29094 --delete --if-exists --topic resource_topic
kafka-topics --bootstrap-server broker1:29092,broker2:29093,broker3:29094 --create --if-not-exists --topic resource_topic --replication-factor 3 --partitions 10

echo -e "\nAll Topics"
kafka-topics --bootstrap-server broker1:29092 --list