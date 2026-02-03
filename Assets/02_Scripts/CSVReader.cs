using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVReader : MonoBehaviour
{
    // 엑셀의 첫 번째 줄(헤더)을 제외하고 읽기 위한 정규식
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static List<DialogueData> Read(string file)
    {
        var list = new List<DialogueData>();
        // Resources 폴더에서 파일을 읽어옵니다.
        TextAsset data = Resources.Load<TextAsset>(file);

        if (data == null)
        {
            Debug.LogError($"파일을 찾을 수 없습니다: {file}");
            return list;
        }

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        // 첫 번째 줄은 헤더(변수명)이므로 i = 1부터 시작합니다.
        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            // 새로운 DialogueData 객체 생성 (ScriptableObject)
            // 주의: 실제 프로젝트에서는 파일로 저장하거나 리스트로만 관리할 수 있습니다.
            DialogueData entry = ScriptableObject.CreateInstance<DialogueData>();

            entry.NpcId = values[0].Trim(TRIM_CHARS);

            // Enum 변환 (Quest, Common, Affection)
            entry.DialogueType = (DialogueType)System.Enum.Parse(typeof(DialogueType), values[1]);

            entry.ConditionKey = values[2].Trim(TRIM_CHARS);

            // 숫자로 변환
            int.TryParse(values[3], out entry.ConditionValue);

            // 대사 내용 (따옴표 제거)
            entry.Sentence = values[4].Trim(TRIM_CHARS).Replace("\\n", "\n"); // \n을 실제 줄바꿈으로 변경

            // AfterActionEvent 추가
            if (values.Length > 5)
            {
                entry.AfterActionEvent = values[5].Trim(TRIM_CHARS);
            }

            list.Add(entry);
        }
        return list;
    }
}