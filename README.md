# Unity Asset Checker
A tool that looks for missing references in your Asset folder.

## How to use ##

1) Open the Window menu and press *"Asset Checker"* and you'll see this window

![2](https://user-images.githubusercontent.com/62873054/181507561-c59b0fca-8fc0-4e36-84b7-72be992122b5.png)

2) Press *"Check assets"* button

![3](https://user-images.githubusercontent.com/62873054/181507860-96a55b30-0aab-467b-8f12-18c7711fa8e7.png)

Every time a checking is finished you see a new window with list. 
![4](https://user-images.githubusercontent.com/62873054/181508271-36c5349a-e463-46ae-a295-c2975de9069f.png)

Left column shows assets and asset children down through hierarchy that're missing references, while right one shows their missing components number (in hierarchy) and missing references inside their components.

## Principals of work ##

1) First off, it gets array of asset paths using `AssetDatabase.GetAllAssetPaths()`
2) Gets `GameObject` from each path using `AssetDatabase.LoadAssetAtPath<GameObject>(path)` and checks for null
3) Then it passes each `GameObject` to `Check` method that recursively checks given object and its children for:
- missing components via `!` operator (in this case the operator shows Object existance what is described here https://docs.unity3d.com/ScriptReference/Object-operator_Object.html).

- component internal references that are missing via `new SerializedObject(component)` that makes us able to iterate `SerializableField's` 

I used this logic, cause of the only one way i found.
