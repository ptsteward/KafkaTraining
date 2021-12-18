#!/bin/bash

echo -e "\nStarting ksqlDB Init... Waiting for Topics Init"
while [ $(curl -s -o /dev/null -w %{http_code} http://rest-proxy:8082/topics/resource_topic) -ne 200 ]
do 
    echo -e $(date) "Waiting for last Topic to exist"
    sleep 5
done

echo -e "\nStarting ksqlDB Init... Waiting for Schema Init"
while [ $(curl -s -o /dev/null -w %{http_code} http://schema-registry:8081/subjects/resource_topic-value/versions/latest/schema) -ne 200 ]
do 
    echo -e $(date) "Waiting for last Schema to exist"
    sleep 5
done

echo -e "\nRunning KSQL"
cat /data/scripts/test.ksql \
/data/scripts/resources2.ksql \
/data/scripts/list_things.ksql \
| ksql http://ksqldb-server:8088

#/data/scripts/resources.ksql \
#/data/scripts/resources2.ksql \
#/data/scripts/test.ksql \
#/data/scripts/resource_location.ksql \
