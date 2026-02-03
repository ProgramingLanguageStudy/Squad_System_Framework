using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions; // 정규식 사용을 위해 추가

public class DialogueImporter : EditorWindow
{
    private string csvPath = "Assets/Resources/DialogueTable.csv";
    private string savePath = "Assets/Resources/Dialogues";

    // CSV의 쉼표를 제대로 분리하기 위한 정규식 (따옴표 안의 쉼표는 무시)
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

        // 1. 파일 공유 모드로 읽기 (엑셀이 켜져 있어도 에러 방지)
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
            Debug.LogError($"파일을 읽는 중 에러 발생: {e.Message}");
            return;
        }

        string[] lines = lineList.ToArray();

        // 2. 데이터 변환 시작
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            // 정규식을 이용해 쉼표 분리 (문장 내 쉼표 보호)
            string[] values = Regex.Split(lines[i], SPLIT_RE);

            // ScriptableObject 생성
            DialogueData entry = CreateInstance<DialogueData>();

            // Trim('\"')을 사용하여 양 끝의 큰따옴표 제거
            entry.NpcId = values[0].Trim('\"').Trim();
            entry.DialogueType = (DialogueType)System.Enum.Parse(typeof(DialogueType), values[1].Trim('\"').Trim());
            entry.ConditionKey = values[2].Trim('\"').Trim();

            int.TryParse(values[3].Trim('\"').Trim(), out entry.ConditionValue);

            // Sentence 필드의 큰따옴표 제거 및 줄바꿈 처리
            entry.Sentence = values[4].Trim('\"').Replace("\\n", "\n").Trim();

            if (values.Length > 5)
            {
                entry.AfterActionEvent = values[5].Trim('\"').Trim();
            }

            // 파일로 저장
            string fileName = $"{entry.NpcId}_{entry.DialogueType}_{i}.asset";
            AssetDatabase.CreateAsset(entry, $"{savePath}/{fileName}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("<color=green><b>CSV 임포트 완료!</b></color> Asset 파일들이 생성되었습니다.");
    }
}