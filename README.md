# DefinetlyNotAFishingBot

A pixel-based fishing bot for World of Warcraft: Wrath of the Lich King (client 3.3.5a), intended for private servers where fishing bots are permitted. It works purely by looking at the screen and sending regular keyboard/mouse input — no memory reading, no DLL injection, no administrator rights.

## How it works

The developer places the see-through overlay window ("Where is the Fish?!") over the patch of water where the bobber lands; the bot then runs this cycle:

1. **Start sequence** (once): hide the game UI with Alt+Y (so no interface element covers the bobber or disturbs the detection; it is restored on stop, and a checkbox in the settings turns this off entirely) → equip the fishing outfit → wait → apply the lure onto the now-equipped pole → wait for its 5 s apply channel → apply the buff → wait. Every step is followed by a pause so the server round trip completes.
2. **Loop**: wait the refish time → park the mouse just outside the capture area (a bobber landing under the cursor would glow from the mouse-over effect and break the color match) → cast → scan the overlay region for the bobber. A pixel only counts when it is both within the color tolerance **and** at least half as red-dominant as the picked color — that dominance requirement is what separates the red feather from brownish water and terrain.
3. It watches the found bobber (a lime rectangle in the preview marks where it believes the bobber is) at ~25–30 scans per second — fast enough for the short dip, and only possible because just the small watch region is captured per scan, not the whole overlay. When the matching pixels collapse into the splash or the bobber's center visibly dips down — confirmed by two consecutive scans, and never within the first seconds where no fish can bite yet — that is the bite.
4. On the bite it moves the mouse onto the bobber immediately, waits the loot time (the client registers the hover meanwhile), right-clicks to loot, and waits for the server.
5. Before the next cast it re-applies lure and buff when their timers (~9 minutes) say they are about to expire — the real buffs cannot be read without touching game memory, so timers are the signal.

The bot verifies that WoW really has the input focus before every key press and click (bringing it to the foreground itself when needed), so no input can land in the wrong window.

Because input is sent with `SendInput`, Windows only allows it when the game does **not** run at a higher integrity level. In short: never start WoW "as administrator", and the bot itself needs no elevation either.

## One-time setup in WoW

- Run WoW in **Windowed** or **Windowed (Maximized)** mode — the overlay cannot sit on top of exclusive fullscreen.
- Enable **Auto Loot** (Interface → Controls), so a plain right-click on the bobber loots everything.
- Put **Fishing** on the action bar slot for your fishing key (default: `2`).
- Put a **lure macro** on the slot for your lure key (default: `0`):

  ```
  /use Bright Baubles
  /use 16
  ```

  Slot 16 is the main hand, i.e. the equipped fishing pole. Any lure works — just swap the name.
- Optional: a buff (e.g. food/drink) on the buff key and an equip-fishing-gear macro on the outfit key. If you don't use one of them, set its key to `None` in the bot — it will be skipped.
- Close the chat input and any open windows before starting, and make sure the character stands at the water facing it.

## Using the bot

1. Start `DefinetlyNotAFishingBot.exe`. Three windows appear: the settings window, the overlay, and a console (see below; it can simply be minimized).
2. Drag and resize the overlay over the water where the bobber usually lands. The position and size are saved to the config, so the overlay comes back up in the same spot next time. The preview at the bottom of the settings window shows exactly what the bot sees. Keep the region tight: less water = faster and fewer false matches. Aim for the bobber landing in the **middle** of the region — if it lands right at an edge, drifting half out of the region looks like a bite and casts that land just outside are wasted.
3. Cast manually once, then **click the bobber's red feather in the preview**. The picked color appears on the swatch and is saved for future sessions.
4. Set the **color tolerance** (start around 25–40). Too low and the bobber isn't found; too high and water/UI pixels match too. If the status reports "Color matches too much water", lower it or re-pick the color.
5. Click **Start**. The settings window minimizes itself when it overlaps the overlay — parked elsewhere (e.g. on another screen) it stays open; the overlay is always visible either way. The bot focuses WoW and takes over — keep your hands off mouse and keyboard. The overlay's frame color shows what the bot is doing: **red** = bot off, **blue** = fishing, **green** = looting a bite. Restore the settings window at any time to watch the preview: a lime rectangle marks where the bot sees the bobber, the status text is colored by state (green = running, yellow = paused, red = stopped), and a **Loots** counter shows the catches of the current run (reset on every start).
6. **Escape stops the bot** at any time, no matter which window has the focus — including the game; the settings window pops back up if it was minimized. The Escape key is only claimed while the bot runs; once stopped it behaves normally in the game again. A hint label in the settings window states this too.
7. To pause instead, just alt-tab or click any other window: the bot pauses immediately and shows "Paused" in the status. Click back into WoW to let it resume, or press **Stop** in the settings window.

## Console commands

The executable is a console application: the console window shows a timestamped log of everything the bot does (focus, key presses, bobber position and pixel counts, bites, catches). When started from a terminal — or with redirected pipes, e.g. by an AI agent testing it — it accepts commands on stdin:

```
start   # same as clicking the Start button
stop    # same as clicking the Stop button
status  # print the current status line
quit    # close the application
```

When the program is started by double-click, the console can simply be minimized and ignored.

## Settings

- **Refish Timer** — pause after each attempt before casting again.
- **Loot Timer** — pause after clicking the bobber so the loot lands in the bags.
- **Color tolerance** — how far (per RGB channel, 0–255) a pixel may deviate from the picked bobber color and still count.
- All settings, including the picked bobber color, are stored in `%USERPROFILE%\.definetly_not_a_fishing_bot_config.json`.

## Troubleshooting

- **"Bobber not found — recasting…" every time**: raise the tolerance, re-pick the color from a fresh cast, make the overlay region smaller, or turn the camera so the water fills the region without red-ish clutter behind it.
- **"Bobber not found" on some casts only**: the bobber sometimes lands outside the overlay region — enlarge the region or re-aim it so casts land near its middle. The bot recovers by recasting, it just costs time.
- **Bot clicks but nothing is looted**: enable Auto Loot; make sure the overlay region really covers where the bobber lands so the click position is correct.
- **False bites (clicks without a splash)**: lower the tolerance and avoid regions with moving background (waterfalls, other players).
- **Keys don't arrive in the game**: WoW must have the focus (the bot handles this) and must not be running elevated. Windows display scaling is safest at 100%.
- **"WoW window not found"**: the 3.3.5a client window is searched by its window class (`GxWindowClassD3d`) and by the title "World of Warcraft"; start the game before pressing Start.

## Building

```
nuget restore DefinetlyNotAFishingBot.sln
msbuild DefinetlyNotAFishingBot.sln
```

Output: `DefinetlyNotAFishingBot\bin\Debug\DefinetlyNotAFishingBot.exe` (.NET Framework 4.8).
