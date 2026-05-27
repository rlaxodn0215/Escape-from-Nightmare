import argparse
import hashlib
import math
import os
import random
import re
from pathlib import Path

import numpy as np
from PIL import Image, ImageDraw, ImageFilter, ImageFont


ROOT = Path(__file__).resolve().parents[2]
ART_ROOT = ROOT / "Assets" / "Art"
CATALOG = ROOT / "Assets" / "Resources" / "EscapeFromNightmares" / "ResourcePathCatalog.asset"

SAMPLES = {
    "child_room_north",
    "first_floor_hallway_north",
    "study_north",
    "kitchen_north",
    "basement_main_west",
    "study_safe_locked",
    "kitchen_clock_clue",
    "dining_seat_order_clue",
    "item_front_door_key",
    "ui_rotate_left",
    "ui_rotate_right",
    "monster_shadow",
}


def font(size, bold=False):
    candidates = [
        r"C:\Windows\Fonts\malgunbd.ttf" if bold else r"C:\Windows\Fonts\malgun.ttf",
        r"C:\Windows\Fonts\arialbd.ttf" if bold else r"C:\Windows\Fonts\arial.ttf",
        r"C:\Windows\Fonts\segoeuib.ttf" if bold else r"C:\Windows\Fonts\segoeui.ttf",
    ]
    for path in candidates:
        if path and Path(path).exists():
            return ImageFont.truetype(path, size)
    return ImageFont.load_default()


def clamp(value):
    return max(0, min(255, int(value)))


def rgba(hex_color, alpha=255):
    hex_color = hex_color.lstrip("#")
    return tuple(int(hex_color[i : i + 2], 16) for i in (0, 2, 4)) + (alpha,)


def seed_for(name):
    digest = hashlib.sha256(name.encode("utf-8")).hexdigest()
    return int(digest[:8], 16)


def add_noise(img, amount=18, seed=0):
    rng = np.random.default_rng(seed)
    arr = np.array(img).astype(np.int16)
    noise = rng.normal(0, amount, arr.shape[:2]).astype(np.int16)
    for channel in range(3):
        arr[:, :, channel] = np.clip(arr[:, :, channel] + noise, 0, 255)
    return Image.fromarray(arr.astype(np.uint8), "RGBA")


def add_vignette(img, strength=0.65):
    w, h = img.size
    y, x = np.ogrid[-1:1:h * 1j, -1:1:w * 1j]
    d = np.sqrt(x * x + y * y)
    mask = 1 - np.clip(d * strength, 0, 0.72)
    arr = np.array(img).astype(np.float32)
    arr[:, :, :3] *= mask[:, :, None]
    return Image.fromarray(np.clip(arr, 0, 255).astype(np.uint8), "RGBA")


def cracked_wall(draw, w, h, seed, density=18):
    rng = random.Random(seed)
    for _ in range(density):
        x = rng.randint(0, w)
        y = rng.randint(0, int(h * 0.75))
        points = [(x, y)]
        for _ in range(rng.randint(3, 6)):
            x += rng.randint(-24, 24)
            y += rng.randint(10, 38)
            points.append((x, y))
        draw.line(points, fill=(18, 22, 30, 130), width=rng.randint(1, 2))


def base_scene(size, name, floor=True):
    w, h = size
    img = Image.new("RGBA", size, rgba("#111824"))
    d = ImageDraw.Draw(img, "RGBA")
    d.rectangle([0, 0, w, int(h * 0.68)], fill=rgba("#1b2634"))
    d.rectangle([0, int(h * 0.68), w, h], fill=rgba("#11151d"))
    if floor:
        for i in range(9):
            y = int(h * (0.70 + i * 0.035))
            d.line([0, y, w, y + 18], fill=(55, 58, 66, 45), width=1)
        for i in range(12):
            x = int(w * i / 12)
            d.line([x, int(h * 0.69), x - 70, h], fill=(45, 47, 55, 35), width=1)
    cracked_wall(d, w, h, seed_for(name))
    return img


