# DV8 React Interview Challenge

## Starting and submitting the project
1. To install the dependencies run
```
	npm install 
```

2. To start the dev server, run 
```
	npm run dev
```

3. To create a git bundle, commit all your changes and run  

```
	git bundle create yourname-frontend-solution.bundle HEAD master
```

4. Submit your solution by emailing the bundle file back to us.

## Description
In order for our software to run on devices in the field, we need to have a record of the rod string data to perform calculations.
The rod string is broken up into tapers which are defined by a type (Polished rod, Steel, or Fiberglass), a length in feet, and a diameter in inches. 
This repository contains a simple interface for updating the rod string data of a given well. It is not currently functioning properly. Your task is to fix the bugs and to implement some improvements.	

## Requirements
1. Fix the 'Add' button and implement the 'Delete' button for modifying the rod string data.
2. A taper can only be a Polished Rod if it is the FIRST taper in a rod string. Disable the Polished Rod option in the add form unless it is the first taper in the rod string data.
3. Refactor the fetch function into a reusable hook for fetching data from a url and setting state on page load.
4. Make any other changes / improvements you feel are necessary.

## Notes
If you would like to make any comments on the code, you can edit this file and leave them below.
1. Your comments go here.
