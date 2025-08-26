using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class StudentReportWrapper
{
    public List<StudentProfile> students;
}

public static class GeneoReportParser
{
    public static StudentProfile LoadProfileById(string studentId)
    {
        TextAsset jsonData = Resources.Load<TextAsset>("StudentReports/data");
        if (jsonData != null)
        {
            StudentReportWrapper wrapper = JsonUtility.FromJson<StudentReportWrapper>(jsonData.text);
            var found = wrapper.students.Find(s => s.studentId == studentId);
            if (found != null)
            {
                return found;
            }
        }

        Debug.LogWarning("Student not found in GeneoReportParser.");
        return null;
    }

}
