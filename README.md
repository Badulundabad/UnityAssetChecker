# Unity Asset Checker
This is a tool for Unity Editor to look for missing references in assets.

## Supported types ##
- GameObject (with any components inside)
- Scene (with objects inside)
- Material

## How to use ##

1) Open the Window menu and press *"Asset Checker"* and you'll see this window

![2](https://user-images.githubusercontent.com/62873054/181507561-c59b0fca-8fc0-4e36-84b7-72be992122b5.png)

2) Press *"Check assets"* button to run check

3) Since the check is finished you see a new window with two collapsed lists

![image](https://user-images.githubusercontent.com/62873054/182186385-4e473688-7fe8-4b4e-bee7-a9284adf8732.png)

![image](https://user-images.githubusercontent.com/62873054/182187610-318f5738-2e8d-4c89-abef-9747df9cb0ba.png) 

Structure of list elements:
- for asset object list - *Object name* | *Component type*
- for scene object list - *Scene name*  | *Object name* | *Component type*

4) Press a button next to any object in the lists to go to the object 

![image](https://user-images.githubusercontent.com/62873054/182190853-710de032-e0be-4e0c-870a-a72ac0c9028e.png)

- Object will be opened in Prefab editor if it's missing its script 
- Object will be opened in Inspector if its compponent missing any reference value
- If the object is on a scene that isn't loaded then it asks you to load this scene

## Principals of work ##

1) Gets an array of asset paths through `AssetDatabase.GetAllAssetPaths()`

2) Retrieves `GameObject` from each path using `AssetDatabase.LoadAssetAtPath<GameObject>(path)` and checks for null

3) Then checks each `GameObject` and its children for:
  - missing components via `!` operator (in this case the operator shows `Object` existance what is described here https://docs.unity3d.com/ScriptReference/Object-     operator_Object.html).
  - component internal references that are missing via `new SerializedObject(component)` that makes us able to iterate `SerializedProperty`'s and check them for:
    - `serializedProperty.propertyType == SerializedPropertyType.ObjectReference` (property derives from `UnityEngine.Object`)
    - `serializedProperty.objectReferenceInstanceIDValue != 0` (property has value)
    - `serializedProperty.objectReferenceValue == null` (has no or missed reference)