def draw_door(draw, box, open_gap=False, knob=True):
    x1, y1, x2, y2 = box
    draw.rectangle(box, fill=rgba("#17141a"), outline=rgba("#4c3d42"), width=4)
    inset = max(6, int((x2 - x1) * 0.06))
    draw.rectangle([x1 + inset, y1 + inset, x2 - inset, y2 - inset], outline=rgba("#3e3437"), width=2)
    if open_gap:
        draw.polygon([(x1, y1), (x2, y1 + 15), (x2, y2 - 15), (x1, y2)], fill=(7, 8, 12, 190))
    if knob:
        r = max(4, int((x2 - x1) * 0.035))
        draw.ellipse([x2 - inset * 2 - r, (y1 + y2) / 2 - r, x2 - inset * 2 + r, (y1 + y2) / 2 + r], fill=rgba("#8a6d55"))


def draw_window(draw, box, moon=False):
    x1, y1, x2, y2 = box
    draw.rectangle(box, fill=rgba("#0c1624"), outline=rgba("#3a4c5e"), width=3)
    draw.line([(x1 + x2) / 2, y1, (x1 + x2) / 2, y2], fill=rgba("#263849"), width=2)
    draw.line([x1, (y1 + y2) / 2, x2, (y1 + y2) / 2], fill=rgba("#263849"), width=2)
    if moon:
        draw.ellipse([x1 + 15, y1 + 18, x1 + 48, y1 + 51], fill=(190, 205, 235, 120))


def draw_safe(draw, box, open_state="closed", item=False):
    x1, y1, x2, y2 = box
    draw.rounded_rectangle(box, radius=12, fill=rgba("#202326"), outline=rgba("#6a5a45"), width=4)
    if open_state == "closed":
        draw.rectangle([x1 + 24, y1 + 28, x2 - 24, y2 - 28], outline=rgba("#4a4035"), width=3)
        draw.ellipse([x1 + 46, y1 + 52, x1 + 96, y1 + 102], outline=rgba("#9a7b43"), width=4)
        draw.text((x1 + 120, y1 + 58), "3 1 4 2", font=font(22, True), fill=rgba("#8f7756"))
    else:
        draw.polygon([(x1 + 12, y1 + 14), (x1 + 110, y1 + 48), (x1 + 110, y2 - 28), (x1 + 12, y2 - 12)], fill=rgba("#161719"), outline=rgba("#756049"))
        draw.rectangle([x1 + 130, y1 + 45, x2 - 35, y2 - 45], fill=rgba("#0b0c0e"), outline=rgba("#4f4438"))
        if item:
            draw.rectangle([x1 + 190, y1 + 110, x1 + 270, y1 + 158], fill=rgba("#8e744f"), outline=rgba("#c09a5f"), width=2)
            draw.line([x1 + 205, y1 + 120, x1 + 255, y1 + 145], fill=rgba("#2c251d"), width=3)


def room_theme(name):
    if "child_room" in name:
        return "child"
    if "hallway" in name:
        return "hallway"
    if "study" in name:
        return "study"
    if "kitchen" in name:
        return "kitchen"
    if "basement" in name:
        return "basement"
    if "attic" in name:
        return "attic"
    if "altar" in name:
        return "altar"
    if "laundry" in name:
        return "laundry"
    if "bathroom" in name:
        return "bathroom"
    if "mirror" in name:
        return "mirror"
    if "dining" in name:
        return "dining"
    if "dressing" in name or "master" in name:
        return "bedroom"
    if "entrance" in name:
        return "entrance"
    if "stairwell" in name:
        return "stairwell"
    return "room"


