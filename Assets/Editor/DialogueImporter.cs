using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions; // ���Խ� ����� ���� �߰�

public class DialogueImporter : EditorWindow
{
    private string csvPath = "Assets/Resources/DialogueTable.csv";
    private string savePath = "Assets/Resources/Dialogues";

    // CSV�� ��ǥ�� ����� �и��ϱ� ���� ���Խ� (����ǥ ���� ��ǥ�� ����)
    private static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";

    [MenuItem("Tools/Dialogue Importer")]
    public static void ShowWindow()
    {
        GetWindow<DialogueImporter>("Dialogue Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("CSV to DialogueData Importer", EditorStyles.boldLabel);
        csvPath = EditorGUILayout.TextField("CSV Path", csvPath);
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        if (GUILayout.Button("Import CSV Now"))
        {
            ImportCSV();
        }
    }

    private void ImportCSV()
    {
        if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

        List<string> lineList = new List<string>();

        // 1. ���� ���� ���� �б� (������ ���� �־ ���� ����)
        try
        {
            using (FileStream fs = new FileStream(csvPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (!sr.EndOfStream)
                    {
                        lineList.Add(sr.ReadLine());
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"������ �д� �� ���� �߻�: {e.Message}");
            return;
        }

        string[] lines = lineList.ToArray();

        // 2. ������ ��ȯ ����
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            // ���Խ��� �̿��� ��ǥ �и� (���� �� ��ǥ ��ȣ)
            string[] values = Regex.Split(lines[i], SPLIT_RE);

            // ScriptableObject ����
            DialogueData entry = CreateInstance<DialogueData>();

            // Trim('\"')�� ����Ͽ� �� ���� ū����ǥ ����
            entry.NpcId = values[0].Trim('\"').Trim();
            entry.DialogueType = (DialogueType)System.Enum.Parse(typeof(DialogueType), values[1].Trim('\"').Trim());
            int.TryParse(values[3].Trim('\"').Trim(), out entry.ConditionValue);

            // Sentence �ʵ��� ū����ǥ ���� �� �ٹٲ� ó��
            entry.Sentence = values[4].Trim('\"').Replace("\\n", "\n").Trim();


            // ���Ϸ� ����
            string fileName = $"{entry.NpcId}_{entry.DialogueType}_{i}.asset";
            AssetDatabase.CreateAsset(entry, $"{savePath}/{fileName}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("<color=green><b>CSV ����Ʈ �Ϸ�!</b></color> Asset ���ϵ��� �����Ǿ����ϴ�.");
    }
}