using UnityEngine;

public class LabReturnState : MonoBehaviour
{
   private const string PosX = "LabPlayerPosX";
    private const string PosY = "LabPlayerPosY";
    private const string PosZ = "LabPlayerPosZ";
    private const string RotY = "LabPlayerRotY";
    private const string TeacherDoneKey = "LabTeacherDone";
    private const string SelectedScreenKey = "LabSelectedScreenId";

    public static bool HasSavedPose =>
        PlayerPrefs.HasKey(PosX) && PlayerPrefs.HasKey(PosY) &&
        PlayerPrefs.HasKey(PosZ) && PlayerPrefs.HasKey(RotY);

    public static void SavePlayerPose(Transform t)
    {
        Vector3 p = t.position;
        PlayerPrefs.SetFloat(PosX, p.x);
        PlayerPrefs.SetFloat(PosY, p.y);
        PlayerPrefs.SetFloat(PosZ, p.z);
        PlayerPrefs.SetFloat(RotY, t.eulerAngles.y);
        PlayerPrefs.Save();
    }

    public static void RestorePlayerPose(Transform t)
    {
        if (!HasSavedPose) return;
        float x = PlayerPrefs.GetFloat(PosX);
        float y = PlayerPrefs.GetFloat(PosY);
        float z = PlayerPrefs.GetFloat(PosZ);
        float ry = PlayerPrefs.GetFloat(RotY);

        t.position = new Vector3(x, y, z);
        var e = t.eulerAngles;
        e.y = ry;
        t.eulerAngles = e;
    }

    public static void SetTeacherDone(bool done)
    {
        PlayerPrefs.SetInt(TeacherDoneKey, done ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool IsTeacherDone() => PlayerPrefs.GetInt(TeacherDoneKey, 0) == 1;

    public static void SetSelectedScreenId(int id)
    {
        PlayerPrefs.SetInt(SelectedScreenKey, id);
        PlayerPrefs.Save();
    }

    public static int GetSelectedScreenId()
    {
        return PlayerPrefs.GetInt(SelectedScreenKey, -1);
    }

    public static void Reset()
    {
        PlayerPrefs.DeleteKey(PosX);
        PlayerPrefs.DeleteKey(PosY);
        PlayerPrefs.DeleteKey(PosZ);
        PlayerPrefs.DeleteKey(RotY);
        PlayerPrefs.DeleteKey(TeacherDoneKey);
        PlayerPrefs.DeleteKey(SelectedScreenKey);
        PlayerPrefs.Save();
    }

}

