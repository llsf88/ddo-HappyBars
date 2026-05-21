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

### Gaps

Use `H gap px` and `V gap px` to control the horizontal and vertical spacing between hotbars.

Both default to `5` pixels.

### Save Hotbars

Saves the current hotbar positions, orientation, and current character.

Save files are stored in the `saves` folder and use the character name and character ID.

### Load Hotbars

Loads the saved layout for the current character.

HappyBars looks for the character ID first, then falls back to the character name.

If a hotbar needs to change orientation, it rotates first and moves afterward.

### Snap Hotbars

When enabled, manually dragging a hotbar shows a visual snap preview.

Snaps are chosen from the hotbar edges, similar to window snapping.

Long edges are divided into possible slots based on the size of the hotbar being moved.

It can snap above, below, left, or right, while avoiding overlaps with other hotbars and positions outside the game client.

### ESC

Press `ESC` during Set Hotbars or Load Hotbars to stop moving hotbars.

---

made by Dronael from Tharne. Only possible to be done because of the support of the amazing people in the DH discord. Feel free to contact me to fix bugs or with new ideas.
