# Bienvenido a el InEditor

This is a package underdevelopment for easy
creating class `Editor`. The current version
is not complete yet but you still can test it with
some easy steps.

----

## How to create a BaseInEditor

### Step 1

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

### Step 2

Use `namespace` `InEditor`.
Change where the class inheritance from `Editor` to `BaseInEditor`,
and this will do the trick.

```C#
using UnityEditor;
using InEditor; // Use InEditor

namespace Anyway
{
    [CustomEditor(typeof(YourClass))]
    public class YourClassEditor : BaseInEditor // Inherite BaseInEditor
    {
    
    }
}
```

### Step 3

Add what you what to show in `Inspector` with `InEditorAttribute`.
The number in the `Attribute` means the `DisplayOrder` of the `field`.

```C#
using UnityEditor;
using InEditor;

namespace Anyway
{
    [CustomEditor(typeof(YourClass))]
    public class YourClassEditor : BaseInEditor
    {
        [InEditor(1)] // Add InEditorAttribute for the field
        [SerializeField]
        private bool boolean;
    }
}
```