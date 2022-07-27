# Unity Asset Checker
A tool that looks for missing references in your Asset folder.

## How to use ##

Open the Window menu and press "Asset Checker".

There are two ways of using the tool (look at picture #1):
- press "Find missing references" button to scan your assets once 
- enable "Update loop" check box to scan your assets every OnGUI call (since it's enabled "Find missing references" button is hidden)

Every time a scaning is finished you see the list with two columns. Left one shows asset that's missing reference or asset children down through hierarchy. Right column shows number of missing component in asset hierarchy or missing reference of component property.

Also you can see error message which says "Asset checker has found X missing references" where X is amount of references.

## Principal logic ##

This tool's using:
1) '!' opeator to check Component for existance what's described here https://docs.unity3d.com/ScriptReference/Object-operator_Object.html
2) objectReferenceValue and objectReferenceInstanceIDValue properties to check Component property value