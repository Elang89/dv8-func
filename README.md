docker pull mcr.microsoft.com/azure-storage/azurite

docker run -p 10000:10000 -p 10001:10001 -p 10002:10002 -d --name az_emulator \
    mcr.microsoft.com/azure-storage/azurite