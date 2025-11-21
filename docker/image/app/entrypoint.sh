#!/bin/bash

# Check if information about Redis IP and hostname is available in environment variables and register the information in /etc/hosts.
# This is needed for the application to be able to validate Redis' TLS certificate and avoid errors when connecting to Redis:
if [[ "${Cache__RedisEasyCaching__DBConfig__Host}" != "" ]] && [[ "${Cache__RedisEasyCaching__DBConfig__IP}" != "" ]] ; then
  echo "" >> /etc/hosts
  echo "${Cache__RedisEasyCaching__DBConfig__IP} ${Cache__RedisEasyCaching__DBConfig__Host}" >> /etc/hosts
fi

# Start application:
cd /app || exit
dotnet Agora.Api.dll
