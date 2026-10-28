var ui=UnityEngine.Object.FindObjectOfType<UİManager>();
var window=UnityEditor.EditorWindow.GetWindow(System.Type.GetType("UnityEditor.GameView,UnityEditor"));window.Focus();window.Repaint();
ui.OpenElementChange();UnityEngine.Canvas.ForceUpdateCanvases();
var list=new System.Collections.Generic.List<string>();foreach(var g in ui.transitionPanel.GetComponentsInChildren<UnityEngine.CanvasGroup>(true))list.Add(g.name+" alpha="+g.alpha);
foreach(var g in ui.transitionPanel.GetComponentsInChildren<UnityEngine.UI.Graphic>(true))if(g.gameObject.activeInHierarchy)list.Add(g.name+" enabled="+g.enabled+" alpha="+g.color.a);
return string.Join("\n",list);
