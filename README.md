# UIElements
Various scripts related to Unity UI Toolkit (UIElements). These are not all plug-and-play but should serve as examples you can adopt for your own needs. Look in `_misc` for more stylesheets to help you get started with styling these components.

* [Aspect Ratio Padding](#aspectratiopadding)
* [Blur/Glass Effect](#blurglass-effect)
* [Color Field & Color Picker](#color-field-and-popup)
* [Popup panels](#popup-panels) (include General, Message and TextField popups)
* [Tooltip](#tooltip)

## Color Field and Popup

![Image of colorfield](/Images/colorfield.webp)

A Color field and Color picker popup. You will need the popup and blur panel related scripts too if you want to use it as is.

The popup element should be at bottom of the document's root so that it can appear over other elements. 

```
	...
    <Game.UI.ColorPopup name="color-popup" start-visible="false" />
</ui:UXML>
```

Insert the color field somewhere via UXML.

```
<Game.UI.ColorField name="grid-fill-color" label="Fill Colour" reset-label="\U0000f0e2" />
```

Link the color field with the color popup.

```cs
var colorPopup = root.Q<ColorPopup>("color-popup");
var gridFillColorField = root.Q<ColorField>("grid-fill-color");
gridFillColorField.ColorPopup = colorPopup;
gridFillColorField.ResetButtonPressed += () => gridFillColorField.value = Const.DefaultGridFillColor;
```

## Popup panels

![Image of popup](/Images/popup-edit.png)
![Image of popup](/Images/popup-msg.png)

Examples of how modal popup panels can be done. These will add a fullscreen element to prevent mouse clicks from reaching elements behind. Of course these need to be added to the root of a document and at the bottom so they appear over other elements.

```
    <Game.UI.PopupPanel name="settings-popup" class="settings-popup" start-visible="false">
        <ui:Instance template="SettingsPanel" class="settings-panel fill" />        
        <ui:Button text="\U0000f00d" name="popup-close-button" class="popup-close-button" />
    </Game.UI.PopupPanel>
	
    <Game.UI.PopupTextField name="popup-textfield" start-visible="false" />
    <Game.UI.PopupMessage name="popup-message" start-visible="false" />
</ui:UXML>
```
Game.UI.PopupPanel can be used to contain whatever you want to be in a popup panel  (for example a Settings panel) or you can derive from it to create a more specialised popup, like the included PopupTextField, PopupMessage, and ColorPopup examples.

```cs
var popupMsg = root.Q<PopupMessage>("popup-message");
var popupEd = root.Q<PopupTextField>("popup-textfield");


popupMsg.Show("Lable", "message", "Close");

// 20 = how many characters allowed
popupEdit.Show("label", "message", "empty or text to set", 20,
	t => 
	{ 	// submit was pressed
		popupEdit.Hide();
		Debug.Log("Player entered: " + t);
	},
	() => 
	{	// cancel was pressed
		popupEdit.Hide();
	});
```

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

The tooltip will appear below the item by default but you can also hint to it to appear above, or to the left or right. Simply attach the following in front of the tooltip text to apply the hint. "L:" for left, "R:" for right, "T:" for above, and "B:" for below.

For example, to have the tooltip appear to the right-hand side of a button you would do this

`<ui:Button name="some-button" text="Button" tooltip="R:Settings" />`


## AspectRatioPadding

![Image of AspectRatioPadding](/Images/aspectratio.png)

This element will add padding/margin to the left and right and update it such to push the "central" items to fit into a certain aspect ratio. So, if you choose to have a 4:3 ratio but this element is at 16:9 then padding is added to the left and right, same for when you design at 16:9 but the game is played on ultra wide. For anything at or below your design ratio the padding will be 0.

Sample, 4:3 design on 16:9 "display".

```
<ui:UXML xmlns:ui="UnityEngine.UIElements" xmlns:uie="UnityEditor.UIElements" editor-extension-mode="False">
    <Game.UI.AspectRatioPadding width="4" height="3" style="flex-direction: row; background-color: yellow;">
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

```cs
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


