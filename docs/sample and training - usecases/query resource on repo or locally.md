## Use Case
To view list of resource(s) from repo or local resource directory (ZMOD) or You want to query to drill down resources which are interested by class and query string.</br>

Note : 
Ensure you have umoya cli tool installed. You can just type 'umoya' in commanline and check version 2.4.2 is installed or not.
If you do not have then please follow this page : [Installation](https://github.com/Umoya-ai/UMOYA/blob/master/docs/sample%20and%20training%20-%20usecases/install%20umoya%20cli%20tool.md) and [Configuration](https://github.com/Umoya-ai/UMOYA/blob/master/docs/sample%20and%20training%20-%20usecases/init%20or%20configure%20umoya%20cli%20tool.md) 

### Steps
* To get the list of your resource from local ZMOD directory or Repo server, use the below command:</br>
  umoya list</br>
  umoya list –from repo

* To get the resources from a specific server, use below commandv
  umoya list –from repo -repo-url "Repo Server URL"</br>
  For example:  umoya list –from repo -repo-url "https://hub.mlw.ai"

* To get list from Repo server based on Type/Class(Model, Code, Data) use below command</br>
  umoya list –from repo –class Model

* To save all from ZMOD local directory or Repo Server into some output file, use the below command</br>
  umoya list –file MyLocalData.json</br>
  umoya list –from repo –file MyServerData.json

* You can filter the list for our ZMOD drive or Repo server by using the below command</br>
  umoya list –from repo –query "some query string"</br>
  umoya list –query "some query string"
