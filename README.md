# UIElements
Various scripts related to Unity UI Toolkit (UIElements).

## AspectRatioPadding

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

![Image of AspectRatioPadding](/Images/aspectratio.png)
