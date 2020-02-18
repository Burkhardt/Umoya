# UMOYA CLI Cheatsheet

## Install
        dotnet tool install -g umoya --add-source https://hub.umoya.ai/v3/index.json

## Uninstall
        dotnet tool uninstall -g umoya
        
## Upgrade UMOYA CLI Tool
        dotnet tool update -g umoya --add-source https://hub.umoya.ai/v3/index.json

## Initialize your project directory
        umoya init
        
## Configure your UMOYA\Repo server 
        umoya info --repo-url https://hub.umoya.ai/v3/index.json
        
## Publish resource with/without dependencies
        umoya publish "C:\temp\HelloWorld.pmml"@1.0.0 --description "Hello World Model" --tags MyModel,Testing
        umoya publish HelloWorld.pmml@1.0.0 --description "Hello World Model" --authors Rainer --owners Vinay
        umoya publish HelloWorld.pmml@1.0.0 --description "Hello World Model" --using HelloWorldCode.ipynb@1.0.0
        umoya publish HelloWorld.pmml@1.0.0 --description "Hello World Model" --using HelloWorldCode.ipynb@1.0.0,HelloWorldData.csv@1.0.0
        

## List your local resources
        umoya list [options]
        umoya list --class Model
        umoya list --class Code
        umoya list --class Data
        umoya list --query "HelloWorld"
        umoya list --take 20
        umoya list --skip 10
        
## List resources from UMOYA\Repo server
        umoya list --from repo [options]
        umoya list --from repo --class Model
        umoya list --from repo --class Code
        umoya list --from repo --class Data
        umoya list --from repo --query "HelloWorld"
        umoya list --from repo --take 20
        umoya list --from repo --skip 10
        
## Version
        umoya
