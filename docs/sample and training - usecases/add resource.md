## Use Case : 

Repo (https://hub.mlw.ai﻿) has Model (HelloWorld.pmml),  You want to add into your local ZMOD drive.

### Steps
1. Go to your ZMOD directory.
2. If you do not have ZMOD local directory, You create one directory and configure from here.
3. Go to Repo Web portal and find your interested resource and version to add.
4. Go to detail resource page and copy umoya add command.

For example, If you are interested for HelloWorld.pmml model , version 3.0.0 then you can run below command
umoya add HelloWorld.pmml@3.0.0
To add latest version , You can avoid giving version.
umoya add HelloWorld.pmml
You can verify for resource and its dependencies in local ZMOD directory with 'umoya list' action
umoya list
