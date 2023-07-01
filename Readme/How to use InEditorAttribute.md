## How to use InEditorAttribute

We use `InEditorAttribute` to tell `Editor` to draw field with designated functionalities. Be sure that
both `InEditorAttribute` and `BaseInEditor` have to exist at the same time to work properly.

### Basic usage

The `InEditorAttribute` can make non-serialized field drawable.

```C#
[InEditor(1)]
public static bool boolean
```

The `InEditorAttribute` can make non-serialized property drawable, too.
However, it is important to know that it will trigger `get` and `set` as well.
So be really careful with it.

```C#
[InEditor(1)]
public static bool Boolean
{
    get => boolean;
    set => boolean = value;
}
```

#### Safe

```C#
[InEditor(1)]
public static Singleton current
{ get; set; }
```

#### Not Safe

```C#
[InEditor(1)]
public static Singleton current
{
    get 
    {
        FindObjectOfType<Singleton>();
    }
}
```

When the property is drawn once, the `FindObjectOfType<Singleton>()` will be called once.

### Data of class

When we encounter some data value is packed inside a class, Mark with the attribute in the class again.
Just remember that every class is a `InEditorElement` that consists of hierarchy like data architecture.
So the mark inside the class can start with 1 again.

```C#
using System;

namespace Anyway
{
    public class YourClass : Monobehaviour
    {
        [InEditor(1)]
        [SerializeField]
        public string str;
    
        [InEditor(2)]
        private int integer;
    
        [Serializable]
        private class ManagedClass
        {
            [InEditor(1)] // Mark here, and start order from 1 agian.
            public bool boolean;
        }
        
        [InEditor(3)]
        [SerializeField]
        private ManagedClass managed;
    }
}
```

### Read-only-like

For some field, we only want to show it in the `Inspector`. But we don't want it to be changed accidentally.
`InEditorAttribute` can be mark as read-only. You can watch the value from outside the window, but not allowing
anyone going into the not safe area.

```
Calvin Harris - Outside
🎵 I'll show you what it feels like now I'm on the outside. 🎵
```

```C#
[InEditor(1, false)]
private static string thisWillDoTheTrick;
```

### Force to show as label

If something contains too much information, and you what it to show only as `LabelField`.
You can give it a `Hint` telling it to draw like label.
This is really useful when it comes to some `System` class not serializable but too deep to inspect.
Cast it to `string`, the data will automatically call `ToString()` and turned into label.

```C#
[InEditor(1, Hint = IMGUIDrawHintEnum.ToStringLabel)]
private static DateTime data;
```
