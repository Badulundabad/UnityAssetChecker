# Unity Asset Checker
A tool that looks for missing references in your Asset folder.

## How to use ##

Open the Window menu and press *"Asset Checker"*.

There are two ways of using the tool:
- press *"Find missing references"* button to scan your assets once 
- enable *"Update loop"* check box to scan your assets every `OnGUI` call (since it's enabled *"Find missing references"* button is hidden)

![3](https://user-images.githubusercontent.com/62873054/181435526-34b7a8e7-2909-4756-a143-755de0918a74.png)

Every time a scaning is finished you see a list with two columns. Left one shows assets and asset children down through hierarchy that're missing references, while right one shows their missing components number (in hierarchy) and missing references inside their components.
![4](https://user-images.githubusercontent.com/62873054/181437334-646d611e-7376-49b2-8f10-5b3abdef6f09.png)

Also you can see an error message which says *"Asset checker has found X missing references"* where X is amount of references.
![image](https://user-images.githubusercontent.com/62873054/181437775-4fca13e5-df0e-485e-8f9c-f4c5c180f258.png)

## Principals of work ##

1) First off, it gets array of asset paths (where can be folders as well) via `AssetDatabase`
2) Filters it with *"Assets"* word to exlude assets external packagaes
3) Gets `GameObject` from each path and checks for null
4) Then it runs method on each `GameObject` that recursievly checks it and its children for:
- missing components via `!` operator (in this case the operator shows Object existance what is described here https://docs.unity3d.com/ScriptReference/Object-operator_Object.html).

![image](https://user-images.githubusercontent.com/62873054/181443523-4afc8468-c7d2-44ca-8f43-0b2ff7fff496.png)

- component internal references that are missing via `new SerializedObject(component)` that makes us able to iterate `SerializableField's` 

![image](https://user-images.githubusercontent.com/62873054/181443563-965de641-540b-4606-98d6-e0d10e0ef111.png)

I used this logic, cause of the only one way i found.
