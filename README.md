# UIElements
Various scripts related to Unity UI Toolkit (UIElements).


## Tooltip

![Image of Tooltip](/Images/tooltip.gif)

UIElements does not have tooltip support at runtime. Here's some code to show how you can get it working.

You want to add something like this near the end of a UXML document. It needs to be at the end so that the tooltip will appear over other elements and not behind them.

- Delay is how long it waits (ms) before it shows up.
- Fade Time is how how quick or slow it fades in and out. 0 for no fade.

`<Game.UI.Tooltip name="tooltip" picking-mode="Ignore" delay="500" fade-time="15" />`

Now you need to bind this to all the buttons and other elements which can show a tooltip. You probably have some c# script where you are doing all kinds of bindings already so just put it in there.

```cs
var tooltip = root.Q<Tooltip>("tooltip");
var button = root.Q<Button>("some-button");
button.AddManipulator(new TooltipManipulator(tooltip));
```

The tooltip text comes from an element's tooltip attribute, for example ...

`<ui:Button name="some-button" text="Button" tooltip="Settings" />`

The tooltip will appear below the item by default but you can also hint to it to appear above, or to the left or right. Simply attach the following in front of the the tooltip text to apply the hint. "L:" for left, "R:" for right, "T:" for above, and "B:" for below.

For example, to have the tooltip appear to the right-hand side of a button you would do this

`<ui:Button name="some-button" text="Button" tooltip="R:Settings" />`


## AspectRatioPadding

![Image of AspectRatioPadding](/Images/aspectratio.png)

This is a row layout element which will add padding elements to the left and right and update them to push the "central" items to fit into a certain aspect ratio. So, if you choose to have a 4:3 ratio but this element is at 16:9 then padding is added to the left and right, same for when you design at 16:9 but the game is player on ultra wide. 

Sample, 4:3 design on 16:9 "display".

```
<ui:UXML xmlns:ui="UnityEngine.UIElements" xmlns:uie="UnityEditor.UIElements" editor-extension-mode="False">
    <Game.UI.AspectRatioPadding width="4" height="3" style="flex-grow:1; background-color: yellow;">
        <ui:VisualElement style="width:20px; background-color: red;" />
        <ui:VisualElement style="flex-grow:1; background-color: green;" />
        <ui:VisualElement style="width:20px; background-color: blue;" />
    </Game.UI.AspectRatioPadding>
</ui:UXML>
```


## Blur/Glass Effect

![Image of Blur](/Images/blur.webp)

An example of doing a blur or milky glass effect. I am using URP but this could be adopted for other since a main part of this is a Rendertexture which provides the panel with the image information it needs.

You can insert the Blur Panel like any other element into UXML, or via C#.

`<Game.UI.BlurPanel name="scene-toolbar" class="toolbar-back">`

Then in C# you need to tell it about the blur image (render texture).

```csharp
[SerializeField] private Texture blurTexture;
...
root.Q<BlurPanel>("scene-toolbar").SetImage(blurTexture);
```

If the Blur Panel is inside other elements that might move then you need to specify the moving parent too else the Blur will not update correctly.

`panel.SetImage(blurTexture, someParentElement);`

Shader information from https://github.com/ArneBezuijen/urp_kawase_UI_blur and https://github.com/sebastianhein/urp_kawase_blur and cleand up for this project.

Add the Blur Render Feature to the Forward Renderer Data via inspector. A Blur Render camera is used to render only a panel which uses a Blur material. Think of this camera as viewing the rendered world via a window (a plane using a blur shader material) and then rendering that to a texture. This texture is then used by the UI element. Note the culling mask and layer used by camera and plane object.

![Image of Blur](/Images/blur2.png)

![Image of Blur](/Images/blur3.png)



