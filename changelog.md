# Change Log

## [1.0.0] 2023/06/29

### [Added] IMGUI Serialized Property Field

- Added `field` to draw serialized property
'cause Unity under 2022.1 is not able to get `boxedValue`.

### [Added] IMGUI Field

- Added `IMGUIField` for drawing every not serialized properties.
  - `IMGUIAnimationCurveField`
  - `IMGUIBoundsField`
  - `IMGUIBoundsIntField`
  - `IMGUIColor32Field`
  - `IMGUIColorField`
  - `IMGUIDoubleField`
  - `IMGUIFloatField`
  - `IMGUIGradientField`
  - `IMGUIIntField`
  - `IMGUILayerMaskField`
  - `IMGUILongField`
  - `IMGUIObjectField`
  - `IMGUIRectField`
  - `IMGUIRectIntField`
  - `IMGUITextField`
  - `IMGUIToggleField`
  - `IMGUIVector2Field`
  - `IMGUIVector2IntField`
  - `IMGUIVector3Field`
  - `IMGUIVector3IntField`
  - `IMGUIVector4Field`
- Added `IMGUIFold` for drawing `foldout`

## [1.0.1] 2023/06/30

### [Changed] Lower Unity Version

- `HandledMemberTarget` now rewrite to accept 2019 with no 
`boxedValue`.
- `switch` pattern rewritten to `case`s & blocks.

### [Added] README.md

## [1.0.2] 2023/06/30

### [Changed] Lower Unity Version to 2019.1

### [Changed] Rename `DesignatedIMGUI` to `Hint`

- To adapt older version used in old project, which is not public, rename it to `Hint`.

### [Added] `InEditorAttribute`.`Disabled`

- Shows `field` with `DisabledScope`.

### [Added] `InEditorAttribute`.`Method`

- Shows `field` with pre-define custom `Editor` or `Monobehaviour` `method`.

### [Fixed] Not Serialized Field of `Gradient` Crash Unity Editor

- Now if `field` of `Gradient` with value of `null` will call `new Gradient()`