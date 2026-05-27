from __future__ import annotations

import math
import random
import subprocess
from pathlib import Path

import numpy as np
from PIL import Image, ImageDraw, ImageEnhance, ImageFilter, ImageOps

from generate_horror_sprites import draw_room, seed_for


ROOT = Path(__file__).resolve().parents[2]
ROOMS = ROOT / "Assets" / "Art" / "Rooms"
PREVIEW = ROOT / "GeneratedArtifacts" / "RoomBackgrounds1920"
TARGET_SIZE = (1920, 1080)
SOURCE_SIZE = (1280, 720)

CHILD_ROOMS = {
    "child_room_east",
    "child_room_north",
    "child_room_north_drawer_empty",
    "child_room_south",
    "child_room_west",
}


def to_1920(img: Image.Image) -> Image.Image:
    return ImageOps.fit(img.convert("RGBA"), TARGET_SIZE, method=Image.Resampling.LANCZOS, centering=(0.5, 0.5))


def overlay_texture(img: Image.Image, name: str) -> Image.Image:
    rng = np.random.default_rng(seed_for(name))
    arr = np.array(img.convert("RGBA")).astype(np.float32)
    h, w = arr.shape[:2]

    fine = rng.normal(0, 8, (h, w, 1)).astype(np.float32)
    coarse = rng.normal(0, 22, (math.ceil(h / 12), math.ceil(w / 12), 1)).astype(np.float32)
    coarse_img = Image.fromarray(np.clip(coarse[:, :, 0] + 128, 0, 255).astype(np.uint8), "L")
    coarse_img = coarse_img.resize((w, h), Image.Resampling.BICUBIC).filter(ImageFilter.GaussianBlur(7))
    coarse_arr = np.array(coarse_img).astype(np.float32)[:, :, None] - 128

    vertical = np.linspace(1.05, 0.78, h, dtype=np.float32)[:, None, None]
    arr[:, :, :3] = arr[:, :, :3] * vertical + fine + coarse_arr * 0.42

    # Cold child-room color grade.
    arr[:, :, 0] *= 0.76
    arr[:, :, 1] *= 0.86
    arr[:, :, 2] *= 1.10
    arr[:, :, :3] -= 8

    y, x = np.ogrid[-1:1:h * 1j, -1:1:w * 1j]
    dist = np.sqrt((x * 0.92) ** 2 + (y * 1.12) ** 2)
    vignette = 1 - np.clip((dist - 0.18) * 0.55, 0, 0.62)
    arr[:, :, :3] *= vignette[:, :, None]

    return Image.fromarray(np.clip(arr, 0, 255).astype(np.uint8), "RGBA")


def draw_wall_damage(draw: ImageDraw.ImageDraw, name: str) -> None:
    rng = random.Random(seed_for(name) + 91)
    for _ in range(46):
        x = rng.randint(0, TARGET_SIZE[0])
        y = rng.randint(30, 740)
        pts = [(x, y)]
        for _ in range(rng.randint(3, 7)):
            x += rng.randint(-42, 38)
            y += rng.randint(12, 58)
            pts.append((x, y))
        draw.line(pts, fill=(7, 10, 16, rng.randint(58, 135)), width=rng.choice([1, 1, 2, 3]))
        if rng.random() < 0.45:
            draw.line([(px + 2, py + 1) for px, py in pts], fill=(58, 65, 78, 22), width=1)

    for _ in range(24):
        cx = rng.randint(80, TARGET_SIZE[0] - 80)
        cy = rng.randint(80, 680)
        rx = rng.randint(25, 115)
        ry = rng.randint(12, 58)
        draw.ellipse([cx - rx, cy - ry, cx + rx, cy + ry], fill=(6, 8, 12, rng.randint(10, 32)))


