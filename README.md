The minimum Dungeon Helper version required for this plugin to work correctly is the current Beta version available at https://dungeonhelper.com/beta

This plugin was created with the help of https://gitlab.com/slick-dh-tools/uiruler therefore has parts of its code in it. Thanks for @Slick818 for providing the source code of UIRuler.

# HappyBars

HappyBars helps arrange, save, load, and snap DDO hotbars.

## How To Use

### Set Hotbars

Use the text box in the HappyBars tab to describe the layout you want.

Each line becomes a row. Values separated by commas become columns.

Example:

```txt
1,5
2,6
3,7
```

### Empty Spaces

Use `x` or `0` when you want to leave an empty slot in the layout.

Example:

```txt
x,3
4,6
9,5
```

### Vertical Hotbars

Add `V` after a hotbar number to place that hotbar vertically.

Example:

```txt
1,5v
2,6v
```

### Anchor And Screen Limits

The first numbered hotbar in the first column is used as the anchor.

If the full layout would go past the game client border, HappyBars moves the anchor first, then places the other hotbars around it.

The bottom of the client is treated as 30 pixels shorter to avoid the XP bar.

### Save Hotbars

Saves the current hotbar positions, orientation, and current character.

The save file uses the character name and character ID.

### Load Hotbars

Loads the saved layout for the current character.

HappyBars looks for the character ID first, then falls back to the character name.

If a hotbar needs to change orientation, it rotates first and moves afterward.

### Snap Hotbars

When enabled, manually dragging a hotbar shows a visual snap preview.

HappyBars chooses the neighboring hotbar closest to your mouse, then previews the final position.

It can snap above, below, left, or right, while avoiding overlaps with other hotbars.

### ESC

Press `ESC` during Set Hotbars or Load Hotbars to stop moving hotbars.
