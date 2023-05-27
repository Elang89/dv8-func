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

Additionally some tests were added to this function, if you check the github actions tab on the repository,
they will show the results of some jobs created to run these tests. There is also a data folder at the root of Q1 which shows the results of the azure function. 

### Q4

To run Q4 locally do the following: 

1. Run `cd Q4/client`
2. Run `npm run dev`

Besides the fixes that were asked for the following other improvements were made: 

1. Functions were refactored and changed from anonymous arrow functions to defined functions. 
2. Typescript was added and the project was updated to use it. 
3. Prop definitions were created with typescript's interfaces. 