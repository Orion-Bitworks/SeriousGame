public enum LevelID
{
    Pipe = 0,
    Heart = 1
}

public static class LevelProgress
{
    private static GameData data;

    static LevelProgress()
    {
        data = SaveSystem.Load();
    }

    public static void CompleteLevel(int index)
    {
        data.levelsCompleted[index] = true;
        SaveSystem.Save(data);
    }

    public static bool IsLevelCompleted(int index)
    {
        return data.levelsCompleted[index];
    }

    public static void ResetProgress()
    {
        data = new GameData();
        SaveSystem.Save(data);
    }
}