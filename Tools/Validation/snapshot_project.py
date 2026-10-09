"""Create and verify a project source ZIP. Does not modify or delete source files."""
from datetime import datetime
import hashlib
import json
from pathlib import Path
import zipfile

ROOT = Path(__file__).resolve().parents[2]
folders = ["Assets", "Packages", "ProjectSettings", "Docs", "Tools"]
destination = ROOT / "Backups" / ("stage1-baseline-" + datetime.now().strftime("%Y%m%d-%H%M%S") + ".zip")
destination.parent.mkdir(exist_ok=True)
manifest = {}
with zipfile.ZipFile(destination, "x", compression=zipfile.ZIP_DEFLATED, compresslevel=6) as archive:
    for folder in folders:
        for path in sorted((ROOT / folder).rglob("*")):
            if not path.is_file() or path.is_symlink() or "__pycache__" in path.parts:
                continue
            path.resolve().relative_to(ROOT.resolve())
            name = path.relative_to(ROOT).as_posix()
            data = path.read_bytes()
            manifest[name] = hashlib.sha256(data).hexdigest()
            archive.writestr(name, data)
    archive.writestr("SNAPSHOT_MANIFEST.json", json.dumps(manifest, indent=2))
with zipfile.ZipFile(destination) as archive:
    for name, expected in manifest.items():
        if hashlib.sha256(archive.read(name)).hexdigest() != expected:
            raise RuntimeError("Backup verification failed: " + name)
print(json.dumps({"path": str(destination), "files": len(manifest), "verified": True, "bytes": destination.stat().st_size}, indent=2))
