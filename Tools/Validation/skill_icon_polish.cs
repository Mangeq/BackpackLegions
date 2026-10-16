if(UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Stop Play first");
var path="Assets/Prefabs/UI/SkillTreeUI.prefab";var root=UnityEditor.PrefabUtility.LoadPrefabContents(path);
try
{
    string[][] files={new[]{"v1_shortbow","v1_archer_gloves","v1_warbow","v1_focus_ring","v1_moonbow","v1_hawk_pendant"},new[]{"v1_mana_elixir","v1_rune_scroll","v1_flow_amulet","v1_void_seal","v1_elder_staff","v1_archmage_robe"},new[]{"mortar_barut_kesesi_normal","mortar_seri_doldurma_takimi_normal","mortar_patlayici_gulle_normal","mortar_agir_gulle_normal","mortar_hedef_tilsimi_normal","mortar_kusatma_muhimmati_normal"},new[]{"v1_leather_armor","v1_legion_sword","v1_march_boots","v1_fortress_shield","v1_captain_armor","v1_legion_crown"}};
    var ui=root.GetComponent<SkillTreePanelUI>();for(int i=0;i<4;i++)ui.branches[i].icons=files[i].Select(f=>UnityEditor.AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Resources/Data/Items/Shop/Normal/"+f+".asset").itemIcon).ToArray();
    if(ui.branches.Any(b=>b.icons.Any(icon=>icon==null)))throw new System.Exception("Missing icon");
    ui.SelectBranch(0);UnityEditor.PrefabUtility.SaveAsPrefabAsset(root,path);
}finally{UnityEditor.PrefabUtility.UnloadPrefabContents(root);}
UnityEditor.AssetDatabase.Refresh();return "24 semantic icons assigned.";
