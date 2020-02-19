## Use Case : 
You have resource(s) Model, Code and Data and You want to publish on Repo (https://hub.mlw.ai) with or without dependencies.<br/><br/>
Note : Make sure that You have UMOYA CLI Tool installed on your system. You can just type 'umoya' in commanline and check version 2.6.2 or latest.If you do not have then please follow this page : [Installation](https://github.com/Umoya-ai/UMOYA/blob/master/docs/sample%20and%20training%20-%20usecases/install%20umoya%20cli%20tool.md) and [Configuration](https://github.com/Umoya-ai/UMOYA/blob/master/docs/sample%20and%20training%20-%20usecases/init%20or%20configure%20umoya%20cli%20tool.md) Page

### Steps
You can publish resource with or without dependencies. 
Publish Data without dependency<br>
umoya publish C:\temp\HelloWorldData.csv@1.0.0 --description "Hello World Data" --authors Rainer --owners Vinay -t MyModel,Testing
Publish Code and Model with direct dependencies<br/>
umoya publish C:\temp\HelloWorldCode.ipynb@1.0.0 --description "Hello World Model Code" --authors Vinay --owners Swapnil --tags Testing --using HelloWorldData.csv@1.0.0<br/>
umoya publish C:\temp\HelloWorld.ipynb@1.0.0 --description "Hello World Model" --authors Anil --owners Vinay --tags Testing --using HelloWorldCode.ipynb@1.0.0

Once It is published, You can go to web-portal and verify your resource or you can [query resource](https://github.com/Umoya-ai/UMOYA/blob/master/docs/sample%20and%20training%20-%20usecases/query%20resource%20on%20repo%20or%20locally.md).