def draw_room(name, size):
    w, h = size
    img = base_scene(size, name)
    d = ImageDraw.Draw(img, "RGBA")
    theme = room_theme(name)
    direction = name.rsplit("_", 1)[-1]
    if theme == "child":
        draw_window(d, [40, 70, 185, 330], moon=True)
        d.rectangle([330, 390, 650, 450], fill=rgba("#3a302a"), outline=rgba("#6e5e52"), width=3)
        d.rectangle([410, 450, 620, 610], fill=rgba("#2a2422"), outline=rgba("#5c514b"), width=3)
        d.rectangle([665, 135, 1010, 350], outline=rgba("#4f4540"), width=5)
        for i in range(5):
            d.rectangle([700 + i * 55, 165 + i % 2 * 35, 760 + i * 55, 245 + i % 2 * 25], fill=rgba("#595653"))
        d.arc([900, 450, 1120, 670], 200, 350, fill=rgba("#6b5a4d"), width=4)
        d.polygon([(990, 430), (1045, 500), (990, 548)], fill=rgba("#5b5149"), outline=rgba("#8d7561"))
        d.ellipse([80, 565, 190, 680], fill=rgba("#211a17"), outline=rgba("#4c3a2c"))
        if "drawer_empty" in name:
            d.rectangle([500, 470, 610, 525], fill=rgba("#100f11"), outline=rgba("#6b5a4d"), width=2)
    elif theme == "hallway":
        count = 4 if "north" in name else 3
        xs = [80, 330, 610, 910] if count == 4 else [120, 480, 840]
        for i, x in enumerate(xs):
            draw_door(d, [x, 125, x + 180, 540], open_gap=("chase" in name and i == len(xs) - 1))
        d.arc([-80, -160, w + 80, 440], 190, 350, fill=(65, 72, 88, 85), width=5)
        if "chase" in name:
            d.rectangle([0, 0, w, h], fill=(45, 5, 7, 40))
    elif theme == "study":
        draw_window(d, [48, 110, 270, 520], moon=True)
        d.rectangle([360, 420, 720, 500], fill=rgba("#3d3028"), outline=rgba("#6e5b4a"), width=3)
        d.rectangle([780, 90, 1160, 300], outline=rgba("#5e5142"), width=5)
        for i in range(6):
            d.line([815, 125 + i * 26, 1110, 110 + i * 18], fill=rgba("#8a8170"), width=2)
        state = "open" if "safe_open" in name else "closed"
        draw_safe(d, [760, 330, 1110, 635], state, item=("with_item" in name))
    elif theme == "kitchen":
        d.rectangle([80, 395, 1180, 600], fill=rgba("#302c28"), outline=rgba("#5f554a"), width=3)
        d.rectangle([160, 265, 320, 390], fill=rgba("#1f2527"), outline=rgba("#5a6261"), width=3)
        d.ellipse([510, 96, 655, 241], fill=rgba("#2a2727"), outline=rgba("#887765"), width=4)
        d.line([582, 168, 520, 168], fill=rgba("#111318"), width=7)
        d.line([582, 168, 650, 168], fill=rgba("#111318"), width=4)
        draw_door(d, [910, 130, 1110, 560])
    elif theme == "basement":
        d.rectangle([0, 0, w, h], fill=(13, 21, 24, 90))
        for x in range(120, 1200, 180):
            d.line([x, 0, x + 70, h], fill=rgba("#28343a"), width=4)
        d.rectangle([420, 190, 800, 610], fill=rgba("#171a1c"), outline=rgba("#4b5558"), width=4)
        if "west" in name or "hide_view" in name:
            d.rectangle([360, 160, 850, 660], fill=rgba("#151719"), outline=rgba("#4c4c46"), width=5)
    elif theme == "altar":
        d.rectangle([410, 410, 870, 575], fill=rgba("#231c1c"), outline=rgba("#68534a"), width=4)
        d.ellipse([560, 190, 720, 340], outline=rgba("#694840"), width=5)
        for x in (360, 920):
            d.rectangle([x, 310, x + 25, 540], fill=rgba("#4f3d34"))
            d.ellipse([x - 12, 285, x + 37, 330], fill=(180, 120, 55, 110))
        if "key_spawned" in name:
            d.polygon([(650, 362), (690, 382), (650, 402), (610, 382)], fill=rgba("#c39a55"))
    elif theme == "attic":
        d.polygon([(0, 0), (w // 2, 80), (w, 0), (w, 220), (0, 220)], fill=rgba("#141a22"))
        d.rectangle([120, 420, 360, 610], fill=rgba("#3a2c23"), outline=rgba("#6a5748"))
        d.arc([760, 330, 1020, 640], 180, 350, fill=rgba("#6e5849"), width=4)
        draw_door(d, [900, 120, 1115, 570], open_gap="east" in name)
    elif theme == "bathroom":
        d.rectangle([430, 130, 850, 500], fill=rgba("#0f151b"), outline=rgba("#6a7276"), width=5)
        d.ellipse([520, 185, 760, 425], outline=rgba("#879098"), width=5)
        d.rectangle([130, 455, 430, 610], fill=rgba("#1f2629"), outline=rgba("#59656a"))
        draw_door(d, [930, 130, 1120, 565])
    elif theme == "mirror":
        for x in [180, 470, 760]:
            d.rectangle([x, 120, x + 210, 555], fill=rgba("#111925"), outline=rgba("#687b86"), width=4)
            d.line([x + 20, 150, x + 180, 520], fill=(135, 150, 165, 70), width=2)
        draw_door(d, [980, 140, 1160, 565])
    elif theme == "dining":
        d.rectangle([250, 380, 980, 535], fill=rgba("#30251f"), outline=rgba("#705746"), width=4)
        for x in [250, 430, 610, 790, 970]:
            d.rectangle([x - 30, 315, x + 30, 410], fill=rgba("#211b19"), outline=rgba("#5c4c42"))
        draw_door(d, [940, 120, 1120, 560])
    elif theme == "bedroom":
        d.rectangle([190, 430, 640, 610], fill=rgba("#211b1f"), outline=rgba("#5f4e4a"), width=4)
        d.rectangle([760, 150, 1110, 585], fill=rgba("#1a171a"), outline=rgba("#5a4b48"), width=4)
        draw_door(d, [930, 130, 1130, 570])
    elif theme == "entrance":
        draw_door(d, [480, 90, 800, 610], open_gap="chase" in name)
        d.rectangle([130, 370, 370, 550], fill=rgba("#201b18"), outline=rgba("#5c4a3f"))
        if "chase" in name:
            d.rectangle([0, 0, w, h], fill=(55, 0, 0, 45))
    elif theme == "stairwell":
        for i in range(9):
            y = 165 + i * 45
            d.polygon([(380, y), (900, y + 18), (830, y + 44), (310, y + 26)], fill=rgba("#2b2725"), outline=rgba("#5c5148"))
        draw_door(d, [900, 110, 1100, 555])
    else:
        draw_door(d, [540, 130, 740, 570])
    img = add_noise(img, 10, seed_for(name))
    img = add_vignette(img, 0.62)
    return img


def draw_closeup(name, size):
    w, h = size
    img = base_scene(size, name, floor=False)
    d = ImageDraw.Draw(img, "RGBA")
    cx, cy = w // 2, h // 2
    if "safe_locked" in name or "safe_open" in name:
        draw_safe(d, [int(w * 0.29), int(h * 0.15), int(w * 0.72), int(h * 0.85)], "open" if "open" in name else "closed", item=("with_item" in name))
    elif "kitchen_clock" in name:
        d.ellipse([cx - 230, cy - 230, cx + 230, cy + 230], fill=rgba("#242225"), outline=rgba("#7d6b58"), width=8)
        d.ellipse([cx - 180, cy - 180, cx + 180, cy + 180], fill=rgba("#686060"), outline=rgba("#19191c"), width=4)
        for i in range(12):
            a = math.radians(i * 30 - 90)
            r1, r2 = 140, 170
            d.line([cx + math.cos(a) * r1, cy + math.sin(a) * r1, cx + math.cos(a) * r2, cy + math.sin(a) * r2], fill=rgba("#15171c"), width=5)
        d.line([cx, cy, cx - 150, cy], fill=rgba("#0b0d12"), width=10)
        d.line([cx, cy, cx + 170, cy], fill=rgba("#0b0d12"), width=5)
    elif "dining_seat_order" in name:
        tokens = [("DOLL", "#65504a"), ("TRAIN", "#514a5f"), ("BLOCK", "#665a35"), ("BELL", "#6b5641")]
        for i, (label, color) in enumerate(tokens):
            x = int(w * (0.16 + i * 0.22))
            d.rectangle([x, 210, x + 140, 420], fill=rgba("#211c1b"), outline=rgba("#80634e"), width=3)
            d.text((x + 24, 300), str(i + 1), font=font(50, True), fill=rgba("#c7a66b"))
            d.text((x + 10, 365), label, font=font(24, True), fill=rgba(color))
    elif "color_sequence" in name:
        colors = [("#050505", "BLACK"), ("#e5e1d4", "WHITE"), ("#8c1d1d", "RED"), ("#777777", "GRAY")]
        for i, (color, label) in enumerate(colors):
            x = int(w * (0.17 + i * 0.19))
            d.rectangle([x, 250, x + 130, 390], fill=rgba(color), outline=rgba("#7e6650"), width=4)
            d.text((x + 25, 420), label, font=font(22, True), fill=rgba("#b99661"))
    elif "wall_symbols" in name or "mirror_rule" in name or "symbol" in name:
        labels = ["heart", "child hand", "cracked circle", "keyhole"]
        for i, label in enumerate(labels):
            x = int(w * (0.15 + i * 0.21))
            y = int(h * 0.30)
            if label == "heart":
                d.ellipse([x + 25, y, x + 75, y + 48], fill=rgba("#7b2026"))
                d.ellipse([x + 65, y, x + 115, y + 48], fill=rgba("#7b2026"))
                d.polygon([(x + 25, y + 25), (x + 115, y + 25), (x + 70, y + 95)], fill=rgba("#7b2026"))
            elif label == "child hand":
                d.ellipse([x + 45, y + 36, x + 110, y + 100], fill=rgba("#8b7860"))
                for finger in range(4):
                    d.rounded_rectangle([x + 28 + finger * 19, y + 5, x + 44 + finger * 19, y + 58], radius=8, fill=rgba("#8b7860"))
                d.rounded_rectangle([x + 25, y + 55, x + 55, y + 78], radius=9, fill=rgba("#8b7860"))
            elif label == "cracked circle":
                d.ellipse([x + 28, y + 5, x + 118, y + 95], outline=rgba("#a28a65"), width=6)
                d.line([x + 78, y + 8, x + 62, y + 42, x + 88, y + 56, x + 70, y + 93], fill=rgba("#1a1d22"), width=5)
            else:
                d.ellipse([x + 52, y + 8, x + 96, y + 52], fill=rgba("#0b0d11"), outline=rgba("#9c825e"), width=5)
                d.polygon([(x + 64, y + 50), (x + 84, y + 50), (x + 98, y + 100), (x + 50, y + 100)], fill=rgba("#0b0d11"), outline=rgba("#9c825e"))
            d.text((x - 10, y + 120), label, font=font(22, True), fill=rgba("#9b856c"))
            d.line([x - 18, y + 155, x + 145, y + 138], fill=rgba("#5c4439"), width=3)
    elif "drawer" in name or "desk_surface" in name:
        d.rectangle([140, 250, w - 140, 560], fill=rgba("#3a2a22"), outline=rgba("#7d6551"), width=5)
        if "open" in name:
            d.rectangle([260, 330, w - 260, 500], fill=rgba("#0c0b0c"), outline=rgba("#5d4b3e"), width=4)
            if "with_item" in name:
                d.polygon([(cx - 40, 390), (cx + 80, 420), (cx + 20, 470)], fill=rgba("#c0a779"), outline=rgba("#4a3327"))
    elif "window_view" in name:
        draw_window(d, [150, 70, w - 150, h - 80], moon=True)
        for i in range(7):
            x = 190 + i * 140
            d.line([x, h - 100, x + 50, 180], fill=rgba("#0a0e14"), width=6)
    elif "portrait" in name or "album_photo" in name:
        d.rectangle([260, 80, w - 260, h - 90], fill=rgba("#4b443e"), outline=rgba("#836b51"), width=7)
        d.ellipse([cx - 70, 185, cx + 70, 325], fill=rgba("#211d20"))
        d.rectangle([cx - 130, 330, cx + 130, 515], fill=rgba("#1a1618"))
    elif "drawing_board" in name or "clue_board" in name or "clue_note" in name:
        d.rectangle([210, 120, w - 210, h - 110], fill=rgba("#6e604d"), outline=rgba("#3f3229"), width=6)
        text = "3 1 4 2" if "safe" in name else "REMEMBER"
        d.text((cx - 110, cy - 30), text, font=font(52, True), fill=rgba("#241d1b"))
        for i in range(5):
            d.line([260, 210 + i * 60, w - 260, 230 + i * 45], fill=rgba("#302720"), width=2)
    else:
        d.rectangle([250, 170, w - 250, h - 150], fill=rgba("#2b2422"), outline=rgba("#7c6651"), width=5)
    img = add_noise(img, 8, seed_for(name))
    img = add_vignette(img, 0.55)
    return img


def draw_item(name, size):
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    d = ImageDraw.Draw(img, "RGBA")
    w, h = size
    cx, cy = w // 2, h // 2
    if "key" in name:
        d.line([cx - 120, cy, cx + 105, cy], fill=rgba("#b08b50"), width=28)
        d.ellipse([cx - 170, cy - 55, cx - 60, cy + 55], outline=rgba("#c8a86b"), width=22)
        d.rectangle([cx + 70, cy - 35, cx + 110, cy - 10], fill=rgba("#b08b50"))
        d.rectangle([cx + 70, cy + 10, cx + 125, cy + 35], fill=rgba("#b08b50"))
    elif "fuse" in name:
        d.rounded_rectangle([150, 210, 360, 300], radius=24, fill=rgba("#4d3c2f"), outline=rgba("#b4935a"), width=8)
        d.rectangle([120, 230, 160, 280], fill=rgba("#a7844d"))
        d.rectangle([350, 230, 392, 280], fill=rgba("#a7844d"))
    elif "mirror" in name:
        d.ellipse([150, 80, 360, 310], fill=rgba("#151d29"), outline=rgba("#b59a69"), width=14)
        d.line([190, 145, 310, 255], fill=rgba("#d7dbe0"), width=5)
        d.rectangle([230, 300, 280, 450], fill=rgba("#6f543d"), outline=rgba("#b28d5e"), width=5)
    elif "doll" in name:
        d.ellipse([200, 80, 312, 190], fill=rgba("#6a574c"), outline=rgba("#9b8060"), width=5)
        d.rectangle([185, 190, 330, 380], fill=rgba("#3a2c32"), outline=rgba("#7f5a58"), width=5)
    elif "necklace" in name:
        d.arc([135, 85, 380, 360], 25, 335, fill=rgba("#b4945d"), width=10)
        d.polygon([(cx, 315), (cx + 48, 370), (cx, 430), (cx - 48, 370)], fill=rgba("#693136"), outline=rgba("#c89a64"))
    elif "fragment" in name or "clue" in name:
        d.polygon([(120, 140), (380, 95), (420, 370), (170, 430)], fill=rgba("#79684f"), outline=rgba("#30251e"))
        d.text((175, 230), "3 1 4 2" if "safe" in name else "CLUE", font=font(38, True), fill=rgba("#211913"))
    else:
        d.rectangle([130, 150, 380, 370], fill=rgba("#6c5842"), outline=rgba("#b3925e"), width=8)
    return add_noise(img, 4, seed_for(name))


def draw_object(name, size):
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    d = ImageDraw.Draw(img, "RGBA")
    w, h = size
    if "door" in name:
        draw_door(d, [150, 40, 360, 470])
    elif "drawer" in name:
        d.rectangle([80, 160, 430, 380], fill=rgba("#3a2a22"), outline=rgba("#7d6551"), width=8)
        d.ellipse([245, 250, 275, 280], fill=rgba("#b28d5c"))
    elif "bed" in name:
        d.rectangle([70, 280, 440, 410], fill=rgba("#211c20"), outline=rgba("#6e5850"), width=7)
        d.rectangle([90, 220, 260, 300], fill=rgba("#342630"))
    else:
        d.rectangle([160, 70, 350, 450], fill=rgba("#0b0d12"))
    return add_noise(img, 4, seed_for(name))


def draw_puzzle(name, size):
    if "digit_" in name:
        digit = re.search(r"digit_(\d)", name).group(1)
        img = Image.new("RGBA", size, (0, 0, 0, 0))
        d = ImageDraw.Draw(img, "RGBA")
        d.rounded_rectangle([18, 12, size[0] - 18, size[1] - 12], radius=12, fill=rgba("#2b211b"), outline=rgba("#a77c43"), width=5)
        d.text((size[0] * 0.34, size[1] * 0.20), digit, font=font(int(size[1] * 0.55), True), fill=rgba("#c9a066"))
        return img
    img = base_scene(size, name, floor=False)
    d = ImageDraw.Draw(img, "RGBA")
    w, h = size
    if "study_safe" in name:
        draw_safe(d, [240, 80, 660, 540], "closed")
    elif "laundry" in name:
        d.rectangle([240, 150, 660, 470], fill=rgba("#2b2d2f"), outline=rgba("#8e7c62"), width=6)
        d.text((360, 280), "0 9 1 5", font=font(46, True), fill=rgba("#b89a66"))
    elif "toy" in name:
        labels = ["DOLL", "TRAIN", "BLOCK", "BELL"]
        for i, label in enumerate(labels):
            d.text((140 + i * 175, 270), label, font=font(30, True), fill=rgba("#b79561"))
    elif "altar" in name:
        d.text((150, 240), "mirror  doll  key  necklace", font=font(36, True), fill=rgba("#b99661"))
    elif "mirror" in name:
        d.text((120, 250), "heart  child hand  cracked circle  keyhole", font=font(34, True), fill=rgba("#b99661"))
    elif "drawer" in name:
        colors = [("#050505", "black"), ("#e6e0d0", "white"), ("#8c1d1d", "red"), ("#777777", "gray")]
        for i, (c, _) in enumerate(colors):
            d.rectangle([180 + i * 155, 230, 290 + i * 155, 350], fill=rgba(c), outline=rgba("#b99661"), width=4)
    elif "breaker" in name:
        d.rectangle([260, 120, 640, 500], fill=rgba("#24272a"), outline=rgba("#82705a"), width=6)
        d.text((360, 300), "FUSE", font=font(44, True), fill=rgba("#b99661"))
    else:
        d.text((220, 270), name.replace("_", " "), font=font(36, True), fill=rgba("#b99661"))
    return add_vignette(add_noise(img, 7, seed_for(name)), 0.50)


def draw_ui(name, size):
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    d = ImageDraw.Draw(img, "RGBA")
    w, h = size
    label_map = {
        "button_start": "START",
        "button_quit": "QUIT",
        "button_settings": "SETTINGS",
        "button_close": "CLOSE",
        "inventory_button": "BAG",
        "settings_header": "SETTINGS",
        "settings_label_master": "MASTER",
        "settings_label_bgm": "BGM",
        "settings_label_sfx": "SFX",
        "settings_label_ui": "UI",
    }
    if name in {"ui_rotate_left", "ui_rotate_right", "ui_back_arrow", "ui_hide_exit"}:
        flip = name in {"ui_rotate_left", "ui_back_arrow", "ui_hide_exit"}
        pts = [(w * 0.20, h * 0.50), (w * 0.78, h * 0.12), (w * 0.60, h * 0.50), (w * 0.78, h * 0.88)] if flip else [(w * 0.80, h * 0.50), (w * 0.22, h * 0.12), (w * 0.40, h * 0.50), (w * 0.22, h * 0.88)]
        d.polygon(pts, fill=rgba("#b8873c"), outline=rgba("#f0d17a"))
        for i in range(15):
            x = random.Random(seed_for(name) + i).randint(8, max(9, w - 8))
            y = random.Random(seed_for(name) + i * 3).randint(8, max(9, h - 8))
            d.ellipse([x - 2, y - 2, x + 2, y + 2], fill=rgba("#4a2816", 180))
    elif "slider" in name:
        d.rounded_rectangle([4, h * 0.25, w - 4, h * 0.75], radius=int(h * 0.25), fill=rgba("#1b1715"), outline=rgba("#80623b"), width=3)
        if "fill" in name:
            d.rounded_rectangle([4, h * 0.25, w * 0.72, h * 0.75], radius=int(h * 0.25), fill=rgba("#9d7337"))
        if "handle" in name:
            d.ellipse([4, 4, w - 4, h - 4], fill=rgba("#b8873c"), outline=rgba("#f0d17a"), width=3)
    else:
        d.rounded_rectangle([4, 4, w - 4, h - 4], radius=min(18, max(6, h // 8)), fill=rgba("#171312", 235), outline=rgba("#8a6537"), width=4)
        label = label_map.get(name, "")
        if label:
            fs = max(16, min(48, int(h * 0.45)))
            bbox = d.textbbox((0, 0), label, font=font(fs, True))
            d.text(((w - (bbox[2] - bbox[0])) / 2, (h - (bbox[3] - bbox[1])) / 2 - 3), label, font=font(fs, True), fill=rgba("#e0bd72"))
    return add_noise(img, 3, seed_for(name))


def draw_monster(size):
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    d = ImageDraw.Draw(img, "RGBA")
    w, h = size
    d.ellipse([w * 0.36, h * 0.08, w * 0.64, h * 0.24], fill=(4, 5, 8, 220))
    d.polygon([(w * 0.34, h * 0.22), (w * 0.66, h * 0.22), (w * 0.78, h * 0.72), (w * 0.60, h * 0.96), (w * 0.40, h * 0.96), (w * 0.22, h * 0.72)], fill=(3, 4, 7, 205))
    d.polygon([(w * 0.40, h * 0.16), (w * 0.45, h * 0.19), (w * 0.37, h * 0.20)], fill=(150, 20, 22, 160))
    d.polygon([(w * 0.60, h * 0.16), (w * 0.55, h * 0.19), (w * 0.63, h * 0.20)], fill=(150, 20, 22, 160))
    return img.filter(ImageFilter.GaussianBlur(2))


def draw_ending(size):
    img = base_scene(size, "stage1_clear_background")
    d = ImageDraw.Draw(img, "RGBA")
    w, h = size
    draw_door(d, [w * 0.40, h * 0.12, w * 0.60, h * 0.75], open_gap=True)
    d.polygon([(w * 0.48, h * 0.18), (w * 0.60, h * 0.20), (w * 0.60, h * 0.75), (w * 0.48, h * 0.75)], fill=(210, 195, 150, 65))
    d.text((w * 0.33, h * 0.78), "ESCAPED", font=font(54, True), fill=rgba("#c8a76d"))
    return add_vignette(add_noise(img, 8, 42), 0.50)


def render(path):
    name = path.stem
    category = path.parent.name
    with Image.open(path) as old:
        size = old.size
    if category == "Rooms":
        img = draw_room(name, size)
    elif category in {"CloseUps", "HideViews"}:
        img = draw_closeup(name, size)
    elif category == "Items":
        img = draw_item(name, size)
    elif category == "Objects":
        img = draw_object(name, size)
    elif category == "Puzzles":
        img = draw_puzzle(name, size)
    elif category == "UI":
        img = draw_ui(name, size)
    elif category == "Monster":
        img = draw_monster(size)
    elif category == "Endings":
        img = draw_ending(size)
    else:
        img = base_scene(size, name)
    img.save(path)


def relink_catalog():
    asset_by_id = {}
    for meta_path in ART_ROOT.rglob("*.png.meta"):
        text = meta_path.read_text(encoding="utf-8", errors="ignore").splitlines()
        guid = next((line.split(":", 1)[1].strip() for line in text if line.startswith("guid:")), None)
        internal = None
        for line in text:
            match = re.match(r"\s*internalID:\s*(-?\d+)", line)
            if match:
                internal = match.group(1)
                break
        asset_by_id[meta_path.name.replace(".png.meta", "")] = (guid, internal or "21300000")
    lines = CATALOG.read_text(encoding="utf-8").splitlines()
    updated = 0
    for i, line in enumerate(lines):
        match = re.match(r"  - spriteId: (.+)", line)
        if not match:
            continue
        sprite_id = match.group(1).strip()
        guid, file_id = asset_by_id[sprite_id]
        new_line = f"    sprite: {{fileID: {file_id}, guid: {guid}, type: 3}}"
        if i + 1 < len(lines) and lines[i + 1] != new_line:
            lines[i + 1] = new_line
            updated += 1
    CATALOG.write_text("\n".join(lines) + "\n", encoding="utf-8")
    return updated


def validate():
    pngs = list(ART_ROOT.rglob("*.png"))
    invalid = []
    for path in pngs:
        try:
            with Image.open(path) as img:
                img.verify()
        except Exception:
            invalid.append(str(path))
    temps = list(ART_ROOT.rglob("*.tmp"))
    return {"png": len(pngs), "invalid": len(invalid), "temps": len(temps), "invalid_paths": invalid}


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--samples", action="store_true")
    parser.add_argument("--all", action="store_true")
    parser.add_argument("--validate", action="store_true")
    parser.add_argument("--relink", action="store_true")
    args = parser.parse_args()
    if args.samples or args.all:
        targets = []
        for path in ART_ROOT.rglob("*.png"):
            if args.all or path.stem in SAMPLES:
                targets.append(path)
        for path in sorted(targets):
            render(path)
        print(f"generated={len(targets)}")
    if args.relink:
        print(f"catalog_updated={relink_catalog()}")
    if args.validate:
        print(validate())


if __name__ == "__main__":
    main()
