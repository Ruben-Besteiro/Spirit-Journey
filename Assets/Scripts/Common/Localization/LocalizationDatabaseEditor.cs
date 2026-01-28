using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocalizationDatabase_SO))]
public class LocalizationDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LocalizationDatabase_SO db = (LocalizationDatabase_SO)target;

        GUILayout.Space(10);
        GUILayout.Label("Google Sheets / CSV", EditorStyles.boldLabel);

        if (GUILayout.Button("Import CSV"))
        {
            string path = EditorUtility.OpenFilePanel(
                "Import Localization CSV",
                Application.dataPath,
                "csv"
            );

            if (!string.IsNullOrEmpty(path))
            {
                db.ImportFromCSV(path);
                EditorUtility.SetDirty(db);
            }
        }

        if (GUILayout.Button("Export CSV"))
        {
            string path = EditorUtility.SaveFilePanel(
                "Export Localization CSV",
                Application.dataPath,
                "Localization",
                "csv"
            );

            if (!string.IsNullOrEmpty(path))
            {
                db.ExportToCSV(path);
            }
        }
    }
}
