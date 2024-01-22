using UnityEditor;

[CustomEditor(typeof(WaveManager))]
public class WaveManagerEditor : Editor
{
    SerializedProperty startEnemyCount;
    SerializedProperty enemyCountMultiplier;
    SerializedProperty bossInterval;
    SerializedProperty spawnDelay;
    SerializedProperty roundData;
    SerializedProperty intermissionTimes;

    private void OnEnable()
    {
        startEnemyCount = serializedObject.FindProperty("startEnemyCount");
        enemyCountMultiplier = serializedObject.FindProperty("enemyCountMultiplier");
        bossInterval = serializedObject.FindProperty("bossRoundInterval");
        spawnDelay = serializedObject.FindProperty("spawnDelay");
        roundData = serializedObject.FindProperty("roundData");
        intermissionTimes = serializedObject.FindProperty("intermissionTimes");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(startEnemyCount);
        EditorGUILayout.PropertyField(enemyCountMultiplier);
        EditorGUILayout.PropertyField(bossInterval);
        EditorGUILayout.PropertyField(spawnDelay);
        EditorGUILayout.PropertyField(roundData);
        EditorGUILayout.PropertyField(intermissionTimes);
        EditorGUILayout.HelpBox("Ensure that the total of enemyData probabilities in each roundData is equal to 1", MessageType.Warning);

        serializedObject.ApplyModifiedProperties();
    }
}