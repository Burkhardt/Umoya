## Use Case
Repo (https://hub.mlw.ai﻿) has Model (HelloWorld.pmml),  You want to add into your local ZMOD drive.

### Steps
1. Go to your local resource directory (ZMOD).
2. If you do not have local directory, You create one directory and configure from [here](https://github.com/Umoya-ai/UMOYA/blob/master/docs/sample%20and%20training%20-%20usecases/init%20or%20configure%20umoya%20cli%20tool.md).
3. Go to Repo Web portal and find your interested resource and version to add.
4. Go to detail resource page and copy umoya add command.

For example, If you want to add HelloWorld.pmml model, version 3.0.0 then you can run below command<br/>
umoya add HelloWorld.pmml@3.0.0

To add latest version , You can avoid giving version.<br/>
umoya add HelloWorld.pmml

Note : 
Once It is added, You can verify for resource and its dependencies in local resource directory or use UMOYA [list action](https://github.com/Umoya-ai/UMOYA/blob/master/docs/sample%20and%20training%20-%20usecases/query%20resource%20on%20repo%20or%20locally.md)
