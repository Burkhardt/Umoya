# Draft

## Init
Initialize ZMOD local directory first time with Umoya configurations.
1. Owner Name
2. Setup ZMOD Environment Path
3. Setup Umoya Template Resource Project
4. Setup Umoya local Path in Environment
        
        Syntax :
        umoya init --umoya-home path-of-umoya --zmod-home -path-of-zmod --owner {default is current system user|as per input}


## Remote
Setup UmoyaSource Info
Setup Access Key and URL for local nuget.config. This will be used when publishing artifacts.
        
	Syntax : 
        umoya remote --server-url {url} --accesskey {value} 
        

## Info
Show information about Umoya server and local ZMOD setup configurations.
Show information about interested resource which is from local and Umoya server.

        Syntax : 
        umoya info
        umoya info --resource {resource-name}  --version {value}
	

## Spec (Private Command for Dev Testing)
Build spec for Umoya Resources meta data
1. Setup resource-spec template with contentFiles/<resource type folder>/<resource fie>
2. Populate spec with params
* ResourceName	        -> id
* Authors		-> authors
* Version		-> version
* Owners			-> owners
* Description		-> description
* Type			-> update IconUrl and Tags
* Tags			-> user define Tag (optional)
* ContentFiles	        -> file include resource type folder and resource as relative path
* Dependencies	        -> Umoya Resource and Version (Semantic Version)
                           Dependencies/dependency with id and version
      
       Syntax : 
       Umoya spec build --{key} {value}
                                               
Upgrade spec for Umoya existing resource with above attributes.
        
        Syntax : 
        Umoya spec build --spec <file> --{key} {value}
	
## Pack (Private Command for Dev Testing)
Pack spec with Umoya resource and build package to publish folder.

        Syntax : Umoya pack


## Publish
Publish resource like Model, Data and Code on Repo. Make sure you have configured repo and access key with command : Remote.
1. It prepares spec with meta data like description, owner, author, version, tags and dependencies.
2. It creates nuget Package dynamically with spec.
4. It publishes package on repo with access key as configured by remote command.

        Syntax : 
        umoya publish HelloWorld.pmml@1.2.1 --using XCode.py@1.0.0 --description "Hellow World"
        umoya publish HelloWorld.pmml@1.2.1 -u XCode.py@1.0.0 -d "Hellow World" -o Nimesh,Rainer


## Add
Add resource from Umoya into ZMOD directory (locally)
* It adds the resource from Umoya along with It also resolves dependency.
        
        Syntax : 
        Umoya add resource-name --version resource-version-value
		
		
## Delete 
Remove resource from ZMOD directory (locally) or from Umoya (remove from repository server)
* dotnet remove resource-name -> To remove resource locally.
* dotnet nuget delete 
        
        Syntax : 
        Umoya delete resource-name --from local -> removes resource which is added in local ZMOD directory.
        Umoya delete resource-name --version version-value --from server
		

## List 
List all ZMOD local directory's resources or Umoya server resources with meta data such as type, 
description, author, local version, available version, size.
        
	Syntax : 
        Umoya list
        Umoya list --query <search string>
        Umoya list --type <resource type like model, data, code or other>
        Umoya list --from server
        Umoya list --query <search string> --from server
        Umoya list --type <resourc type like model, data, code or other> --from server

				
## Upgrade 
Upgrades local ZMOD resources' with latest version of resources available from Umoya
        
	Syntax :
        Umoya upgrade
