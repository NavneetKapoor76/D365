#General
	All on Dev CMD
		%comspec% /k "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\Tools\VsDevCmd.bat"

#create project
	1.Create a folder
	2.Launch Dev CMD
		
	3.Generate a Namespace and a component
		pac pcf init --namespace BDP.PCF --name CheckPhoneNumber --template field
	4. run npm install for the project
	5. create a subfolder SolutionPackage

#start a component
	cd ./PCF
	start npm start

#package publisher name
	.\SolutionPackage>pac solution init --publisher-name BDP --publisher-prefix BDP

#package a component :
	.\SolutionPackage>PAC SOLUTION ADD-REFERENCE --PATH C:\source_pcf\PCF\

#Update Solution Name
update solution.xml

#Build a solution
	.\SolutionPackage>"C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\amd64\MSBuild.exe" /t:build /restore

//File is here : .\SolutionPackage\bin\Debug