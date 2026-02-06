using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVReader : MonoBehaviour
{
    // CSV의 첫 번째 줄(헤더)을 제외하고 읽기 위한 정규식
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static List<DialogueData> Read(string file)
    {
        var list = new List<DialogueData>();
        // Resources 경로에서 파일을 읽어옵니다.
        TextAsset data = Resources.Load<TextAsset>(file);

        if (data == null)
        {
            Debug.LogError($"파일을 찾을 수 없습니다: {file}");
            return list;
        }

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        // 첫 번째 줄은 헤더(컬럼명)이므로 i = 1부터 시작합니다.
        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            DialogueData entry = ScriptableObject.CreateInstance<DialogueData>();
            entry.id = $"{values[0].Trim(TRIM_CHARS)}_{i}";
            entry.npcId = values.Length > 0 ? values[0].Trim(TRIM_CHARS) : "";
            if (values.Length > 1) System.Enum.TryParse(values[1].Trim(TRIM_CHARS), out entry.dialogueType);
            if (values.Length > 3) int.TryParse(values[3].Trim(TRIM_CHARS), out entry.conditionValue);
            string raw = values.Length > 4 ? values[4].Trim(TRIM_CHARS).Replace("\\n", "\n") : "";
            entry.lines = string.IsNullOrEmpty(raw) ? System.Array.Empty<string>() : raw.Split('/');
            list.Add(entry);
        }
        return list;
    }
}
