"""Run Stage 1 smoke checks in the connected Unity Editor. Standard Python only.

Start with the saved GameScane scene open and Play stopped. Runtime test changes
are discarded in finally; this runner never saves scenes/assets or clears logs.
"""
import json
from pathlib import Path
import time
import urllib.request

ROOT = Path(__file__).resolve().parents[2]
HERE = Path(__file__).resolve().parent
URL = "http://127.0.0.1:8080/mcp"
session = None


def rpc(method, params):
    global session
    headers = {"Content-Type": "application/json", "Accept": "application/json, text/event-stream"}
    if session:
        headers["Mcp-Session-Id"] = session
    req = urllib.request.Request(URL, json.dumps({"jsonrpc": "2.0", "id": 1, "method": method, "params": params}).encode(), headers)
    with urllib.request.urlopen(req, timeout=60) as response:
        session = response.headers.get("Mcp-Session-Id", session)
        raw = response.read().decode()
    messages = [json.loads(line[6:]) for line in raw.splitlines() if line.startswith("data: ")]
    message = next((m for m in messages if m.get("id") == 1), None) if messages else json.loads(raw)
    if not message or "error" in message:
        raise RuntimeError(message or raw)
    return message["result"]


def tool(name, **args):
    result = rpc("tools/call", {"name": name, "arguments": args})
    payload = result.get("structuredContent")
    if payload is None:
        payload = json.loads(next(c["text"] for c in result["content"] if c["type"] == "text"))
    if result.get("isError") or payload.get("success") is False:
        raise RuntimeError(payload)
    return payload


def code(body):
    return tool("execute_code", action="execute", code=body)["data"]["result"]


def wait_ready():
    deadline = time.monotonic() + 30
    while time.monotonic() < deadline:
        try:
            if code("return UnityEngine.Object.FindObjectOfType<GameManager>() != null && ShopManager.Instance != null;"):
                return
        except RuntimeError as exc:
            # The bridge may be briefly unavailable during Play/domain reload.
            if "'message': None" not in str(exc):
                raise
        time.sleep(0.3)
    raise TimeoutError("Runtime did not initialize")


def main():
    rpc("initialize", {"protocolVersion": "2024-11-05", "capabilities": {}, "clientInfo": {"name": "Stage1Validation", "version": "1.0"}})
    request = urllib.request.Request(URL, json.dumps({"jsonrpc": "2.0", "method": "notifications/initialized"}).encode(), {"Content-Type": "application/json", "Accept": "application/json, text/event-stream", "Mcp-Session-Id": session})
    with urllib.request.urlopen(request, timeout=15) as response:
        response.read()
    rpc("resources/read", {"uri": "mcpforunity://custom-tools"})
    rpc("resources/read", {"uri": "mcpforunity://instances"})
    tool("set_active_instance", instance="BackPackDefance@64cdf686e603a5ef")
    before = code('var s=UnityEngine.SceneManagement.SceneManager.GetActiveScene(); return new{playing=UnityEditor.EditorApplication.isPlaying,dirty=s.isDirty,path=s.path,background=UnityEngine.Application.runInBackground};')
    if before["playing"] or before["dirty"] or before["path"] != "Assets/Scenes/GameScane.unity":
        raise RuntimeError("Open saved GameScane, exit Play, then run again. No changes made.")
    report = {"started": time.strftime("%Y-%m-%dT%H:%M:%S%z"), "checks": []}
    output = ROOT / "Docs/Validation/stage1-latest.json"
    output.parent.mkdir(parents=True, exist_ok=True)
    try:
        code("UnityEngine.Application.runInBackground=true; return true;")
        tool("manage_editor", action="play")
        wait_ready()
        for name in ["01_setup", "02_purchase", "03_move", "04_reject_and_shop", "05_combat", "06_victory", "07_restart", "08_defeat"]:
            rows = code((HERE / (name + ".cs")).read_text(encoding="utf-8-sig"))
            report["checks"].extend(rows)
            print(name, [(r["test"], r["passed"]) for r in rows], flush=True)
            if not all(row["passed"] for row in rows):
                raise AssertionError(name)
            # Allow Start/Destroy/coroutine completion between dependent phases.
            time.sleep(0.3)
        report["console"] = tool("read_console", action="get", types=["error", "warning"], count=100)
        report["passed"] = not report["console"].get("data")
    except Exception as exc:
        report["passed"] = False
        report["error"] = str(exc)
        raise
    finally:
        try:
            tool("manage_editor", action="stop")
            report["final"] = code("UnityEngine.Application.runInBackground=" + str(before["background"]).lower() + "; var s=UnityEngine.SceneManagement.SceneManager.GetActiveScene();return new{playing=UnityEditor.EditorApplication.isPlaying,dirty=s.isDirty,path=s.path,towers=UnityEngine.Object.FindObjectsOfType<Tower>().Length};")
        finally:
            output.write_text(json.dumps(report, ensure_ascii=False, indent=2), encoding="utf-8")
    if not report.get("passed"):
        raise SystemExit("Checks or console inspection failed; see " + str(output))
    print("PASS:", len(report["checks"]), "checks.", output)


if __name__ == "__main__":
    main()
