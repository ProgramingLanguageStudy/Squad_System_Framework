using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVReader : MonoBehaviour
{
    // ������ ù ��° ��(���)�� �����ϰ� �б� ���� ���Խ�
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static List<DialogueData> Read(string file)
    {
        var list = new List<DialogueData>();
        // Resources �������� ������ �о�ɴϴ�.
        TextAsset data = Resources.Load<TextAsset>(file);

        if (data == null)
        {
            Debug.LogError($"������ ã�� �� �����ϴ�: {file}");
            return list;
        }

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        // ù ��° ���� ���(������)�̹Ƿ� i = 1���� �����մϴ�.
        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            // ���ο� DialogueData ��ü ���� (ScriptableObject)
            // ����: ���� ������Ʈ������ ���Ϸ� �����ϰų� ����Ʈ�θ� ������ �� �ֽ��ϴ�.
            DialogueData entry = ScriptableObject.CreateInstance<DialogueData>();

            entry.NpcId = values[0].Trim(TRIM_CHARS);

            // Enum ��ȯ (Quest, Common, Affection)
            entry.DialogueType = (DialogueType)System.Enum.Parse(typeof(DialogueType), values[1]);

            entry.ConditionKey = values[2].Trim(TRIM_CHARS);

            // ���ڷ� ��ȯ
            int.TryParse(values[3], out entry.ConditionValue);

            // ��� ���� (����ǥ ����)
            entry.Sentence = values[4].Trim(TRIM_CHARS).Replace("\\n", "\n"); // \n�� ���� �ٹٲ����� ����

            // AfterActionEvent �߰�
            if (values.Length > 5)
            {
                entry.AfterActionEvent = values[5].Trim(TRIM_CHARS);
            }

            list.Add(entry);
        }
        return list;
    }
}