def add_room_specific_paint(img: Image.Image, name: str) -> Image.Image:
    d = ImageDraw.Draw(img, "RGBA")
    w, h = TARGET_SIZE
    rng = random.Random(seed_for(name) + 17)

    # Moonlight blade, matching the child-room reference without making every room identical.
    if any(token in name for token in ["child", "family_photo", "study", "attic", "master"]):
        x = rng.randint(470, 880)
        d.polygon([(x, 0), (x + 285, 0), (x - 60, h), (x - 365, h)], fill=(84, 105, 145, 20))

    # Add soft cast shadows under major interaction bands.
    d.rectangle([0, int(h * 0.66), w, int(h * 0.72)], fill=(0, 0, 0, 54))
    d.rectangle([0, int(h * 0.88), w, h], fill=(0, 0, 0, 72))

    if "family_photo_room" in name:
        d.rectangle([610, 628, 1260, 686], fill=(67, 54, 55, 155), outline=(112, 92, 84, 125), width=3)
        for x in [250, 470, 695, 980, 1285, 1520]:
            y = rng.randint(185, 340)
            d.rectangle([x, y, x + rng.randint(110, 170), y + rng.randint(145, 210)], fill=(11, 14, 21, 132), outline=(67, 59, 63, 160), width=4)
        d.rectangle([650, 360, 1140, 582], fill=(14, 17, 24, 125), outline=(76, 68, 72, 170), width=5)
    elif "laundry_room" in name:
        d.rounded_rectangle([680, 330, 1030, 720], radius=20, fill=(29, 34, 39, 165), outline=(95, 101, 103, 150), width=5)
        d.ellipse([735, 400, 980, 645], outline=(121, 128, 130, 130), width=8)
        d.rectangle([1110, 395, 1450, 720], fill=(24, 22, 23, 150), outline=(87, 73, 62, 135), width=4)
    elif "kitchen" in name:
        d.rectangle([120, 590, 1750, 780], fill=(41, 37, 36, 150), outline=(97, 84, 75, 120), width=4)
        d.rectangle([230, 355, 520, 585], fill=(24, 30, 34, 140), outline=(82, 91, 93, 130), width=4)
    elif "dining_room" in name:
        d.rectangle([420, 565, 1500, 735], fill=(43, 34, 31, 170), outline=(112, 85, 70, 120), width=4)
        for x in range(455, 1445, 195):
            d.rectangle([x, 430, x + 90, 610], fill=(23, 19, 20, 155), outline=(88, 72, 65, 110), width=3)
    elif "stairwell" in name:
        for i in range(9):
            y = 250 + i * 58
            d.polygon([(585, y), (1365, y + 28), (1265, y + 74), (485, y + 46)], fill=(39, 35, 34, 145), outline=(91, 78, 70, 105))
    elif "altar_room" in name:
        d.rectangle([615, 620, 1305, 800], fill=(31, 24, 25, 178), outline=(107, 76, 67, 135), width=4)
        d.ellipse([830, 248, 1088, 510], outline=(100, 64, 60, 112), width=8)
        if "key_spawned" in name:
            d.polygon([(950, 545), (1010, 575), (948, 604), (884, 575)], fill=(196, 151, 77, 180), outline=(235, 197, 111, 160))
        if "key_taken" in name:
            d.line([880, 575, 1015, 575], fill=(16, 12, 13, 135), width=6)
    elif "study" in name:
        d.rectangle([560, 620, 1075, 745], fill=(47, 37, 32, 160), outline=(104, 84, 70, 125), width=4)
        if "safe_open" in name:
            d.rectangle([1140, 460, 1580, 760], fill=(15, 16, 19, 180), outline=(105, 89, 73, 145), width=5)
            if "with_item" in name:
                d.rectangle([1265, 575, 1395, 640], fill=(136, 105, 62, 170), outline=(204, 159, 91, 140), width=3)
        else:
            d.rounded_rectangle([1160, 455, 1555, 760], radius=16, fill=(31, 34, 36, 165), outline=(102, 87, 71, 135), width=5)
    elif "bathroom" in name or "mirror_room" in name:
        for x in [500, 850, 1200]:
            d.rectangle([x, 180, x + 245, 720], fill=(13, 19, 28, 125), outline=(92, 108, 118, 130), width=5)
            d.line([x + 30, 220, x + 205, 670], fill=(145, 166, 184, 35), width=3)
    elif "entrance" in name:
        d.rectangle([720, 130, 1195, 795], fill=(17, 14, 16, 160), outline=(82, 66, 59, 150), width=6)
        if "chase" in name:
            d.rectangle([0, 0, w, h], fill=(75, 2, 5, 42))
    elif "attic" in name:
        d.polygon([(0, 0), (w // 2, 120), (w, 0), (w, 310), (0, 310)], fill=(10, 14, 20, 82))
        d.rectangle([220, 620, 600, 820], fill=(49, 37, 30, 150), outline=(105, 82, 66, 110), width=4)
    elif "bedroom" in name or "dressing_room" in name or "master_bedroom" in name:
        d.rectangle([270, 620, 960, 820], fill=(33, 27, 31, 160), outline=(96, 78, 74, 120), width=4)
        d.rectangle([1180, 225, 1660, 760], fill=(20, 17, 20, 145), outline=(91, 74, 70, 130), width=5)

    draw_wall_damage(d, name)
    return img


def make_room(path: Path) -> None:
    name = path.stem
    if name in CHILD_ROOMS:
        with Image.open(path) as existing:
            img = to_1920(existing)
        img = ImageEnhance.Sharpness(img).enhance(1.12)
        img = overlay_texture(img, name)
    else:
        base = draw_room(name, SOURCE_SIZE)
        img = to_1920(base)
        img = add_room_specific_paint(img, name)
        img = img.filter(ImageFilter.UnsharpMask(radius=1.6, percent=120, threshold=4))
        img = overlay_texture(img, name)
    img.save(path)


def copy_ai_family_photo(generated_path: Path | None) -> None:
    if generated_path is None or not generated_path.exists():
        return
    target = ROOMS / "family_photo_room_north.png"
    with Image.open(generated_path) as img:
        out = to_1920(img)
        out = overlay_texture(out, "family_photo_room_north_ai")
        out.save(target)


def contact_sheet(paths: list[Path]) -> Path:
    PREVIEW.mkdir(parents=True, exist_ok=True)
    thumb_w, thumb_h = 320, 180
    cols = 4
    rows = math.ceil(len(paths) / cols)
    sheet = Image.new("RGB", (cols * thumb_w, rows * (thumb_h + 26)), (12, 14, 19))
    draw = ImageDraw.Draw(sheet)
    for idx, path in enumerate(paths):
        with Image.open(path) as img:
            thumb = ImageOps.fit(img.convert("RGB"), (thumb_w, thumb_h), method=Image.Resampling.LANCZOS)
        x = (idx % cols) * thumb_w
        y = (idx // cols) * (thumb_h + 26)
        sheet.paste(thumb, (x, y))
        draw.text((x + 6, y + thumb_h + 5), path.stem[:44], fill=(180, 184, 194))
    out = PREVIEW / "contact_sheet.png"
    sheet.save(out)
    return out


def sample_deleted_head_rooms() -> None:
    PREVIEW.mkdir(parents=True, exist_ok=True)
    samples = [
        "Assets/Resources/EscapeFromNightmares/Rooms/family_photo_room_north.png",
        "Assets/Resources/EscapeFromNightmares/Rooms/child_room_west.png",
    ]
    for sample in samples:
        out = PREVIEW / ("head_" + Path(sample).name)
        result = subprocess.run(["git", "cat-file", "-e", f"HEAD:{sample}"], cwd=ROOT)
        if result.returncode != 0:
            continue
        data = subprocess.check_output(["git", "show", f"HEAD:{sample}"], cwd=ROOT)
        out.write_bytes(data)


def main() -> None:
    import argparse

    parser = argparse.ArgumentParser()
    parser.add_argument("--ai-family-photo", type=Path)
    args = parser.parse_args()

    paths = sorted(ROOMS.glob("*.png"))
    for path in paths:
        make_room(path)
    copy_ai_family_photo(args.ai_family_photo)
    sample_deleted_head_rooms()
    sheet = contact_sheet(paths)
    print(f"rooms={len(paths)}")
    print(f"contact_sheet={sheet}")


if __name__ == "__main__":
    main()
