#!/usr/bin/env bash
# set -x

while getopts p:s:r: flag
do
    case "${flag}" in
        p) proto=${OPTARG};;
        s) subject=${OPTARG};;
        r) references=${OPTARG};;
    esac
done

function registry_body {
    refs=$([ ${2} ] && echo "`cat ${2}`" || echo "[]")
    cat <<EOF
    {
        "schemaType": "PROTOBUF",
        "schema": "${1}",
        "references": ${refs}
    } 
EOF
}

BOM=$'\xEF\xBB\xBF'
rawSchema="`cat "${proto}"`"
noBOM=${rawSchema/"${BOM}"/}
escapedSchema=${noBOM//\"/\\\"}
curl -silent -X POST "http://schema-registry:8081/subjects/${subject}/versions" -H "Content-Type: application/vnd.schemaregistry.v1+json" --data @<(cat <<EOF 
    $(registry_body "${escapedSchema}" "${references}") 
EOF
)