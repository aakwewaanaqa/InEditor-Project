# Bienvenido a el InEditor

This is a package underdevelopment for easy
creating class `Editor`. The current version
is not complete yet but you can test it with
some easy steps.

## Step 1

Create a `Editor` class in any folder in the Assets only its name is Editor,
just as a normal `Editor`.

```C#
using UnityEditor;

namespace Anyway
{
    [CustomEditor(typeof(YourClass))]
    public class YourClassEditor : Editor
    {
    
    }
}
```

## Step 2

Use `namespace` `InEditor`.
Change where the class inheritance from `Editor` to [BaseInEditor](https://github.com/aakwewaanaqa/InEditor-Project/blob/update/Editor/Class/BaseInEditor.cs),
and this will do the trick.

```C#
using UnityEditor;
using InEditor;

namespace Anyway
{
    [CustomEditor(typeof(YourClass))]
    public class YourClassEditor : BaseInEditor
    {
    
    }
}
```

## Step 3

Add what you what to show in `Inspector` with [InEditorAttribute](https://github.com/aakwewaanaqa/InEditor-Project/blob/update/Runtime/Attribute/InEditorAttribute.cs).
The number in the `Attribute` means the [DisplayOrder](https://github.com/aakwewaanaqa/InEditor-Project/blob/update/Runtime/Attribute/InEditorAttribute.cs#L15-L15) of the `field`.

```C#
using UnityEditor;
using InEditor;

namespace Anyway
{
    [CustomEditor(typeof(YourClass))]
    public class YourClassEditor : BaseInEditor
    {
        [InEditor(1)]
        [SerializeField]
        private bool boolean;
    }
}
```