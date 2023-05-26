# Dv8 Test Project

## Description

This is the test project for Dv8, in this project four folders are contained. 
These are Q1-4 and they correspond to the four questions present in this test. 


## How to run

### Q1

To run Q1 locally do the following:

1. Run `cd Q1/Dv8Timed.Func`
2. Run `docker pull mcr.microsoft.com/azure-storage/azurite`
3. Create a docker container with the command `docker run -p 10000:10000 -p 10001:10001 -p 10002:10002 -d --name az_emulator \
    mcr.microsoft.com/azure-storage/azurite`
4. Run `func start`

### Q4

To run Q4 locally do the following: 
