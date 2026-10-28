var ui=UnityEngine.Object.FindObjectOfType<UİManager>();ui.OpenElementChange();
var output=new System.Collections.Generic.List<string>();
output.Add("panel "+ui.transitionPanel.activeSelf+" hierarchy "+ui.transitionPanel.activeInHierarchy+" parent "+ui.transitionPanel.transform.parent.name);
foreach(var t in ui.transitionPanel.GetComponentsInChildren<UnityEngine.RectTransform>(true))output.Add(t.name+" rect="+t.rect+" scale="+t.lossyScale+" pos="+t.position);
return string.Join("\n",output);
