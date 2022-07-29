# Unity Asset Checker
A tool that looks for missing references in your Asset folder.

## How to use ##

1) Open the Window menu and press *"Asset Checker"* and you'll see this window

![2](https://user-images.githubusercontent.com/62873054/181507561-c59b0fca-8fc0-4e36-84b7-72be992122b5.png)

2) Press *"Check assets"* button

3) Since check is finished you see a new window with such list 

![2](https://user-images.githubusercontent.com/62873054/181740931-20edd692-114c-497d-abc9-fd9b4388c2c0.png)

Left column shows path to assets that're missing references, while right one shows types of their missing components and missing references inside their components.

You can press on any element in the right column to select asset.

![1](https://user-images.githubusercontent.com/62873054/181742271-895e4f1c-e956-4975-b6d6-972f6b26d8b4.png)


## Principals of work ##

1) Gets an array of asset paths through `AssetDatabase.GetAllAssetPaths()`

2) Retrieves `GameObject` from each path using `AssetDatabase.LoadAssetAtPath<GameObject>(path)` and checks for null

3) Then checks each `GameObject` and its children for:
  - missing components via `!` operator (in this case the operator shows `Object` existance what is described here https://docs.unity3d.com/ScriptReference/Object-     operator_Object.html).
  - component internal references that are missing via `new SerializedObject(component)` that makes us able to iterate `SerializedProperty`'s and check them for:
    - `serializedProperty.propertyType == SerializedPropertyType.ObjectReference` (property derives from `UnityEngine.Object`)
    - `serializedProperty.objectReferenceInstanceIDValue != 0` (property has value)
    - `serializedProperty.objectReferenceValue == null` (has no or missed reference)

I used this logic, cause of the only one way i found.
